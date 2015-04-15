using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Kinect;
using UnityEngine;

public class LimbRenderer : MonoBehaviour
{
	public List<string> JointNames;

	private LineRenderer _lineRenderer;
	private Body _body;

	private void Start()
	{
		_lineRenderer = GetComponent<LineRenderer>();
	}

	private void Update()
	{
		if (_body == null)
		{
			return;
		}

		_lineRenderer.SetWidth(0.1f, 0.1f);
		_lineRenderer.SetVertexCount(JointNames.Count());
		var hashCode = _body.GetHashCode();
		var color = new Color((hashCode & 0xff) / 255f, ((hashCode & 0xff00) >> 8) / 255f, ((hashCode & 0xff0000) >> 16) / 255f);
		_lineRenderer.material.color = color;

		for (var i = 0; i < JointNames.Count; i++)
		{
			var jointType = ((JointType) Enum.Parse(typeof (JointType), JointNames[i]));
			var position = _body.Joints[jointType].Position.ToVector();
			_lineRenderer.SetPosition(i, position);
		}
	}

	public void UpdateBody(Body body)
	{
		_body = body;
	}
}