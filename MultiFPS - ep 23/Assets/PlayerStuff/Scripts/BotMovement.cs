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

	float aggroRange = 1000000f;

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

		}
		else {
			// No destination, so let's just stop and be idle.
			netChar.direction = Vector3.zero;
		}

		// Let's figure out where we should be facing!
		// By default: Look where we're going.
		Vector3 lookDirection = netChar.direction;

		// Do we have an enemy target in range?
		TeamMember closest = null;
		float dist = 0;
		foreach(TeamMember tm in GameObject.FindObjectsOfType<TeamMember>()) {	// WARNING: SLOW!
			if(tm == GetComponent<TeamMember>()) {
				// How Zen! We found ourselves.
				// Loop to the next possible target!
				continue;
			}

			if(tm.teamID==0 || tm.teamID != GetComponent<TeamMember>().teamID) {
				// Target is on the enemy team!
				float d = Vector3.Distance(tm.transform.position, transform.position);
				if( d <= aggroRange ) {
					// Target is in range!

					// TODO: Do a raycast to make sure we actually have line of sight!

					// Is the target closer than the last target we found?
					if(closest==null || d < dist) {
						closest = tm;
						dist = d;
					}
				}
			}
		}

		if(closest != null) {
			// We have a target, so let's use that direction as our look direction!
			lookDirection = closest.transform.position - transform.position;
		}

		// Rotate towards our look direction
		Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
		lookRotation.eulerAngles = new Vector3(0, lookRotation.eulerAngles.y, 0);
		transform.rotation = lookRotation;

		if(closest != null) {
			// Figure out the relative vertical angle to our target and adjust aimAngle
			Vector3 localLookDirection = transform.InverseTransformPoint(closest.transform.position);
			float targetAimAngle = Mathf.Atan2(localLookDirection.y, localLookDirection.z) * Mathf.Rad2Deg;
			netChar.aimAngle = targetAimAngle;
		}
		else {
			// We don't have a target, just aim casual
			netChar.aimAngle = 0;
		}
	}
}
