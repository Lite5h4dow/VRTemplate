using Godot;
using System;
using Gameplay;

public class Player : VRPlayer {
  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    base._Ready();

  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta) {
    base._Process(delta);
    GD.Print();
  }
}
