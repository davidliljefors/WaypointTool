using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class WaypointSystem : MonoBehaviour
{
	[SerializeField] private List<Vector3> waypoints = new List<Vector3>();

	public List<Vector3> Waypoints => waypoints;
}
