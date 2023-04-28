defmodule DarkWorldsServer.Communication do
  @doc """
  The Communication context
  """

  def encode!(value) do
    Jason.encode!(value)
  end

  def decode(value) do
    Jason.decode(value)
  end

  def pid_to_external_id(pid) when is_pid(pid) do
    pid |> :erlang.term_to_binary() |> Base58.encode()
  end

  def external_id_to_pid(external_id) do
    external_id |> Base58.decode() |> :erlang.binary_to_term([:safe])
  end
end
