defmodule DarkWorldsServer.Communication do
  @doc """
  The Communication context
  """

  @spec encode!(term()) :: String.t() | no_return
  def encode!(value) do
    Jason.encode!(value)
  end

  @spec decode(iodata()) :: {:ok, term()} | {:error, Jason.DecodeError.t()}
  def decode(value) do
    Jason.decode(value)
  end
end
