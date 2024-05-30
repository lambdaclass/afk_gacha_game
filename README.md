# AFK Gacha Game

<img src="docs/AFKGachaBattle.PNG" alt="AFKGachaBattle">

## Table of Contents

- [AFK Gacha Game](#afk-gacha-game)
  - [Table of Contents](#table-of-contents)
  - [About](#about)
  - [Licensing](#licensing)
  - [Requirements](#requirements)
    - [Unity Setup](#unity-setup)
  - [Suggested Development Environment](#suggested-development-environment)
  - [Useful Commands](#useful-commands)
  - [Contact and Socials](#contact-and-socials)
  - [Technical Documentation](#documentation)

## About

Welcome to the realm of AFK Gacha Game, crafted by LambdaClass.

AFK Gacha Game is the second game that utilizes our groundbreaking [Game Backend](https://github.com/lambdaclass/game_backend). This open-source backend, meticulously developed by Lambda, ensures seamless and reliable gameplay.

Step into a universe where you, the player, get to turn the tides on an all-out war between the forces of good and evil. Side together with Mirra, the capricious deity known for manipulating entire societies by exploiting their deepest desires, or choose to rebel against its advances instead.

AFK Gacha Game is more than a game; it's an adventure into a world where strategy, skill, and a dash of chaos converge. Join the battle and confront the challenges that lie ahead in this captivating and dynamic gaming experience. The stage is set, and the Champions of Kaline await your commands—embrace the challenge and become a legend!



## Licensing

The code is licensed under the Apache 2 license, while the music and graphics are licensed under a CC attribution and share-alike license.

## Requirements

- **Backend:**
Make sure you have Mirra Backend installed and running. This can be found here: https://github.com/lambdaclass/mirra_backend

- **Flaticon:**
	- <a href="https://www.flaticon.com/free-icons/rpg" title="rpg icons">Rpg icons created by WR Graphic Garage - Flaticon</a>
	- <a href="https://www.flaticon.com/free-icons/confuse" title="confuse icons">Confuse icons created by Freepik - Flaticon</a>
	- <a href="https://www.flaticon.com/free-icons/fire" title="fire icons">Fire icons created by WR Graphic Garage - Flaticon</a>
	- <a href="https://www.flaticon.com/free-icons/nerf" title="nerf icons">Nerf icons created by WR Graphic Garage - Flaticon</a>
	- <a href="https://www.flaticon.com/free-icons/magic-potion" title="magic potion icons">Magic potion icons created by WR Graphic Garage - Flaticon</a>
	- <a href="https://www.flaticon.com/free-icons/snowflake" title="snowflake icons">Snowflake icons created by Lumi - Flaticon</a>
	- <a href="https://www.flaticon.com/free-icons/armor" title="armor icons">Armor icons created by max.icons - Flaticon</a>
  - <a href="https://www.flaticon.com/free-icons/electric-shock" title="electric shock icons">Electric shock icons created by Prosymbols Premium - Flaticon</a>
  - <a href="https://www.flaticon.com/free-icons/blueprint" title="blueprint icons">Blueprint icons created by Freepik - Flaticon</a>

- **Unity:**
  - [Download Unity](https://unity.com/unity-hub)
  - Download the following Unity Store assets and add them to `client/Assets/ThirdParty`
    - [RPG & MMO UI 6](https://assetstore.unity.com/packages/2d/gui/rpg-mmo-ui-6-99450)
    - [Map Maker](https://assetstore.unity.com/packages/2d/environments/map-maker-249063)
    - [RPG inventory icons](https://assetstore.unity.com/packages/2d/gui/icons/rpg-inventory-icons-56687#version-current)
    - [Basic RPG Icons](https://assetstore.unity.com/packages/2d/gui/icons/basic-rpg-icons-181301)
    - [Resource Vector Graphics](https://assetstore.unity.com/packages/2d/gui/icons/resource-icons-101998)
    - [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)
    - [Gold Mining Game](https://assetstore.unity.com/packages/2d/gui/gold-mining-game-2d-mine-ui-tilset-263856)
    - [UX Flat Icons](https://assetstore.unity.com/packages/2d/gui/icons/ux-flat-icons-free-202525)
    - Import the necessary TextMeshPro assets by going to:
      - "Window -> TextMeshPro -> Import TMP Essential Resources" and
      - "Window -> TextMeshPro -> Import TMP Examples and Extras"

### Unity Setup

- In Unity Hub, click on the add project button and select the `client` folder.
- To test the game, select the scene in `Assets/Scenes/Overworld` and run it by clicking the play button.

## Suggested Development Environment

Set up your environment with the following steps:

- Download the [.NET SDK](https://dotnet.microsoft.com/es-es/download/dotnet/thank-you/sdk-7.0.403-macos-arm64-installer) for your operating system.
- In VSCode, download the .NET extension. After installation, change the version to 1.26 (locate the version dropdown next to the Uninstall button in the extension window).
- To check if the setup is successful, go to VSCode's console, select the Output tab, and pick Omnisharp Log in the dropdown. If there are no error logs and Omnisharp is scanning the project, the configuration is correct.
- Run `make setup` to install the necessary dependencies to format and lint the code.
- Add dotnet-format to your PATH by running the following command in your terminal:
```bash
cat << \EOF >> ~/.zprofile
# Add .NET Core SDK tools
export PATH="$PATH:/Users/$USER/.dotnet/tools"
EOF
```

## Contact and Socials

If you have any questions, feedback, or comments:

- **Email:** gamedev@lambdaclass.com

We share our development and creative process in the open, follow us for frequent updates on our game:

- **Twitter:** [@CurseOfMirra](https://twitter.com/curseofmirra)
- **Reddit:** [r/curseofmirra](https://www.reddit.com/r/curseofmirra/)
- **Discord:** [join link](https://discord.gg/hxDRsbCpzC)
- **Telegram:** [t.me/curseofmirra](https://t.me/curseofmirra)

## Documentation
### Client Paths

The following diagram outlines the initialization process of a connection between the client and the backend, as well as the retrieval of the user's data to be displayed on the header.

On the start of the application, the SocketConnection, which is a game object in the scene, tries to establish a websocket connection with the backend. Upon successful establishment of the connection, the SocketConnection requests the current user data from the backend (note: the register/login logic is not shown in this diagram).

In parallel, the HeaderManager waits for the user information to be retrieved via the GlobalUserData, which is the class that contains the user information.

Once the user data is retrieved from the backend, it is sent to the HeaderManager, where the relevant information is displayed in the UI. Additionally, the HeaderManager subscribes to updates from the GlobalUserData, ensuring that any changes are promptly reflected in the UI. 

<img src="docs/Header Sequence Diagram.jpg" alt="Header Sequence Diagram">

___

The next diagram depicts the actions that take place when a user enters the Summon scene.

When entering the Summon scene, the SummonManager script is responsible of fetching the summon boxes from the backend via the SocketConnection. When these are returned, the manager instantiates a UI element for each box.

The user can then buy one of these boxes, triggering a chain communication that traverses all the way to the backend, passing through currency availability checks first. Once the backend processes the user's request, it returns the summoned unit so the summon manager can display it in the UI.

<img src="docs/Summon Sequence Diagram.jpg" alt="Summon Sequence Diagram">
