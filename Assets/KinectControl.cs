using System.Collections.Generic;
using System.Linq;
using Windows.Kinect;
using UnityEngine;

public class KinectControl : MonoBehaviour
{
	private KinectSensor _sensor;
	private BodyFrameReader _reader;
	private Body[] _data;
	public LimbRenderer LeftArm;
	public LimbRenderer RightArm;
	public LimbRenderer Spine;
	public LimbRenderer LeftLeg;
	public LimbRenderer RightLeg;
	public ulong TrackingId;

	private void Start()
	{
		_sensor = KinectSensor.GetDefault();

		if (_sensor != null)
		{
			_reader = _sensor.BodyFrameSource.OpenReader();

			if (!_sensor.IsOpen)
			{
				_sensor.Open();
			}
		}
	}

	private void OnApplicationQuit()
	{
		if (_reader != null)
		{
			_reader.Dispose();
			_reader = null;
		}

		if (_sensor != null)
		{
			if (_sensor.IsOpen)
			{
				_sensor.Close();
			}
			_sensor = null;
		}
	}

	private void Update()
	{
		if (_reader == null) return;
		var frame = _reader.AcquireLatestFrame();
		if (frame == null) return;

		if (_data == null)
		{
			_data = new Body[_sensor.BodyFrameSource.BodyCount];
		}

		frame.GetAndRefreshBodyData(_data);

		frame.Dispose();
		frame = null;


		var body = _data.SingleOrDefault(x => x.TrackingId == TrackingId);

		if (body == null) return;

		LeftArm.UpdateBody(body);
		RightArm.UpdateBody(body);
		Spine.UpdateBody(body);
		LeftLeg.UpdateBody(body);
		RightLeg.UpdateBody(body);

		//if (body.HandRightState != HandState.Closed)
		//{
		//	gameObject.transform.position = body.Joints[JointType.HandRight].Position.ToVector()*10;
		//}
		//if (body.HandLeftState != HandState.Closed)
		//{
		//	var angley =
		//		body.Joints[JointType.HandLeft].Position.X;
		//	var anglex =
		//		body.Joints[JointType.HandLeft].Position.Y;
		//	var anglez =
		//		body.Joints[JointType.HandLeft].Position.Z;

		//	gameObject.transform.rotation =
		//		Quaternion.Euler(
		//			gameObject.transform.rotation.x + anglex*100,
		//			gameObject.transform.rotation.y + angley*100,
		//			gameObject.transform.rotation.z + anglez*100);
		//}
	}
}

public static class CameraSpacePointExtensions
{
	public static Vector3 ToVector(this CameraSpacePoint point)
	{
		return new Vector3(point.X, point.Y, point.Z)*6;
	}
}
