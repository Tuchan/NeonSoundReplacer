# NeonSoundReplacer
Easily change any sound (music, sound effects, dialogues, and more) in Neon White!

Supports **MP3, WAV, OGG, AIFF** and more. (everything that unity supports)

> [!WARNING]  
> If you're a speedrunner, please read "Speedrunning".

![screenshot](https://github.com/Tuchan/NeonSoundReplacer/assets/43300571/a21e3cac-fde1-4bcd-8163-5290384469cb)

# Speedrunning
When it comes to speedrunning, it's **essential** for leaderboard moderators to be able to hear what you're doing in a game. For example, if they can't hear the water boost sound effect, they can suspect that you got it from nowhere, thus rejecting your record. In short, if you're planning on submitting a record to the leaderboard, please make sure that **NO** gameplay sound effects (demon noises, gun sounds, water boost noise, wall-breaking noise, etc.) are being changed. You can check that by dragging the sound effects volume slider around. Game music, voices, level finish jingles, and more, are fine to have on video. Also, remember that someone will have to watch the video you're sending, so don't put weird NSFW or turbo bass-boosted sounds in there. 

**And... Sound Pack creators, please mention that the pack isn't suitable for speedrunning if that's the case... please and thank you.**

> [!IMPORTANT]  
> To change RP properties you have to have MelonPreferencesManager installed.

# FAQ
<details>
<summary>How to change sounds?</summary>
  
The whole system is designed to be very easy to work with. **If you're planning to change sounds, you have to turn on the melonloader console before launching the game.** The console will display sounds that are currently played. For the sake of this tutorial, let's say we want to change the __katana slash sound effect__. How would we do that?

1. Go to the `Mods/NeonSoundReplacer` folder and put a sound effect that you want to use later (switch the original one with).
2. Open the MelonPreferencesManager window, go into `Neon Sound Replacer Settings`, select `Log Sounds`, and remember to click the `Save Preferences` button at the top.
3. Go into the game and trigger that sound effect (in that case the katana slash)
4. Go into the console and look for the proper sound effect, you might want to trigger it a few times to check if that's the correct one. (mostly trial and error if you can't find it)
5. The sound effects triggered by the game will look like this:

![originalaudio](https://github.com/Tuchan/NeonSoundReplacer/assets/43300571/2efaffe4-5918-4958-8b29-c7295b3a9875)

You now know that the sound effect in the game files is called `WEAPON_KATANA_FIRE` :D

6. Now go into `Neon Sound Replacer Settings` and format the line like this `WEAPON_KATANA_FIRE = soundeffect.mp3`. Remember to save preferences!
7. Close the window and trigger that sound again. If you hear a different sound effect, congrats :D If not, look in the console for potential errors. 
</details>
<details>
<summary>How to create sound packs?</summary>

If you finished changing the sounds, create a .txt file and paste all of the lines from `Replace Sounds Here`, and put it in a zip with all of the sound effects that you use. If you want to see a more practical example then download the [OldDemoSoundPack in the featured section.](https://github.com/Tuchan/NeonSoundReplacer#featured-soundpacks)
</details>
<details>
<summary>How to install sound packs?</summary>

Unzip the file and paste all lines that the pack creator provided in the `Replace Sounds Here` box. Grab all sound files from the zip and put them in `Mods/NeonSoundReplacer`.
</details>
<details>
  <summary>There is a delay</summary>

  If you notice a slight delay, edit the sound effect and remove the silence at the start. If that didn't help than try exporting files to `.ogg` instead of `.mp3` or `.wav`. If you already did that then I can't help you more, unless I find a fix. The issue could be caused by your hard drive being slow, game lagging, etc. Since we're reading a file from disc every time the sound effect is played, it will be slower than loading it all at once (AssetBundles). I personally haven't had an issue even on my slower hard drive, so if you encounter that issue, either create a [New Issue](https://github.com/Tuchan/NeonSoundReplacer/issues/new) or DM me on discord @tuchan.
</details>

# How to install
1. Go to [releases](https://github.com/Tuchan/NeonSoundReplacer/releases/latest) and download the `NeonSoundReplacer.dll` and put it in your Mods folder

> [!NOTE]  
> If there is an update, you will see a pop-up in game.

# Featured Soundpacks
### OldDemoSoundPack 
Features music and level jingle found in the Neon White Demo.

[<img alt="download" width="100px" src="https://mtctutorials.com/wp-content/uploads/2019/04/Download-button-png-GREEN-color-by-mtc-tutorials.png"/>](https://cdn.discordapp.com/attachments/1058192110291009596/1145006681655353404/OldDemoSoundPack.zip)



# Other things
If you don't plan on changing sound effects in that session and you want to launch your game faster, add `--melonloader.hideconsole` to launch properties (right-click the game on steam -> properties -> launch options at the bottom of that window) to hide the console that melonloader loads in.

# Other Neon White Projects

[<img alt="NeonWhiteRP" width="500px" src="https://socialify.git.ci/Tuchan/NeonWhiteRP/image?description=1&font=Inter&name=1&owner=1&pattern=Plus&theme=Dark"/>](https://github.com/Tuchan/NeonWhiteRP)
