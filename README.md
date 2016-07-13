# Zumo

## Setup

Each `.unity` scene file in the project contains part of the game; different
combinations of scenes are loaded at runtime for different menus, game modes,
etc. To edit scenes, it's easiest to open both `Game.unity` and the scene you
want to edit - that way the background lighting and room objects appear but
objects from other scenes don't.

Before running the game, all scenes except `Game.unity` need to be closed. To
automate this, use the `Set Master Scene File` option in the
`Assets -> Scene Autoload` menu and select `Game.unity` in the file dialog.
This will save the scene configuration when you enter play mode and restore
it when you stop play. To disable the feature, select `Assets -> Scene Autoload ->
Don't Load Master On Play`.
