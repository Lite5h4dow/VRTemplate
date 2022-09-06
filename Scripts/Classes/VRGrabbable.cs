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
	  if (Hand != null) return;
	  GetParent().RemoveChild(this);
	  hand.MountPoint.AddChild(this);
	  Translation = GrabOffset;
	  Rotation = RotationOffset;
	  Hand = hand;
	  IsGrabbed = true;
	}


	// handle global positioning and parenting to scene when released
	public void HandleRelease(Vector3 linear, Vector3 angular) {
	  Vector3 position = GlobalTranslation;
	  Vector3 rotation = GlobalRotation;
	  Hand = null;
	  Node scene = GetNode("/root/Scene");
	  GetParent().RemoveChild(this);
	  scene.AddChild(this);
	  GlobalRotation = rotation;
	  GlobalTranslation = position;
	  LinearVelocity = linear;
	  AngularVelocity = angular;
	}


	// Add this to VR Hand when grabbed and emit signal. Transform rotation and position to specified positions
	public override void _Process(float delta) {
	  base._Process(delta);

	}
  }
}
