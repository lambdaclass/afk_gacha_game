defmodule DarkWorldsServer.Engine.ActionOk do
  alias DarkWorldsServer.Engine.ActionRaw

  @enforce_keys [:player, :action, :value]
  @derive Jason.Encoder
  defstruct [:player, :action, :value]

  def from_action_raw(%ActionRaw{
        player: {:ok, player},
        action: {:ok, action},
        value: {:ok, value}
      }) do
    {:ok, %__MODULE__{player: player, action: action, value: value}}
  end

  def from_action_raw(_) do
    {:error, "Invalid message"}
  end
end
