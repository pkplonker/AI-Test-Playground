using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boid : MonoBehaviour
{
	private BoidController boidController;
	private Vector2 velocity;
	private Vector2 acceleration;
	private Vector2 position;


	public void Init(BoidController boidController)
	{
		this.boidController = boidController;
		velocity = new Vector2(Random.Range(boidController.initialMinVelocity, boidController.initialMaxVelocity),
			Random.Range(boidController.initialMinVelocity, boidController.initialMaxVelocity));
	}

	private void OnDrawGizmos()
	{
		//Gizmos.color = Color.red;
		//Gizmos.DrawWireSphere(transform.position,boidController.separationDistance);
	}

	public void Move(List<Boid> boids)
	{
		acceleration += Cohesion(boids)* boidController.cohesionStrength;
		acceleration += Separation(boids)*boidController.separationStrength;
		acceleration += Alignment(boids)* boidController.alignmentStrength;
		acceleration += Target();
		acceleration = Vector2.ClampMagnitude(acceleration, boidController.maxAccel);
		if(boidController.debug)
			Debug.DrawRay(transform.position, acceleration.normalized, Color.white);
		velocity += acceleration * Time.deltaTime;
		velocity = Vector2.ClampMagnitude(velocity, boidController.maxVelocity);
		position += velocity  * Time.deltaTime;
		transform.position = position;
		
		//rotation
		if (acceleration != Vector2.zero)
		{
			var angle = Mathf.Atan2(acceleration.y, acceleration.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}

	private Vector2 Target() => boidController.GetTarget() - transform.position;


	private Vector2 Separation(List<Boid> boids)
	{
		Vector2 separationDirection = Vector2.zero;
		foreach (var boid in boids)
		{
			if (boid == this) continue;
			if (Vector2.Distance(transform.position, boid.transform.position) > boidController.separationDistance) continue;
			Vector2 direction = position - boid.position;
			if (direction.magnitude > 0)
				separationDirection += direction.normalized / direction.magnitude;
			
		}

		return  separationDirection.normalized;
	}

	private Vector2 Cohesion(List<Boid> boids)
	{
		Vector2 centre = Vector2.zero; 
		var count = 0;
		foreach (var boid in boids)
		{
			if (boid == this) continue;
			var dist =Vector2.Distance(transform.position, boid.transform.position);
			if (dist > boidController.cohesionDistance) continue;
			centre += (Vector2) boid.transform.position;
			count++;
		}

		return (centre / (boids.Count - 1)).normalized;
	}

	private Vector2 Alignment(List<Boid> boids)
	{
		Vector2 alignment = Vector2.zero; 
		foreach (var boid in boids)
		{
			if (boid == this) continue;
			var dist =Vector2.Distance(transform.position, boid.transform.position);
			if (dist > boidController.alignmentDistance) continue;
			alignment += boid.velocity;
			
		}

		return alignment.normalized;
	}
}