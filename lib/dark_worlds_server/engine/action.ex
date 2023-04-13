defmodule DarkWorldsServer.Engine.Action do
  @enforce_keys [:player, :action, :value]
  @derive Jason.Encoder
  defstruct [:player, :action, :value]

  def from_json(json) do
    case Jason.decode(json) do
      {:ok, data} ->
        {:ok,
         %__MODULE__{
           player: data["player"],
           action: data["action"],
           value: data["value"]
         }}

      {:error, error} ->
        {:error, error}
    end
  end
end
