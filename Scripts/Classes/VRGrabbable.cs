#nullable enable

using Godot;

namespace Gameplay {
  public class VRGrabbable : RigidBody {
    [Export]
    public Vector3 GrabOffset { get; private set; }
    [Export]
    public Vector3 RotationOffset { get; private set; }

    public VRHand? Hand { get; private set; }

    private bool IsGrabbed { get; set; }

    // handle relative position and parenting when grabbed
    public void HandleGrab(VRHand hand) {
      // if (Hand != null) Hand.HandleRelease();
      if (Hand == null) {
        GetParent().RemoveChild(this);
        hand.Controller.AddChild(this);
        Translation = GrabOffset;
        Rotation = RotationOffset;
        Hand = hand;
        IsGrabbed = true;
      } else {

      }
    }


    // handle global positioning and parenting to scene when released
    public void HandleRelease() {
      Vector3 position = GlobalTranslation;
      Vector3 rotation = GlobalRotation;
      Hand = null;
      GetParent().RemoveChild(this);
      Owner.AddChild(this);
      GlobalRotation = rotation;
      GlobalTranslation = position;
    }


    // Add this to VR Hand when grabbed and emit signal. Transform rotation and position to specified positions
    public override void _Process(float delta) {
      base._Process(delta);

    }
  }
}