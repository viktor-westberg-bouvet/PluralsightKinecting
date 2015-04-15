using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Kinect;

public class PersonController : MonoBehaviour
{
	private KinectSensor _sensor;
	private BodyFrameReader _reader;
	private Body[] _data;
	private readonly Dictionary<ulong, GameObject> _currentBodies = new Dictionary<ulong, GameObject>();
	public GameObject PersonTemplate;

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

		for (var i = 0; i < _sensor.BodyFrameSource.BodyCount; i++)
		{
			var trackingId = _data[i].TrackingId;

			if (trackingId == 0)
			{
				continue;
			}

			if (_currentBodies.ContainsKey(trackingId))
			{
				if (!_data[i].IsTracked)
				{
					KillBody(trackingId);
				}
			}
			else
			{
				var trackedBody = Instantiate(PersonTemplate);
				trackedBody.GetComponent<KinectControl>().TrackingId = trackingId;
				_currentBodies.Add(trackingId, trackedBody);
			}
		}

		var validIds = _data.Where(x => x.TrackingId != 0).Select(x => x.TrackingId);
		var keysToRemove = _currentBodies.Where(x => !validIds.Contains(x.Key)).Select(x => x.Key).ToList();

		foreach (var deadKey in keysToRemove)
		{
			KillBody(deadKey);
		}
	}

	private void KillBody(ulong trackingId)
	{
		Destroy(_currentBodies[trackingId]);
		_currentBodies.Remove(trackingId);
	}
}