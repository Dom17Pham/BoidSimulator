using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorManager : MonoBehaviour
{
     [Header("Behaviour Settings")]

    // Prefab for the predator unit
    public PredatorUnit predatorUnit; 

    // Number of predators to spawn
    [SerializeField] public int numPredators; 

    // The space that the predators will spawn in 
    [SerializeField] public Vector3 spawnBounds; 

    // Travelling speed of the predators
    [Range(0, 10)]
    [SerializeField] private float _minSpeed; 
    public float minSpeed { get{return _minSpeed;}}
    [Range(0, 10)]
    [SerializeField] private float _maxSpeed; 
    public float maxSpeed { get{return _maxSpeed;}}

    // Rotation speed of the predators
    [Range(0, 10)]
    [SerializeField] private float _rotationSpeed; 
    public float rotationSpeed { get{return _rotationSpeed;}}

    [Range(0, 10)]
    [SerializeField] public float _obstacleDistance;
    public float obstacleDistance { get{return _obstacleDistance;}}
    [Range(0, 10)]
    [SerializeField] public float _obstacleFactor; 
    public float obstacleFactor { get{return _obstacleFactor;}}

    [Header("Visualization Settings")]

    [Range(0, 10)]
    [SerializeField] public float _forecastTime; 
    public float forecastTime { get{return _forecastTime;}}
    [SerializeField] public bool _drawGizmos = false;
    public bool drawGizmos { get{return _drawGizmos;}}

    // Array to store all the boids
    public PredatorUnit[] allPredators {get; set;}

    void Start()
    {
        GeneratePredators();
    }

    void Update()
    {
        for(int i = 0; i < allPredators.Length; i++)
        {
            allPredators[i].MovePredator();
        }
    }

    // Generates the predators within the specified spawn bounds
    private void GeneratePredators()
    {
        allPredators = new PredatorUnit[numPredators];
        for(int i = 0; i < numPredators; i++)
        {
            var randomVector = new Vector3
                           (
                            UnityEngine.Random.Range(-spawnBounds.x / 2f + 1f, spawnBounds.x / 2f - 1f),
                            UnityEngine.Random.Range(-spawnBounds.y / 2f + 1f, spawnBounds.y / 2f - 1f),
                            UnityEngine.Random.Range(-spawnBounds.z / 2f + 1f, spawnBounds.z / 2f - 1f)
                           );
            var spawnPosition = transform.position + randomVector;
            var rotation = Quaternion.Euler(0, 0, 0);
            allPredators[i] = Instantiate(predatorUnit, spawnPosition, rotation);
            allPredators[i].AssignManager(this);
            allPredators[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed,maxSpeed));

        // Set the material color to red
        Renderer renderer = allPredators[i].GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }
        }
    }

}
