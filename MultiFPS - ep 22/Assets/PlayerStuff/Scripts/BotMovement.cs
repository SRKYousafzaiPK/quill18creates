using UnityEngine;
using System.Collections;

public class BotMovement : MonoBehaviour {

	// This script is only for "my bot" -- in other words, only the "local" client will have this
	// enabled.  In practice, this means the MASTER client -- which is probably responsible for
	// spawning bots.
	// REMOTE bots will have this script disabled.

	NetworkCharacter netChar;
	static Waypoint[] waypoints;

	Waypoint destination;
	float waypointTargetDistance = 1f;

	// Use this for initialization
	void Start () {
		netChar = GetComponent<NetworkCharacter>();

		if(waypoints == null) {
			waypoints = GameObject.FindObjectsOfType<Waypoint>();
		}

		destination = GetClosestWaypoint();
	}

	Waypoint GetClosestWaypoint() {
		Waypoint closest = null;
		float dist = 0;

		foreach(Waypoint w in waypoints) {
			if(closest==null || Vector3.Distance(transform.position, w.transform.position) < dist) {
				closest = w;
				dist = Vector3.Distance(transform.position, w.transform.position);
			}
		}

		return closest;
	}
	
	// Update is called once per frame
	void Update () {
		if(destination != null) {
			// We have a destination -- let's check if we have arrived.
			if( Vector3.Distance(destination.transform.position, transform.position) <= waypointTargetDistance ) {
				// We have arrived!

				if(destination.connectWPs != null && destination.connectWPs.Length > 0) {
					// Pick a connected waypoint
					destination = destination.connectWPs[ Random.Range(0, destination.connectWPs.Length) ];
				}
				else {
					// Waypoint isn't connected to anything, which is kind of a problem.
					// We need proper navmesh type stuff!
				}

			}
		}

		// We STILL have a destination, so let's move towards it.
		if(destination != null) {
			netChar.direction = destination.transform.position - transform.position;
			netChar.direction.y = 0;
			netChar.direction.Normalize();

			transform.rotation = Quaternion.LookRotation(netChar.direction);
		}
		else {
			// No destination, so let's just stop and be idle.
			netChar.direction = Vector3.zero;
		}
	}
}
