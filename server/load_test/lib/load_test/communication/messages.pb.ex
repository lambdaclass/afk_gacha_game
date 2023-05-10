defmodule LoadTest.Communication.Proto.Action do
  @moduledoc false

  use Protobuf, enum: true, protoc_gen_elixir_version: "0.12.0", syntax: :proto3

  field :ACTION_UNSPECIFIED, 0
  field :MOVE, 1
  field :ATTACK, 2
  field :PING, 3
  field :UPDATE_PING, 4
end

defmodule LoadTest.Communication.Proto.Direction do
  @moduledoc false

  use Protobuf, enum: true, protoc_gen_elixir_version: "0.12.0", syntax: :proto3

  field :DIRECTION_UNSPECIFIED, 0
  field :UP, 1
  field :DOWN, 2
  field :LEFT, 3
  field :RIGHT, 4
end

defmodule LoadTest.Communication.Proto.ClientAction do
  @moduledoc false

  use Protobuf, protoc_gen_elixir_version: "0.12.0", syntax: :proto3

  field :action, 1, type: LoadTest.Communication.Proto.Action, enum: true
  field :direction, 2, type: LoadTest.Communication.Proto.Direction, enum: true
  field :latency, 3, type: :uint32

  def transform_module(), do: LoadTest.Communication.ProtoTransform
end
