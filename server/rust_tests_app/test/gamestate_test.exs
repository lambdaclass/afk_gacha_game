defmodule GameStateTesting do
  use ExUnit.Case

  @tag :skip
  test "No move if beyond boundaries" do
    assert {:ok, ""} = TestNIFs.no_move_if_beyond_boundaries()
  end

  @tag :skip
  test "No move if occupied" do
    assert {:ok, ""} = TestNIFs.no_move_if_occupied()
  end

  @tag :skip
  test "No move if wall" do
    assert {:ok, ""} = TestNIFs.no_move_if_wall()
  end

  @tag :skip
  test "Attacking" do
    assert {:ok, ""} = TestNIFs.attacking()
  end

  @tag :skip
  test "Movement" do
    assert {:ok, ""} = TestNIFs.movement()
  end
end
