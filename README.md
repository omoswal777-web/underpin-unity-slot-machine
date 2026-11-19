# Lucky Reels — Unity Slot Machine

A small 3-reel slot machine prototype created for the Underpin Technology Unity Developer Intern assignment. The project uses the artwork supplied with the assignment.

## Game overview

- Three independently animated reels.
- Four supplied symbols: Seven, Cherries, Bell and BAR.
- A win occurs only when all three reels stop on the same symbol.
- Credits, adjustable bet and simulated payouts.
- Reels slow down before stopping and use a small stop bounce.
- The result is selected before the animation begins, separating game rules from reel presentation.

## Payout table

| Matching symbol | Payout |
|---|---:|
| Seven | 50 × bet |
| Cherries | 15 × bet |
| Bell | 10 × bet |
| BAR | 5 × bet |

The bet is deducted when a spin begins. The payout is added to the balance only after a three-symbol match.

## Requirements

- Unity 2022.3 LTS or a compatible newer Unity version.
- Unity UI (uGUI), included with Unity.
- WebGL Build Support module for the installed Unity version.

## Open and run

1. Extract the project ZIP.
2. Open Unity Hub and choose **Add > Add project from disk**.
3. Select the `Underpin_Slot_Machine` folder.
4. Open the project in Unity.
5. In the Unity menu, choose **Underpin > Create Slot Machine Demo Scene**.
6. Open `Assets/Scenes/SlotMachine.unity` if it is not opened automatically.
7. Press Play.

The scene builder configures the supplied PNG artwork as sprites and creates the UI scene. It is included so the scene can be regenerated consistently.

## Build the WebGL version

1. In Unity Hub, install **WebGL Build Support** for the Unity Editor version used by the project.
2. Open the project and generate the scene using **Underpin > Create Slot Machine Demo Scene**.
3. Choose **File > Build Settings** (or **Build Profiles** in newer Unity versions).
4. Select **WebGL** and switch platform if required.
5. In Player Settings, set a suitable company/product name and use a WebGL template.
6. Use **Underpin > Build WebGL**. The helper writes the build to `Build/WebGL`.

To test the build, serve the `Build/WebGL` directory using a local HTTP server. WebGL builds generally should not be opened directly from the file system. For example, from the project root:

```bash
python -m http.server 8000
```

Then open the local server URL and navigate to the WebGL build folder. For public hosting, GitHub Pages or another static host can be used after confirming the build loads correctly.

## Bonus / polish

- Bet adjustment controls.
- Per-symbol payout multipliers.
- Stop bounce animation.
- Clear win/loss feedback.
- Disabled controls while the reels are spinning.

## Technical approach

`SlotGameManager` owns the balance, bet, RNG outcome selection, win condition and payout. `SlotReel` owns the visual animation for one reel. The manager selects all final outcomes before starting the animations; this prevents animation timing from changing the game result. Unity's `Random.Range` is used for the prototype's randomized outcomes. This is a casual simulation and is not intended for real-money gambling or cryptographic fairness.

## Project structure

```text
Assets/
  Art/          Supplied assignment artwork
  Scripts/      SlotGameManager and SlotReel
  Editor/       Scene generator and WebGL build helper
  Scenes/       Generated gameplay scene
  Prefabs/
  Animations/
  UI/
  Sounds/
Build/
  WebGL/        Generated locally through Unity; not included until built
```

## Notes

The source project and build helper are included. A compiled WebGL player is not bundled in this starter package because Unity's editor/build pipeline must run locally with the required WebGL module installed. Generate and test the WebGL build before submitting the repository.
