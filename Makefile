.PHONY: docs gen-server-protobuf gen-client-protobuf

docs:
	cd docs && mdbook serve --open

gen-server-protobuf:
	protoc \
		--elixir_out=transform_module=DarkWorldsServer.Communication.ProtoTransform:./server/lib/dark_worlds_server/communication/ \
		--elixir_opt=package_prefix=dark_worlds_server.communication.proto \
		messages.proto

gen-client-protobuf:
	protogen --csharp_out=./  messages.proto
	mv messages.cs client/Assets/Scripts/Messages.pb.cs
