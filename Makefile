.PHONY: generate-protos

generate-protos:
	protoc --csharp_out=./ gateway.proto
	mv Gateway.cs client/Assets/Scripts/Protobuf/Gateway.pb.cs
