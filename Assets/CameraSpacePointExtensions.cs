using Windows.Kinect;
using UnityEngine;

public static class CameraSpacePointExtensions
{
	public static Vector3 ToVector(this CameraSpacePoint point)
	{
		return new Vector3(point.X, point.Y, point.Z)*6;
	}
}