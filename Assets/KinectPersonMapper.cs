using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Kinect;

public class KinectPersonMapper : MonoBehaviour
{
	private KinectSensor _sensor;
	private BodyFrameReader _reader;
	private Body[] _bodies;
	private readonly Dictionary<ulong, GameObject> _currentPersons = new Dictionary<ulong, GameObject>();
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
		_bodies = GetKinectBodies();

		if(_bodies != null)
		{
			AddUntrackedPersons();
			RemoveNoLongerTrackedPersons();
		}
	}

	private Body[] GetKinectBodies()
	{
		if (_reader == null) return null;
		var frame = _reader.AcquireLatestFrame();
		if (frame == null) return null;

		var bodies = new Body[_sensor.BodyFrameSource.BodyCount];

		frame.GetAndRefreshBodyData(bodies);

		frame.Dispose();
		frame = null;
		return bodies;
	}

	private void AddUntrackedPersons()
	{
		for (var i = 0; i < _sensor.BodyFrameSource.BodyCount; i++)
		{
			var trackingId = _bodies[i].TrackingId;
			if (trackingId != 0 && _currentPersons.ContainsKey(trackingId))
			{
				var trackedBody = Instantiate(PersonTemplate);
				trackedBody.GetComponent<PersonControl>().TrackingId = trackingId;
				_currentPersons.Add(trackingId, trackedBody);
			}
		}
	}

	private void RemoveNoLongerTrackedPersons()
	{
		var validIds = _bodies.Where(x => x.TrackingId != 0).Select(x => x.TrackingId);
		var keysToRemove = _currentPersons.Where(x => !validIds.Contains(x.Key)).Select(x => x.Key).ToList();

		foreach (var deadKey in keysToRemove)
		{
			RemovePerson(deadKey);
		}
	}

	private void RemovePerson(ulong trackingId)
	{
		Destroy(_currentPersons[trackingId]);
		_currentPersons.Remove(trackingId);
	}
}