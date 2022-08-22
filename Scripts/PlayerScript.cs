using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class PlayerScript : RigidBody {
  private ARVRCamera camera;
  private ARVROrigin origin;
  private CollisionShape collider;
  private RayCast headCast;

  private ARVRController LeftHandController;
  private ARVRController RightHandController;

  private Spatial LeftTrackedHand;
  private Spatial RightTrackedHand;

  private Area LeftGrabArea;
  private Area RightGrabArea;


  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    // VR Gear
    camera = GetNode<ARVRCamera>("FPController/ARVRCamera");
    origin = GetNode<ARVROrigin>("FPController");
    collider = GetNode<CollisionShape>("BodyCollider");
    headCast = GetNode<RayCast>("HeadCast");

    // Hand Controllers
    LeftHandController = GetNode<ARVRController>("FPController/LeftHandController");
    RightHandController = GetNode<ARVRController>("FPController/RightHandController");

    // Tracked Hands
    LeftTrackedHand = GetNode<Spatial>("FPController/LeftHand");
    RightTrackedHand = GetNode<Spatial>("FPController/RightHand");

    // Grab zones
    LeftGrabArea = GetNode<Area>("FPController/LeftHandController/GrabArea");
    RightGrabArea = GetNode<Area>("FPController/RightHandController/GrabArea");

    // connect to Signals
    LeftHandController.Connect("activated", this, "EnableLeftHandVisibility");
    LeftHandController.Connect("deactivated", this, "DisableLeftHandVisibility");
    RightHandController.Connect("activated", this, "EnableRightHandVisibility");
    RightHandController.Connect("deactivated", this, "DisableRightHandVisibility");

    LeftHandController.Connect("button_pressed", this, "LeftButtonPressed");
    LeftHandController.Connect("button_release", this, "LeftButtonRelease");

    RightHandController.Connect("button_pressed", this, "RightButtonPressed");
    RightHandController.Connect("button_release", this, "RightButtonRelease");
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta) {
    // player rigid body alignment and collision alignment
    AlignCamera(camera, origin, this);
    AlignHeadCast(headCast, camera);
    AlignAndShapeCollision(collider, camera, headCast);
  }

  // Visibility signals
  public void EnableLeftHandVisibility() {
    LeftTrackedHand.Visible = true;
  }
  public void DisableLeftHandVisibility() {
    LeftTrackedHand.Visible = false;
  }
  public void EnableRightHandVisibility() {
    RightTrackedHand.Visible = true;
  }
  public void DisableRightHandVisibility() {
    RightTrackedHand.Visible = false;
  }

  // Button Signals
  public void LeftButtonPressed(int id) {
    GD.Print("Left Button Pressed: ", id);
    HandleInputPressedEvent(id, LeftHandController, LeftGrabArea);
  }

  public void LeftButtonRelease(int id) {
    GD.Print("Left Button Release: ", id);
    HandleInputReleaseEvent(id, LeftHandController, LeftGrabArea);
  }

  public void RightButtonPressed(int id) {
    GD.Print("Right Button Pressed: ", id);
    HandleInputPressedEvent(id, RightHandController, RightGrabArea);
  }

  public void RightButtonRelease(int id) {
    GD.Print("Right Button Release: ", id);
    HandleInputReleaseEvent(id, RightHandController, RightGrabArea);
  }

  // alligns the camera to the center of the player controller by offseting the playspace by the inverse amount the camera has moved. this keeps the camera and the player collision and controller in a predictable place relative to the game world 
  private static void AlignCamera(ARVRCamera camera, ARVROrigin origin, RigidBody player) {
    Vector3 offset = camera.GlobalTranslation - player.GlobalTranslation;
    player.Translation += offset;
    origin.Translation += -offset;
    camera.Translation += offset;
  }

  // alligns the raycast responsible for detecting how large the player collision should be based on their distance to the nearest object below them.
  private static void AlignHeadCast(RayCast headCast, ARVRCamera camera) {
    float offset = camera.GlobalTranslation.y - headCast.GlobalTranslation.y;
    headCast.Translation += new Vector3(0, offset, 0);
  }

  // morphs the player collision to their height and defaults to 2m if the player isnt in range of any object (e.g. flying)
  private static void AlignAndShapeCollision(CollisionShape collision, ARVRCamera camera, RayCast headCast) {
    float yRotOffset = camera.Rotation.y - collision.Rotation.y;
    collision.RotateY(yRotOffset);

    CapsuleShape shape = (CapsuleShape)collision.Shape;
    Vector3 collisionPoint = headCast.GetCollisionPoint();
    float distance = camera.GlobalTranslation.DistanceTo(collisionPoint);

    float trueHeight = distance - shape.Radius;
    shape.Height = trueHeight;

    collision.Translation = new Vector3(0, -(distance / 2) + (shape.Radius / 2), 0);
  }

  private static void HandleInputPressedEvent(int id, ARVRController controller, Area GrabArea) {
    switch (id) {
      case 7:
        // A button
        return;

      case 1:
        // B button
        return;

      case 2:
        // Grab
        return;

      case 15:
        // Trigger
        return;

      default:
        return;
    }
  }

  private static void HandleInputReleaseEvent(int id, ARVRController controller, Area GrabArea) {
    switch (id) {
      case 7:
        // A button
        return;

      case 1:
        // B button
        return;

      case 2:
        // Grab
        return;

      case 15:
        // Trigger
        return;

      default:
        return;
    }
  }
}
