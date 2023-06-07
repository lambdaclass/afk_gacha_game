# LoadTest

```bash
cd server/load_test
mix deps.get
export SERVER_HOST=10.150.20.186:4000
iex -S mix
```

Inside the Elixir shell

```
LoadTest.PlayerSupervisor.spawn_50_sessions()
```

to create 50 games with 3 players sending random commands every 30 ms.

If you plan on creating more than 50 sessions, first increase the file descriptor limit of your shell by doing

```bash
ulimit -n 65535
before running iex -S mix
```
