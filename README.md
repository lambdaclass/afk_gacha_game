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

Then run the server with

```
make run
```

This will setup a server listening through a websocket on port `4000`, on the `/play` path. This server handles the game state, consisting of a number of players scattered on a grid. By default there are two players on a `5x5` grid.

Clients can move players by sending `JSON` messages through the websocket. To try it locally, you can use [websocat](https://github.com/vi/websocat) (or something like [Postman](https://www.postman.com/)) and then issue move commands to the server by connecting

```
websocat ws://127.0.0.1:4000/play
```

and sending messages like this:

```
{"player": 1, "action": "move", "value": "down"}
{"player": 1, "action": "move", "value": "up"}
{"player": 1, "action": "move", "value": "left"}
{"player": 1, "action": "move", "value": "right"}
```

which will move player `1` down, then up, left and right by one cell each.

You should be able to see the updated state grid on the server logs after every command.
