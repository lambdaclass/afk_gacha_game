defmodule DarkWorldsServer.Engine.Board do
  @enforce_keys [:grid]
  @derive Jason.Encoder
  defstruct [:grid]

  defmodule TupleEncoder do
    alias Jason.Encoder

    defimpl Encoder, for: Tuple do
      def encode(data, options) when is_tuple(data) do
        data
        |> Tuple.to_list()
        |> Encoder.List.encode(options)
      end
    end
  end
end
