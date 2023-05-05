# Dark Worlds Server

Elixir + Rust backend for Dark Worlds.

## Requirements

- [Elixir](https://elixir-lang.org/)
- [Rust](https://www.rust-lang.org/tools/install)

## Running locally

Install dependencies and compile the project with

```
make setup
```

Start the Docker database with

```
make db
```

Then run the server with

```
make run
```
This will setup a server listening through a websocket on port `4000`, on the `/matchmaking` path. This server handles the game state, consisting of a number of players scattered on a grid. By default there are three players on a `100x100` grid.

Clients can move players by sending `JSON` messages through the websocket. To try it locally, you can use [websocat](https://github.com/vi/websocat) (or something like [Postman](https://www.postman.com/)) and then issue move commands to the server by connecting to

```
websocat ws://127.0.0.1:4000/play/[GAME-SESSION]
```
where `[GAME-SESSION]` is the unique ID that can be found on the screen and at the end of the URL after starting a new game.

Once connected, instructions can be sent via messages like this:

```
{"action": "move", "value": "down"}
{"action": "move", "value": "up"}
{"action": "move", "value": "left"}
{"action": "move", "value": "right"}
```

which will move the active player down, then up, left and right by one cell each. While this command:

```
{"action": "attack", "value": "down"}
```

will issue an attack to the player below. Attacks have a 1-second cooldown.

You should be able to see the updated state grid on the server logs after every command.

## Tests

Run tests with

```
make tests
```

## Documentation

To serve the documentation locally, first install both [mdbook](https://rust-lang.github.io/mdBook/guide/installation.html) and [mdbook-mermaid](https://github.com/badboy/mdbook-mermaid#installation), then run

```
make docs
```
