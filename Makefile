.PHONY: setup run tests

setup:
	mix deps.get
	mix deps.compile

run:
	iex -S mix phx.server

tests:
	mix test
