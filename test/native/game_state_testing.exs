defmodule GameStateTesting do
  use ExUnit.Case

  test "No move if beyond boundaries" do
    RustlerTests.no_move_if_beyond_boundaries()
  end

  test "No move if occupied" do
    RustlerTests.no_move_if_occupied()
  end

  test "Attacking" do
    RustlerTests.attacking()
  end

  test "No move if wall" do
    RustlerTests.no_move_if_wall()
  end

  test "Movement" do
    RustlerTests.movement()
  end
end
