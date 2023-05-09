# Message Protocol

The current message protocol is [Protobuf](https://protobuf.dev/programming-guides/proto3/).

You can find the file defining the spec in the root of the project [messages.proto](../../messages.proto)

## Generating the files from .proto

When using Protobuf the common thing to do is use generators that take you `.proto` files and generated the code in your language of choice.

Since we have 2 distinct langauages (Elixir, C#) below you will find the sections for each one

### Elixir

For Elixir we use the library [protobuf](https://github.com/elixir-protobuf/protobuf)

#### Requirements:

- [Protobuf compiler](https://github.com/protocolbuffers/protobuf#protocol-compiler-installation) (`protoc`), you can check the [releases](https://github.com/protocolbuffers/protobuf/releases) for a pre-built binary or in macOS do `brew install protobuf`
- `protoc-gen-elixir` plugin for `protoc`. Install with `mix escript.install hex protobuf` and make sure to include in your `PATH`, if you are using asdf run `asdf reshim`

#### Generating code:

After much trial and error, we distilled the code generation into a simple command you can use from the root of the project

```
make gen-server-protobuf
```

This will generate 2 files

- `messages.pb.ex`, which is the module specifying the structs (and other things) for our protobuf messages
- `proto_transform.ex`, this module is referenced by `messages.pb.ex` in the `transform_module/0` callback. This module handles the transformation from our data types (structs, maps, etc) into the generated structs of the protobuf messages (and viceversa)

### CSharp

For C# we use the library [protobuf-net](https://github.com/protobuf-net/protobuf-net)

#### Requirements:

- [Protobuf compiler](https://github.com/protocolbuffers/protobuf#protocol-compiler-installation) (`protoc`), you can check the [releases](https://github.com/protocolbuffers/protobuf/releases) for a pre-built binary or in macOS do `brew install protobuf`
- [.NET6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) runtime needed by `protogen` 3.2.12
- [protogen](https://www.nuget.org/packages/protobuf-net.Protogen) tool to generate the code, similar to `protoc`

#### Generating code:

After much trial and error, we distilled the code generation into a simple command you can use from the root of the project

```
make gen-client-protobuf
```
