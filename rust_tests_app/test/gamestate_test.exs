defmodule GameStateTesting do
  use ExUnit.Case

  test "No move if beyond boundaries" do
    TestNIFs.no_move_if_beyond_boundaries()
  end

  test "No move if occupied" do
    TestNIFs.no_move_if_occupied()
  end

  test "Attacking" do
    TestNIFs.attacking()
  end

  test "No move if wall" do
    TestNIFs.no_move_if_wall()
  end

  test "Movement" do
    TestNIFs.movement()
  end
end
