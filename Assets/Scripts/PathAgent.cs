using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathAgent : MonoBehaviour
{
	private WaypointSystem waypointSystem;
	private int currentPathIndex;
	private float speed = 5f;

	void Start()
	{
		waypointSystem = FindObjectOfType<WaypointSystem>();
		transform.position = waypointSystem.Waypoints[0];
		currentPathIndex = 0;
	}

	void Update()
	{
		Vector3 target = waypointSystem.Waypoints[currentPathIndex];
		transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
		if (Vector3.Equals(target, transform.position))
		{
			currentPathIndex = (currentPathIndex + 1) % waypointSystem.Waypoints.Count;
		}
	}











}
