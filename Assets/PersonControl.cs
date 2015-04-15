using System.Collections.Generic;
using System.Linq;
using Windows.Kinect;
using UnityEngine;

public class PersonControl : MonoBehaviour
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
	}
}