# Champions of Mirra

## Table of Contents

- [About](#about)
- [Licensing](#licensing)
- [Requirements](#requirements)
- [Suggested Development Environment](#suggested-development-environment)
- [Project Setup](#project-setup)
- [Unity Setup](#unity-setup)
- [Running the Backend](#running-the-backend)
- [Useful Commands](#useful-commands)
- [Documentation](#documentation)

## About

Welcome to Champions of Mirra, a mobile autobattler by LambdaClass.

The game runs on our [Game Backend](https://github.com/lambdaclass/game_backend). This open source backend, meticulously developed by Lambda, ensures seamless and reliable gameplay.

## Licensing

The code is licensed under the Apache 2 license, while the music and graphics are licensed under a CC attribution and share-alike license.

## Requirements

Ensure you have the following dependencies installed:

- **Docker and Docker Compose**
  - [Install Docker](https://docs.docker.com/get-docker/)
  - [Get Docker Compose](https://docs.docker.com/compose/install/)
- **Rust:**
  - [Install Rust](https://www.rust-lang.org/tools/install)
- **Elixir and Erlang:**
  - [Install Elixir using ASDF](https://thinkingelixir.com/install-elixir-using-asdf/)
  - Erlang/OTP 26
  - Elixir 1.15.4
- **Unity:**
  - [Download Unity](https://unity.com/unity-hub)
- **Lambda Game Backend** 
  - [Clone the repository](https://github.com/lambdaclass/game_backend)

## Suggested Development Environment

Set up your environment with the following steps:

- Download the [.NET SDK](https://dotnet.microsoft.com/es-es/download/dotnet/thank-you/sdk-7.0.403-macos-arm64-installer) for your operating system.
- In VSCode, download the .NET extension. After installation, change the version to 1.26 (locate the version dropdown next to the Uninstall button in the extension window).
- To check if the setup is successful, go to VSCode's console, select the Output tab, and pick Omnisharp Log in the dropdown. If there are no error logs and Omnisharp is scanning the project, the configuration is correct.

## Project Setup

Ensure Docker is running and execute the following commands:

```bash
git clone https://github.com/lambdaclass/curse_of_myrra
cd curse_of_myrra/server
make db
make setup
```

## Unity Setup

- In Unity Hub, click on the add project button and select the `client` folder.
- Choose the correct editor version and download the [Top Down Engine](https://assetstore.unity.com/packages/templates/systems/topdown-engine-89636) by [More Mountains](https://moremountains.com). Include it in the `Assets/ThirdParty` folder after purchasing the license.

## Running the Backend

Ensure Docker is running and execute:

```bash
make start
```

For local testing using the [game backend](https://github.com/lambdaclass/game_backend), temporarily edit the `mix.exs` file to point to your _local_ copy of the game backend, for example:
`{:game_backend, path: "/Users/MyUsername/lambda/game_backend"}`

For testing using a remote server, point to the _GitHub URL_ instead and specify the desired branch like so:
`{:game_backend, git: "https://github.com/lambdaclass/game_backend", branch: "main"}`

## Useful Commands

```bash
make tests
```

Will run Elixir and Rust tests

```bash
make format
```

Will format Elixir and Rust code.

```bash
make prepush
```

Will format your code and run Credo check and tests.

## Documentation

## Contact and Socials

If you have any questions, feedback, or comments:

- **Email:** gamedev@lambdaclass.com