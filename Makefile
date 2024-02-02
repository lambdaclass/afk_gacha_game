.PHONY: generate-protos

generate-protos: 
    protoc --csharp_out=./ messages.proto \
    mv Messages.cs client/Assets/Scripts/Messages.pb.cs
