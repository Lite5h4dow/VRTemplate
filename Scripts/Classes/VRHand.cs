#nullable enable

using System.Linq;
using System.Collections.Generic;

using Godot;

namespace Gameplay {
  public class VRHand : Node {

    private float GrabThreshold;
    public ARVRController Controller { get; private set; }

    public Spatial TrackedHand { get; private set; }

    public Area GrabArea { get; private set; }

    public Spatial MountPoint { get; private set; }

    public VRGrabbable? HeldBody { get; set; }

    public bool AButtonPressed { get; private set; }
    public bool AButtonTouched { get; private set; }

    public bool BButtonPressed { get; private set; }
    public bool BButtonTouched { get; private set; }

    public bool GripPressed { get; private set; }
    public float GripAxis { get; private set; }

    public bool ThumbStickPressed { get; private set; }
    public bool ThumbStickTouched { get; private set; }

    public bool ThumbPadPressed { get; private set; }
    public bool ThumbPadTouched { get; private set; }

    public float ThumbStickAxisX { get; private set; }
    public float ThumbStickAxisY { get; private set; }

    public bool TriggerPressed { get; private set; }
    public bool TriggerTouched { get; private set; }
    public float TriggerAxis { get; private set; }

    public VRGrabbable? holding { get; private set; }

    private Vector3 LastPosition;
    public Vector3 LinearVelocity { get; private set; }

    private Basis LastBasis;
    public Vector3 AngularVelocity { get; private set; }

    public VRHand(ARVRController controller, Spatial trackedHand, Area grabArea, Spatial mountPoint, float grabThreshold) {
      // set hand controllers
      Controller = controller;
      TrackedHand = trackedHand;
      GrabArea = grabArea;
      MountPoint = mountPoint;
      GrabThreshold = grabThreshold;

      LastBasis = Controller.GlobalTransform.basis;
      LastPosition = Controller.GlobalTranslation;

      // set Signals
      Controller.Connect("button_pressed", this, "HandleInputPressedEvent");
      Controller.Connect("button_release", this, "HandleInputReleaseEvent");

      Controller.Connect("activated", this, "HandleControllerEnable");
      Controller.Connect("deactivated", this, "HandleControllerDisable");
    }

    public override void _Process(float delta) {
      TriggerAxis = Controller.GetJoystickAxis(2);
      GripAxis = Controller.GetJoystickAxis(4);
      ThumbStickAxisX = Controller.GetJoystickAxis(0);
      ThumbStickAxisY = Controller.GetJoystickAxis(1);

      if (GrabThresholdSurpassed() && holding == null) {
        HandleGrab();
      }

      if (!GrabThresholdSurpassed() && holding != null) {
        HandleRelease();
      }

      CalculateLinearVelocity(delta);
      CalculateAngularVelocity(delta);
    }

    private void CalculateLinearVelocity(float delta) {
      LinearVelocity = (Controller.GlobalTranslation - LastPosition) / delta;
      LastPosition = Controller.GlobalTranslation;
    }

    private void CalculateAngularVelocity(float delta) {
      Quat RelativeQuat = (Controller.GlobalTransform.basis.RotationQuat() - LastBasis.RotationQuat()) / delta;
      LastBasis = Controller.GlobalTransform.basis;

      Quat outQuat = MathHelper.QuaternionToAxisAngle(RelativeQuat);
      AngularVelocity = new Vector3(outQuat.x, outQuat.y, outQuat.z);
    }

    public void HandleControllerEnable() {
      TrackedHand.Visible = true;
    }

    public void HandleControllerDisable() {
      TrackedHand.Visible = false;
    }


    // get the closest grabbable.
    public VRGrabbable? GetGrabable() {
      GD.Print("finding bodies");
      var bodies = GrabArea.GetOverlappingBodies();
      if (bodies.Count > 0) GD.Print(bodies);
      IEnumerable<VRGrabbable> grabbables = bodies.OfType<VRGrabbable>();

      VRGrabbable? closest = grabbables.OrderBy((i) => {
        return Controller.GlobalTranslation.DistanceTo(i.GlobalTranslation);
      }).FirstOrDefault();

      return closest;
    }


    private bool GrabThresholdSurpassed() {
      return (GripAxis > GrabThreshold) && GripPressed;
    }

    public void HandleRelease() {
      if (holding == null) return;
      holding.HandleRelease(LinearVelocity, AngularVelocity);
      holding = null;
    }

    private void HandleGrab() {
      VRGrabbable? grabbable = GetGrabable();
      if (grabbable == null) return;
      grabbable.HandleGrab(this);
      holding = grabbable;
    }

    // Handle Input Events
    public void HandleInputPressedEvent(int id) {
      GD.Print(Controller.GetHand(), " pressed: ", id);
      switch (id) {
        case 7:
          // A button
          AButtonPressed = true;
          return;

        case 5:
          // A touch
          AButtonTouched = true;
          return;

        case 1:
          // B button
          BButtonPressed = true;
          return;

        case 6:
          // B touch
          BButtonTouched = true;
          return;

        case 2:
          // Grab
          GripPressed = true;
          return;

        case 15:
          // Trigger
          TriggerPressed = true;
          return;

        case 13:
          // Pad press
          ThumbPadPressed = true;
          return;

        case 11:
          // Pad touch
          ThumbPadTouched = true;
          return;

        case 14:
          // Thumbstick Press
          ThumbStickPressed = true;
          return;

        case 12:
          // Thumbstick touch
          ThumbStickTouched = true;
          return;

        default:
          return;
      }
    }

    public void HandleInputReleaseEvent(int id) {
      GD.Print(Controller.GetHand(), " released: ", id);
      switch (id) {
        case 7:
          // A button
          AButtonPressed = false;
          return;

        case 5:
          // A touch
          AButtonTouched = false;
          return;

        case 1:
          // B button
          BButtonPressed = false;
          return;

        case 6:
          // B touch
          BButtonTouched = false;
          return;

        case 2:
          // Grab
          GripPressed = false;
          return;

        case 15:
          // Trigger
          TriggerPressed = false;
          return;

        case 13:
          // Pad press
          ThumbPadPressed = true;
          return;

        case 11:
          // Pad touch
          ThumbPadTouched = true;
          return;

        case 14:
          // Thumbstick Press
          ThumbStickPressed = true;
          return;

        case 12:
          // Thumbstick touch
          ThumbStickTouched = true;
          return;

        default:
          return;
      }
    }
  }
}
