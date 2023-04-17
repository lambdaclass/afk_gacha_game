.PHONY: setup run elixir-tests rust-tests

setup:
	mix deps.get
	mix deps.compile

run:
	mix assets.build
	iex -S mix phx.server

tests: elixir-tests rust-tests

elixir-tests:
	mix test

rust-tests:
	cargo test --manifest-path native/gamestate/Cargo.toml
