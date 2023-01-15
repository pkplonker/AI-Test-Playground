using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boid : MonoBehaviour
{
	private BoidController boidController;
	private Vector3 velocity;
	private Vector3 acceleration;
	private Vector3 position;
	private Boid[] boids;

	public void Init(BoidController boidController, List<Boid> boids)
	{
		var b = new List<Boid>(boids);
		b.Remove(this);
		this.boids = b.ToArray();
		this.boidController = boidController;
		velocity = new Vector3(Random.Range(boidController.initialMinVelocity, boidController.initialMaxVelocity),
			Random.Range(boidController.initialMinVelocity, boidController.initialMaxVelocity));
	}

	private void OnDrawGizmos()
	{
		//Gizmos.color = Color.red;
		//Gizmos.DrawWireSphere(transform.position,boidController.separationDistance);
	}

	public void Move()
	{
		acceleration += ProcessBaseRules();
		acceleration += Target();
		acceleration = Vector3.ClampMagnitude(acceleration, boidController.maxAccel);
		if (boidController.debug)
			Debug.DrawRay(transform.position, acceleration.normalized, Color.white);
		velocity += acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude(velocity, boidController.maxVelocity);
		position += velocity * Time.deltaTime;

		transform.position = position;

		//rotation
		if (acceleration == Vector3.zero) return;
		var angle = Mathf.Atan2(acceleration.y, acceleration.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	private Vector3 Target() => (boidController.GetTarget() - transform.position) * boidController.targetStrength;

	private Vector3 ProcessBaseRules()
	{
		var alignment = Vector3.zero;
		var sqrAlignmentDistance = Mathf.Pow(boidController.alignmentDistance, 2);
		var centre = Vector3.zero;
		var count = 0;
		var sqrCohesionDistance = Mathf.Pow(boidController.cohesionDistance, 2);
		var separationDirection = Vector3.zero;
		var sqrSeparationDistance = Mathf.Pow(boidController.separationDistance, 2);
		for (var i = 0; i < boids.Length; i++)
		{
			var direction = new Vector3(position.x - boids[i].position.x, position.y - boids[i].position.y, 0);
			var sqrMag = Vector3.SqrMagnitude(direction);
			alignment = ProcessAlignment(sqrMag, sqrAlignmentDistance, alignment, boids[i]);
			centre = ProcessCohesion(sqrMag, sqrCohesionDistance, centre, boids[i], ref count);
			separationDirection = ProcessSeperation(direction, sqrSeparationDistance, separationDirection);
		}

		var totalAlignment = alignment.normalized * boidController.alignmentStrength;
		var totalCohesion = (centre / (boids.Length - 1)).normalized * boidController.cohesionStrength;
		var totalSeparation = separationDirection.normalized * boidController.separationStrength;
		return totalAlignment + totalCohesion + totalSeparation;
	}

	private static Vector3 ProcessAlignment(float sqrMag, float sqrAlignmentDistance, Vector3 alignment, Boid boid)
	{
		if (sqrMag > sqrAlignmentDistance)
			alignment = new Vector3(alignment.x + boid.velocity.x, alignment.y + boid.velocity.y, 0);
		return alignment;
	}

	private static Vector3 ProcessCohesion(float sqrMag, float sqrCohesionDistance, Vector3 centre, Boid boid,
		ref int count)
	{
		if (sqrMag < sqrCohesionDistance) return centre;
		centre = new Vector3(centre.x + boid.position.x, centre.y + boid.position.y, 0);
		count++;

		return centre;
	}

	private static Vector3 ProcessSeperation(Vector3 direction, float sqrSeperationDistance,
		Vector3 separationDirection)
	{
		if (Vector3.SqrMagnitude(direction) <
		    sqrSeperationDistance && direction.sqrMagnitude > 0)
			separationDirection =
				new Vector3(separationDirection.x + direction.x, separationDirection.y + direction.y, 0);
		return separationDirection;
	}
}