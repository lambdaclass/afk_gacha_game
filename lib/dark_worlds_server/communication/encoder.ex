defmodule DarkWorldsServer.Communication.Encoder do
  @doc """
  Called when `use` is invoked
  """
  defmacro __using__(_) do
    quote do
      @derive Jason.Encoder
    end
  end
end

defimpl Jason.Encoder, for: Tuple do
  def encode(data, options) when is_tuple(data) do
    data
    |> Tuple.to_list()
    |> Jason.Encoder.List.encode(options)
  end
end
