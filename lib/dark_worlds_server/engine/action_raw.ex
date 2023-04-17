defmodule DarkWorldsServer.Engine.ActionRaw do
  @enforce_keys [:player, :action, :value]
  @derive Jason.Encoder
  defstruct [:player, :action, :value]

  def from_json(json) do
    case Jason.decode(json) do
      {:ok, data} ->
        {:ok,
         %__MODULE__{
           player: data["player"] |> encode_player(),
           action: data["action"] |> encode_action(),
           value: data["value"] |> encode_value()
         }}

      {:error, error} ->
        {:error, error}
    end
  end

  def encode_player(player) when is_integer(player), do: {:ok, player}
  def encode_player(_other), do: {:error, :invalid}

  def encode_action("move"), do: {:ok, :move}
  def encode_action("attack"), do: {:ok, :attack}
  def encode_action(_other), do: {:error, :invalid}

  def encode_value("up"), do: {:ok, :up}
  def encode_value("down"), do: {:ok, :down}
  def encode_value("left"), do: {:ok, :left}
  def encode_value("right"), do: {:ok, :right}
  def encode_value(_other), do: {:error, :invalid}
end
