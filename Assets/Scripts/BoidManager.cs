using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
 
    [Header("Behaviour Settings")]

    // Prefab for the boid unit
    public BoidUnit boidUnit; 

    // Number of boids to spawn
    [SerializeField] public int numBoids; 

    // The space that the boids will spawn in 
    [SerializeField] public Vector3 spawnBounds; 

    // Travelling speed of the boids
    [Range(0, 10)]
    [SerializeField] private float _minSpeed; 
    public float minSpeed { get{return _minSpeed;}}
    [Range(0, 10)]
    [SerializeField] private float _maxSpeed; 
    public float maxSpeed { get{return _maxSpeed;}}

    // Rotation speed of the boids
    [Range(0, 10)]
    [SerializeField] private float _rotationSpeed; 
    public float rotationSpeed { get{return _rotationSpeed;}}

    // Wander attributes of the boids
    [Range(0, 10)]
    [SerializeField] private float _wanderDistance; 
    public float wanderDistance { get{return _wanderDistance;}}
    [Range(0, 10)]
    [SerializeField] private float _wanderJitter; 
    public float wanderJitter { get{return _wanderJitter;}}


    [Header("Flock Settings")]

    // Separation distance for the boids in the flock
    [Range(0, 10)]
    [SerializeField] public float _separationDistance; 
    public float separationDistance { get{return _separationDistance;}}

    // Alignment distance for the boids in the flock
    [Range(0, 10)]
    [SerializeField] public float _alignmentDistance; 
    public float alignmentDistance { get{return _alignmentDistance;}}

    // Cohesion distance for the boids in the flock
    [Range(0, 10)]
    [SerializeField] public float _cohesionDistance; 
    public float cohesionDistance { get{return _cohesionDistance;}}

    // Bounds distance for the boids in the flock
    [Range(0, 10)]
    [SerializeField] public float _boundsDistance; 
    public float boundsDistance { get{return _boundsDistance;}}

    // Obstacle distance for the boids in the flock
    [Range(0, 10)]
    [SerializeField] public float _obstacleDistance;
    public float obstacleDistance { get{return _obstacleDistance;}}


    [Header("Behaviour Weights")]

    // Cohesion weight for the boid behavior
    [Range(0, 10)]
    [SerializeField] public float _cohesionFactor; 
    public float cohesionFactor { get{return _cohesionFactor;}}

    // Separation weight for the boid behavior
    [Range(0, 10)]
    [SerializeField] public float _separationFactor; 
    public float separationFactor { get{return _separationFactor;}}

    // Alignment weight for the boid behavior
    [Range(0, 10)]
    [SerializeField] public float _alignmentFactor; 
    public float alignmentFactor { get{return _alignmentFactor;}}

    // Bounds weight for the boid behavior
    [Range(0, 10)]
    [SerializeField] public float _boundsFactor; 
    public float boundsFactor { get{return _boundsFactor;}}

    // Obstacle avoidance weight for the boid behavior
    [Range(0, 10)]
    [SerializeField] public float _obstacleFactor; 
    public float obstacleFactor { get{return _obstacleFactor;}}

    // Wander weight for the boid behavior
    [Range(0, 10)]
    [SerializeField] public float _wanderFactor; 
    public float wanderFactor { get{return _wanderFactor;}}

    [Header("Visualization Settings")]

    [Range(0, 10)]
    [SerializeField] public float _forecastTime; 
    public float forecastTime { get{return _forecastTime;}}
    [SerializeField] public bool _drawGizmos = false;
    public bool drawGizmos { get{return _drawGizmos;}}
    
    // Array to store all the boids
    public BoidUnit[] allBoids {get; set;}
    
    void Start()
    {
        GenerateBoids();
    }

    void Update()
    {
        for(int i = 0; i < allBoids.Length; i++)
        {
            allBoids[i].MoveBoid();
        }
    }

    // Generates the boids within the specified spawn bounds
    private void GenerateBoids()
    {
        allBoids = new BoidUnit[numBoids];
        for(int i = 0; i < numBoids; i++)
        {
            var randomVector = new Vector3
                           (
                            UnityEngine.Random.Range(-spawnBounds.x / 2f + 1f, spawnBounds.x / 2f - 1f),
                            UnityEngine.Random.Range(-spawnBounds.y / 2f + 1f, spawnBounds.y / 2f - 1f),
                            UnityEngine.Random.Range(-spawnBounds.z / 2f + 1f, spawnBounds.z / 2f - 1f)
                           );
            var spawnPosition = transform.position + randomVector;
            var rotation = Quaternion.Euler(0, 0, 0);
            allBoids[i] = Instantiate(boidUnit, spawnPosition, rotation);
            allBoids[i].AssignManager(this);
            allBoids[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed,maxSpeed));

        // Set the material color to cyan
        Renderer renderer = allBoids[i].GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.cyan;
        }
        }
    }

    // Draws the spawn bounds wireframe in the scene view for visual reference
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireCube(transform.position, spawnBounds);
    }

}
