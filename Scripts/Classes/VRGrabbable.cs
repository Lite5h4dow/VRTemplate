using Godot;

namespace Gameplay {
  public class VRGrabbable : RigidBody {
    [Export]
    public Vector3 GrabOffset { get; private set; }
    [Export]
    public Vector3 RotationOffset { get; private set; }

    [Export]
    public bool dualWeildable { get; private set; }
    [Export]
    public Vector3 dualWeildGrabOffset { get; private set; }
    [Export]
    public Vector3 DualWeildRotationOffset { get; private set; }
    [Export]
    public ARVRPositionalTracker.TrackerHand? FavorHand { get; private set; }


    VRHand[] Hands { get; set; }

  }
}