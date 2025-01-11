using MelonLoader;
using System;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using ClockStone;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Net.Http;

namespace NeonSoundReplacerNamespace
{
    public class NeonSoundReplacer : MelonMod
    {
        public static new HarmonyLib.Harmony Harmony { get; private set; }
        public static string modVersion;
        public static bool checkedForUpdates = false;
        public static Dictionary<string, string> customSounds = new Dictionary<string, string>();
        public static Dictionary<string, float> customVolumes = new Dictionary<string, float>();

        public override void OnInitializeMelon()
        {
            MelonInfoAttribute melonInfo = Assembly.GetExecutingAssembly().GetCustomAttribute<MelonInfoAttribute>();
            if (melonInfo != null) modVersion = melonInfo.Version;
        }

        [Obsolete]
        public override void OnApplicationLateStart()
        {
            Harmony = new HarmonyLib.Harmony("NeonSoundReplacer");

            SceneManager.activeSceneChanged += OnActiveSceneChange;

            MethodInfo method = typeof(AudioController).GetMethod("PlayAudioItem");
            HarmonyMethod harmonyMethod = new HarmonyMethod(typeof(NeonSoundReplacer).GetMethod("Post_PlayAudioItem"));
            Harmony.Patch(method, null, harmonyMethod);
        }
        private void OnActiveSceneChange(Scene previousScene, Scene newScene)
        {
            if(newScene.name == "Menu") MelonCoroutines.Start(CheckForUpdates());
        }

        public static void Post_PlayAudioItem(AudioItem sndItem, AudioObject __result)
        {
            if (shouldLogSounds.Value) MelonLogger.Msg("Original Audio Name: " + sndItem.Name);
            if (customSounds.ContainsKey(sndItem.Name))
            {
                String newName = customSounds[sndItem.Name];
                float volume = customVolumes.ContainsKey(sndItem.Name) ? customVolumes[sndItem.Name] : 1.0f;
                if (shouldLogSounds.Value) MelonLogger.Msg("============= Replaced Sound: " + newName + " with volume: " + volume);
                if (toggleSounds.Value) MelonCoroutines.Start(ReplaceAudioClip(__result, newName, volume));
            }
        }

       private static IEnumerator ReplaceAudioClip(AudioObject audioObject, String newName, float volume)
        {
            UnityWebRequest clip = UnityWebRequestMultimedia.GetAudioClip("file://" + Path.Combine(Environment.CurrentDirectory, "Mods", "NeonSoundReplacer", newName), AudioType.UNKNOWN);
            yield return clip.SendWebRequest();
            if (clip.result == UnityWebRequest.Result.Success)
            {
                AudioClip audioclip = DownloadHandlerAudioClip.GetContent(clip);
                if (audioclip != null)
                {
                    AudioSource audioSource = audioObject.GetComponent<AudioSource>();
                    if (audioSource != null)
                    {
                        audioSource.volume = 0f;
                        audioSource.Stop();

                        if (audioSource.loop)
                        {
                            MelonCoroutines.Start(PlaySoundRepeatedly(audioSource, audioclip, volume));
                        }
                        else
                        {
                            audioSource.PlayOneShot(audioclip, volume);
                        }
                    }
                    else
                    {
                        MelonLogger.Error("AudioSource component not found.");
                    }
                }
                else
                {
                    MelonLogger.Error("Failed to get AudioClip.");
                }
            }
            else
            {
                MelonLogger.Error("UnityWebRequest for " + newName + " failed: " + clip.error);
            }
        }

        public override void OnPreferencesSaved()
        {
            RefreshCustomSounds();
        }
        public override void OnPreferencesLoaded()
        {
            RefreshCustomSounds();
        }
        private void RefreshCustomSounds()
        {
            customSounds.Clear();
            customVolumes.Clear();
            string[] lines = soundsToReplace.Value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.Contains("="))
                {
                    string[] splitLine = line.Split(new char[] { '=' }, 2);
                    string key = splitLine[0].Trim().Trim('"'); // in case someone puts quotes and spaces
                    string[] valueParts = splitLine[1].Trim().Trim('"').Split(new char[] { ' ' }, 2);
                    string value = valueParts[0].Trim();
                    float volume = 1.0f; // default volume
                    if (valueParts.Length > 1 && float.TryParse(valueParts[1], out float parsedVolume))
                    {
                        volume = parsedVolume;
                    }
                    if (!customSounds.ContainsKey(key))
                    {
                        customSounds.Add(key, value);
                        customVolumes.Add(key, volume);
                    }
                    else
                    {
                        MelonLogger.Error($"Duplicate sound found: {key}. Skipping this entry.");
                    }
                }
                else
                {
                    MelonLogger.Error("Can't process line: " + line);
                    MelonLogger.Error("Make sure you have a '=' in the line");
                }
            }
            MelonLogger.Msg("Refreshed custom sounds");
        }
        
        private static IEnumerator PlaySoundRepeatedly(AudioSource audioSource, AudioClip audioClip, float volume)
        {
            while (true)
            {
                audioSource.PlayOneShot(audioClip, volume);
                yield return new WaitForSeconds(audioClip.length);
            }
        }

        public IEnumerator CheckForUpdates()
        {
            if (checkedForUpdates) yield break;
            yield return new WaitForSecondsRealtime(1f);

            yield return CheckForUpdatesAsync();
        }
        private async Task CheckForUpdatesAsync()
        {
            Version currentVersion = new Version(modVersion);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "NeonSoundReplacer");
            HttpResponseMessage response = await client.GetAsync("https://api.github.com/repos/Tuchan/NeonSoundReplacer/releases/latest");
            if (!response.IsSuccessStatusCode)
            {
                LoggerInstance.Error("Failed to check for updates");
                LoggerInstance.Error($"Response: {response.StatusCode} {response.ReasonPhrase}");
            }
            String responseString = await response.Content.ReadAsStringAsync();
            int startIndex = responseString.IndexOf("tag_name") + 11;
            int endIndex = responseString.IndexOf("\"", startIndex);
            String latestVersionString = responseString.Substring(startIndex, endIndex - startIndex);
            Version latestVersion = new Version(latestVersionString);
            if (currentVersion < latestVersion)
            {
                String githubLink = "https://github.com/Tuchan/NeonSoundReplacer/releases/latest";
                String popupPrefix = "Main Menu/Canvas/Popup/Popup Window/Window Holder/Popup Scaler/Popup Content Holder/";
                LoggerInstance.Warning("A new update is avaliable!");
                LoggerInstance.Warning($"Current Version: {modVersion}");
                LoggerInstance.Warning($"Latest Version: {latestVersionString}");
                LoggerInstance.Warning("Download it here: " + githubLink);
                GameObject.Find("Main Menu/Canvas/Main Menu/Panel/Title Panel/Title Buttons/Quit Button").GetComponent<MenuButtonHolder>().ButtonRef.onClick.Invoke();
                GameObject.Find(popupPrefix + "Popup Text").GetComponent<TextMeshProUGUI>().text = "A new update is avaliable for NeonSoundReplacer, do you want to visit the download page?";
                Button yesButton = GameObject.Find(popupPrefix + "Popup Buttons/Button Yes/Button").GetComponent<Button>();
                Button.ButtonClickedEvent oldYesEvent = yesButton.onClick;
                yesButton.onClick = new Button.ButtonClickedEvent();
                yesButton.onClick.AddListener(() =>
                {
                    Application.OpenURL(githubLink);
                    yesButton.onClick = oldYesEvent;
                    GameObject.Find(popupPrefix + "Popup Buttons/Button No").GetComponent<MenuButtonHolder>().ButtonRef.onClick.Invoke();
                });
                Button noButton = GameObject.Find(popupPrefix + "Popup Buttons/Button No/Button").GetComponent<Button>();
                Button.ButtonClickedEvent oldNoEvent = noButton.onClick;
                noButton.onClick = new Button.ButtonClickedEvent();
                noButton.onClick.AddListener(() =>
                {
                    yesButton.onClick = oldYesEvent;
                    noButton.onClick = oldNoEvent;
                    GameObject.Find(popupPrefix + "Popup Buttons/Button No").GetComponent<MenuButtonHolder>().ButtonRef.onClick.Invoke();
                });
            }
            else
            {
                LoggerInstance.Msg("You're up to date! :D");
            }
            checkedForUpdates = true;
        }

        public static MelonPreferences_Category config;
        public static MelonPreferences_Entry<bool> toggleSounds;
        public static MelonPreferences_Entry<bool> shouldLogSounds;
        public static MelonPreferences_Entry<string> soundsToReplace;

        [Obsolete]
        public override void OnApplicationStart()
        {
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Mods", "NeonSoundReplacer"))) Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Mods", "NeonSoundReplacer"));
            config = MelonPreferences.CreateCategory("NeonSoundReplacer Settings");
            toggleSounds = config.CreateEntry("Toggle Sounds", true, description: "Toggles the sounds on and off.");
            shouldLogSounds = config.CreateEntry("Log Sounds", false, description: "Displays the current audio name in the console, so that you know which sound to replace.");
            soundsToReplace = config.CreateEntry("Replace Sounds Here", "MUSIC_STORY_TITLE = newsound.wav 2\nMUSIC_STORY_MAP = newmenusound.mp3", description: "To understand how to format this, either see an example below or read about it more on the github page.");
            RefreshCustomSounds();
        }
    }
}
