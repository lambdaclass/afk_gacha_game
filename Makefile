.PHONY: generate-protos setup

generate-protos:
	protoc --csharp_out=./ gateway.proto
	mv Gateway.cs client/Assets/Scripts/Protobuf/Gateway.pb.cs

setup:
	dotnet tool install -g dotnet-format

format:
	dotnet-format -w --folder client/Assets/Scripts --verbosity n
