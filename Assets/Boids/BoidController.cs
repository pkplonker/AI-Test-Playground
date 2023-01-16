using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidController : MonoBehaviour
{
	[SerializeField] private GameObject prefab;
	[SerializeField] private float spawnAmount;
	[SerializeField] private float spawnRadius = 5.0f;
	private List<Boid> boids = new();

	[field: Header("Movement")]
	[field: Range(0, 50f)]
	[field: SerializeField]
	public float initialMinVelocity { get; private set; }

	[field: Range(0, 50f)]
	[field: SerializeField]
	public float initialMaxVelocity { get; private set; }

	[field: Range(0, 50f)]
	[field: SerializeField]
	public float maxAccel { get; private set; }

	[field: Range(0, 50f)]
	[field: SerializeField]
	public float maxVelocity { get; private set; }


	[field: Header("Cohesion")]
	[field: Range(0, 50f)]
	[field: SerializeField]
	public float cohesionDistance { get; private set; }

	[field: Range(0, 50f)]
	[field: SerializeField]
	public float cohesionStrength { get; private set; }

	[field: Header("Separation")]
	[field: Range(0, 100f)]
	[field: SerializeField]
	public float separationDistance { get; private set; }

	[field: Range(0, 100f)]
	[field: SerializeField]
	public float separationStrength { get; private set; }

	[field: Header("Alignment")]
	[field: Range(0, 50f)]
	[field: SerializeField]
	public float alignmentDistance { get; private set; }

	[field: Range(0, 100f)]
	[field: SerializeField]
	public float alignmentStrength { get; private set; }

	[field: Header("Targeting")]
	[field: Range(0, 100f)]
	[field: SerializeField]
	public float targetStrength { get; private set; }

	private Vector2 targetPosition;
	[field: SerializeField] public bool debug { get; private set; }
	[field: SerializeField] public bool is2d { get; private set; }

	private int currentChunk = 0;
	private int maxChunk = 3;

	private void Awake()
	{
		for (var i = 0; i < spawnAmount; i++)
		{
			var go = Instantiate(prefab, transform.position +
			                             new Vector3(Random.Range(-spawnRadius, spawnRadius),
				                             Random.Range(-spawnRadius, spawnRadius),
				                             is2d ? 0 : Random.Range(-spawnRadius, spawnRadius)),
				quaternion.identity);
			boids.Add(go.GetComponent<Boid>());
		}

		foreach (var t in boids)
		{
			t.Init(this, boids);
		}
		
	}

	private void Update()
	{
		targetPosition = transform.position;
		foreach (var b in boids)
		{
			b.Move();
		}
	}

	private void OnDrawGizmos()
	{
		if (!debug) return;
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, spawnRadius * 2);
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(targetPosition, 1.0f);
	}

	public Vector3 GetTarget() => targetPosition;
}