TopDown Engine
v3.2

The TopDown Engine is a complete Unity 2D and 3D top down game engine.
Basically it'll allow you to create your own top down game.
It comes complete with lots of visual assets, animations, particle effects, etc...

I HAVE A LOT OF QUESTIONS!
--------------------------

Have you read the FAQ at https://topdown-engine.moremountains.com/topdown-engine-contact#faq ?
You should read it.


WHAT'S IN THE TOPDOWN ENGINE?
-----------------------------

The TopDown Engine contains quite a lot of stuff.
Everything is in the Assets folder, and in it you'll find the following folders :

- /Common : a bunch of stuff (scenes, sprites, animations...) that don't belong to any particular demo, but may be used by any demo. Stuff like the startscreen, loading screen, etc, will go in there. /Common also contains the main /Scripts folder, which is the actual engine : all the scripts that make it work, grouped by theme. This includes camera scripts, character controllers, enemy AI, health management, moving platforms, weapons, level management, etc... It's all sorted for you in the right folders.
- /Demos : grouped by demo types (koala, minimal 2d, loft, minimal 3d...), it contains everything from sprite to prefabs that make these demo work
- /ThirdParty : helpers and libraries

In the demos/common folders you'll usually find the following structure :
- Animations : contains all the animations in the game
- Fonts : the fonts used in the GUI
- Materials : all the materials and associated sprites used in the particle effects
- Resources : all the game's prefabs. You just have to drag one of these in your scene and they'll be ready
- Scenes : a start screen and some demo levels (from the start screen press A on an xbox controller, or space on your keyboard)
- Scripts : specific scripts
- Sprites : all the sprites and spritesheets used in the game. Feel free to reuse them in your own game.

WHAT AM I SUPPOSED TO DO WITH IT?
---------------------------------

The TopDown Engine includes everything you need to start your own top down action game.
You can either start fresh and pick the scripts you need from it, or modify the existing demo levels brick by brick, adding your own sprites and scripts as you go.

DOCUMENTATION
-------------

A complete documentation is available for this asset, go to https://topdown-engine-docs.moremountains.com/API/ for a complete API documentation, or https://topdown-engine-docs.moremountains.com/ for a functional one

2D EXTRAS
---------

Some of the demos in the engine use tilemaps and rely on Unity's 2D extras. If you want to enjoy these demos, make sure you install them.
You can learn more about how to do that at https://topdown-engine-docs.moremountains.com/install.html#tilemaps-and-2d-extras
