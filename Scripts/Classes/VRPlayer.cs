using Godot;

namespace Gameplay {
  public class VRPlayer : RigidBody {
    protected ARVRCamera camera { get; private set; }
    protected ARVROrigin origin { get; private set; }
    protected CollisionShape collider { get; private set; }
    protected RayCast headCast { get; private set; }
    protected VRHand LeftHand { get; private set; }
    protected VRHand RightHand { get; private set; }

    [Export]
    public float MaxHeight { get; private set; }
    [Export]
    public float GrabThreshold { get; private set; }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
      // VR Gear
      camera = GetNode<ARVRCamera>("FPController/ARVRCamera");
      origin = GetNode<ARVROrigin>("FPController");
      collider = GetNode<CollisionShape>("BodyCollider");
      headCast = GetNode<RayCast>("HeadCast");

      // Set Hands
      LeftHand = new VRHand(
        GetNode<ARVRController>("FPController/LeftHandController"),
        GetNode<Spatial>("FPController/LeftHand"),
        GetNode<Area>("FPController/RightHandController/GrabArea"),
        GrabThreshold
      );

      RightHand = new VRHand(
        GetNode<ARVRController>("FPController/RightHandController"),
        GetNode<Spatial>("FPController/RightHand"),
        GetNode<Area>("FPController/LeftHandController/GrabArea"),
        GrabThreshold
      );
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
      // player rigid body alignment and collision alignment
      AlignCamera(camera, origin, this);
      AlignHeadCast(headCast, camera);
      AlignAndShapeCollision(collider, camera, headCast);

      // HandProcesses
      LeftHand._Process(delta);
      RightHand._Process(delta);
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

  }
}