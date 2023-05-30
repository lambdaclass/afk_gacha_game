defmodule Mix.Tasks.RustTests do
  use Mix.Task

  @shortdoc "Runs the tests for Rust"
  def run(args) do
    rust_path = Path.expand("../../rust_tests_app", __DIR__)

    Mix.Task.run("test", [rust_path])
  end
end
