using Godot;
using System;

public class PlayerScript : RigidBody {
  private ARVRCamera camera;
  private ARVROrigin origin;
  private CollisionShape collider;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    camera = GetNode<ARVRCamera>("FPController/ARVRCamera");
    origin = GetNode<ARVROrigin>("FPController");
    collider = GetNode<CollisionShape>("Collider");
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta) {
    AlignCamera(camera, origin, this);
  }

  private static void AlignCamera(ARVRCamera camera, ARVROrigin origin, RigidBody player) {
    Vector3 offset = camera.GlobalTranslation - player.GlobalTranslation;
    origin.GlobalTranslation += offset.Inverse();
    camera.GlobalTranslation += offset;
  }
}
