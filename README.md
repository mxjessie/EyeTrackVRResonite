# EyeTrackVRResonite

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/) that enables the use of [Babble Face Tracking](https://github.com/SummerSigh/ProjectBabble).

## Usage
1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
2. Place [EyeTrackVRResonite.dll](https://github.com/Meister1593/EyeTrackVRResonite/releases) into your `res_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\res_mods` on windows or `$HOME/.steam/steam/steamapps/common/Resonite/res_mods` on linux for a default installation. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create the folder for you.
3. Place [Rug.Osc.dll](https://github.com/Meister1593/EyeTrackVRResonite/releases) into your Resonite base folder, one above your 'res_mods' folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite` on windows or `$HOME/.steam/steam/steamapps/common/Resonite` on linux for a default installation.
4. Start the game. If you want to verify that the mod is working you can check your Resonite logs or create an EmptyObject with an AvatarRawEyeData/AvatarRawMouthData Component (Found under Users -> Common Avatar System -> Face -> AvatarRawEyeData/AvatarRawMouthData).

# Libraries used:
- [Rug-OSC](https://bitbucket.org/rugcode/rug.osc/) - MIT License
