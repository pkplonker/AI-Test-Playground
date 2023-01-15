using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boid : MonoBehaviour
{
	private BoidController boidController;
	private Vector2 velocity;
	private Vector2 acceleration;
	private Vector3 position;
	private Boid[] boids;

	public void Init(BoidController boidController, List<Boid> boids)
	{
		var b = new List<Boid>(boids);
		b.Remove(this);
		this.boids = b.ToArray();
		this.boidController = boidController;
		velocity = new Vector2(Random.Range(boidController.initialMinVelocity, boidController.initialMaxVelocity),
			Random.Range(boidController.initialMinVelocity, boidController.initialMaxVelocity));
	}

	private void OnDrawGizmos()
	{
		//Gizmos.color = Color.red;
		//Gizmos.DrawWireSphere(transform.position,boidController.separationDistance);
	}

	public void Move()
	{
		acceleration += (Vector2) ProcessBaseRules();
		acceleration += (Vector2) Target();
		acceleration = Vector2.ClampMagnitude(acceleration, boidController.maxAccel);
		if (boidController.debug)
			Debug.DrawRay(transform.position, acceleration.normalized, Color.white);
		velocity += acceleration * Time.deltaTime;
		velocity = Vector2.ClampMagnitude(velocity, boidController.maxVelocity);
		position += (Vector3) velocity * Time.deltaTime;

		transform.position = position;

		//rotation
		if (acceleration == Vector2.zero) return;
		var angle = Mathf.Atan2(acceleration.y, acceleration.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	private Vector3 Target() => (boidController.GetTarget() - transform.position) * boidController.targetStrength;
	
	private Vector3 ProcessBaseRules()
	{
		var alignment = Vector2.zero;
		var sqrAlignmentDistance = Mathf.Pow(boidController.alignmentDistance, 2);
		var centre = Vector3.zero;
		var count = 0;
		var sqrCohesionDistance = Mathf.Pow(boidController.cohesionDistance, 2);
		var separationDirection = Vector3.zero;
		var sqrSeperationDistance = Mathf.Pow(boidController.separationDistance, 2);
		for (int i = 0; i < boids.Length; i++)
		{
			var direction = position - boids[i].position;
			var sqrMag = Vector3.SqrMagnitude(direction);
			alignment = ProcessAlignment(sqrMag, sqrAlignmentDistance, alignment, boids[i]);
			centre = ProcessCohesion(sqrMag, sqrCohesionDistance, centre, boids[i], ref count);
			separationDirection = ProcessSeperation(direction, sqrSeperationDistance, separationDirection);
		}

		var totalAlignment = (alignment.normalized * boidController.alignmentStrength) ;
		var totalCohesion = (Vector2) ((centre / (boids.Length - 1)).normalized)* boidController.cohesionStrength;
		var toalSeparation = (Vector2) separationDirection.normalized* boidController.separationStrength;
		return totalAlignment + totalCohesion + toalSeparation;
	}

	private static Vector2 ProcessAlignment(float sqrMag, float sqrAlignmentDistance, Vector2 alignment, Boid boid)
	{
		if (sqrMag > sqrAlignmentDistance)
			alignment += boid.velocity;
		return alignment;
	}

	private static Vector3 ProcessCohesion(float sqrMag, float sqrCohesionDistance, Vector3 centre, Boid boid,
		ref int count)
	{
		if (sqrMag > sqrCohesionDistance)
		{
			centre += boid.position;
			count++;
		}

		return centre;
	}

	private static Vector3 ProcessSeperation(Vector3 direction, float sqrSeperationDistance, Vector3 separationDirection)
	{
		if (Vector2.SqrMagnitude(direction) <
		    sqrSeperationDistance && direction.sqrMagnitude > 0)
			separationDirection += direction;
		return separationDirection;
	}
}