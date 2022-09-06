using Godot;
namespace Gameplay {
  public class MathHelper {
    public static Quat QuaternionToAxisAngle(Quat quat) {
      Quat axisAngle = new Quat(0, 0, 0, 0);

      if (quat.w > 1) quat = quat.Normalized();

      float angle = 2f * Mathf.Acos(quat.w);

      axisAngle.w = Mathf.Sqrt(1 - quat.w * quat.w);

      if (axisAngle.w < 0.0000001) {
        axisAngle.x = quat.x;
        axisAngle.y = quat.y;
        axisAngle.z = quat.z;
      } else {
        axisAngle.x = quat.x / axisAngle.w;
        axisAngle.y = quat.y / axisAngle.w;
        axisAngle.z = quat.z / axisAngle.w;
      }

      return axisAngle;
    }
  }
}