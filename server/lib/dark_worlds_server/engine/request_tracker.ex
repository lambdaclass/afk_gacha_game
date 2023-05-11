defmodule DarkWorldsServer.Engine.RequestTracker do
  def create_table() do
    :ets.new(:request_tracker, [:set, :public, :named_table])
  end

  def add_counter(game_pid, player_id) when is_pid(game_pid) and is_integer(player_id) do
    key = {game_pid, player_id}
    :ets.update_counter(:request_tracker, key, 1, {key, 0})
  end

  def report() do
    aggregate =
      :ets.tab2list(:request_tracker)
      |> Enum.reduce(%{total: 0, msgs_per_game: %{}}, fn {{game_pid, player_id}, count}, acc ->
        acc
        |> update_in([:total], fn val -> val + count end)
        |> update_in([:msgs_per_game, game_pid], fn x ->
          val = x || %{total: 0, msgs_per_player: %{}}
          %{val | :total => val.total + count}
        end)
        |> put_in([:msgs_per_game, game_pid, :msgs_per_player, player_id], count)
      end)

    IO.puts("Report of request tracking")
    IO.puts("--------------------------")
    IO.puts("total msgs: #{aggregate.total}")
    IO.puts("total games: #{length(Map.keys(aggregate.msgs_per_game))}")
    IO.puts("\nDetails per game")
    IO.puts("------------------")

    Enum.each(aggregate.msgs_per_game, fn {game_pid,
                                           %{total: total, msgs_per_player: msgs_per_player}} ->
      IO.puts("#{:erlang.pid_to_list(game_pid)} =>")
      IO.puts("   total msgs: #{total}")
      IO.puts("   total players: #{length(Map.keys(msgs_per_player))}")
      IO.puts("   msgs per player =>")

      Enum.each(msgs_per_player, fn {player_id, value} ->
        IO.puts("       player #{player_id}, total msg: #{value}")
      end)
    end)
  end
end
