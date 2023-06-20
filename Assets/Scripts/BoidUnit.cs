using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidUnit : MonoBehaviour
{
    [SerializeField] private float FOVAngle;
    [SerializeField] private float smoothDamp;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Vector3[] directionsToCheck;
    private List<BoidUnit> cohesionNeighbour = new List<BoidUnit>();
    private List<BoidUnit> separationNeighbour = new List<BoidUnit>();
    private List<BoidUnit> alignmentNeighbour = new List<BoidUnit>();
    private BoidManager Manager;
    private Vector3 currentVelocity;
    private Vector3 currentAvoidanceVector;
    private float speed;
    public Transform myTransform {get; set;}

    private Vector3 wanderTarget;

    private void Awake()
    {
        myTransform = transform;
    }

    public void AssignManager(BoidManager manager)
    {
        Manager = manager;
    }

    public void InitializeSpeed(float speed)
    {
        this.speed = speed;
    }
    public void MoveBoid()
    {
        FindNeighbour();
        CalculateSpeed();

        Vector3 cohesionVector = CalculateCohesionVector() * Manager.cohesionFactor;
        Vector3 alignmentVector = CalculateAlignmentVector() * Manager.alignmentFactor;
        Vector3 separationVector = CalculateSeparationVector() * Manager.separationFactor;
        Vector3 boundsVector = CalculateBoundsVector() * Manager.boundsFactor;
        Vector3 obstacleVector = CalculateObstacleVector() * Manager.obstacleFactor;
        Vector3 wanderVector = CalculateWanderVector() * Manager.wanderFactor;

        // Combine behaviour vectors into one movement vector
        var moveVector = cohesionVector + alignmentVector + separationVector + boundsVector + obstacleVector + wanderVector;
        // Dampen the boid's forward vector to reduce sharp movements and jitterness
        moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);
        // Scale move vector to the boid's speed
        moveVector = moveVector.normalized * speed;
        if (moveVector == Vector3.zero)
        {
            moveVector = myTransform.forward;
        }

        // Rotate the boid towards the direction of the move vector 
        if (moveVector != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVector, Vector3.up);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, Manager.rotationSpeed * Time.deltaTime);
        }

        myTransform.forward = moveVector.normalized;
        myTransform.position += moveVector * Time.deltaTime;

        // Check if approaching spawn bounds
        bool approachingBounds = IsApproachingBounds(myTransform.position);

        if(approachingBounds)
        {
            moveVector = -moveVector;
            myTransform.position += moveVector * Time.deltaTime;
            myTransform.rotation = Quaternion.LookRotation(moveVector);
        }
    }

    private void FindNeighbour()
    {
        cohesionNeighbour.Clear();
        separationNeighbour.Clear();
        alignmentNeighbour.Clear();
        var allboids = Manager.allBoids;
        for (int i = 0; i < allboids.Length; i++)
        {
            var currentBoid = allboids[i];
            if(currentBoid != this)
            {
                // Calculate the squared distance between the current boid and the target boid
                float currentNeighbourDistanceSqr = Vector3.SqrMagnitude(currentBoid.transform.position - transform.position);
            
                // Check if the current boid is within the cohesion distance
                if (currentNeighbourDistanceSqr <= Manager.cohesionDistance * Manager.cohesionDistance)
                {
                    cohesionNeighbour.Add(currentBoid);
                }
                // Check if the current boid is within the separation distance
                if (currentNeighbourDistanceSqr <= Manager.separationDistance * Manager.separationDistance)
                {
                    separationNeighbour.Add(currentBoid);
                }
                // Check if the current boid is within the alignment distance
                if (currentNeighbourDistanceSqr <= Manager.alignmentDistance * Manager.alignmentDistance)
                {
                    alignmentNeighbour.Add(currentBoid);
                }
            }
        }
    }
    
    private void CalculateSpeed()
    {
        if(cohesionNeighbour.Count == 0 )
        {
            return;
        }
        for(int i= 0; i < cohesionNeighbour.Count; i++)
        {
            speed += cohesionNeighbour[i].speed;
        }
        speed /= cohesionNeighbour.Count;
        speed = Mathf.Clamp(speed, Manager.minSpeed, Manager.maxSpeed);
    }
    private Vector3 CalculateCohesionVector()
    {
        var cohesionVector = Vector3.zero;
        if(cohesionNeighbour.Count == 0)
        {
            return cohesionVector;
        }
        int neighboursInFOV = 0;
        for(int i = 0; i < cohesionNeighbour.Count; i++)
        {
             // Check if the neighbor is within the field of view
            if(IsinFOV(cohesionNeighbour[i].myTransform.position))
            {
                neighboursInFOV++;
                cohesionVector += cohesionNeighbour[i].myTransform.position;
            }
        }

        cohesionVector /= neighboursInFOV;
        cohesionVector -= myTransform.position;
        cohesionVector = cohesionVector.normalized;
        return cohesionVector;
    }

    private Vector3 CalculateAlignmentVector()
    {
        var alignmentVector = myTransform.forward;
        if(alignmentNeighbour.Count == 0){
            return alignmentVector;
        }
        int neighboursInFOV = 0;
        for(int i = 0; i < alignmentNeighbour.Count; i++)
        {
            // Check if the neighbor is within the field of view
            if(IsinFOV(alignmentNeighbour[i].myTransform.position))
            {
                neighboursInFOV++;
                alignmentVector += alignmentNeighbour[i].myTransform.forward;
            }
        }

        alignmentVector /= neighboursInFOV;
        alignmentVector = alignmentVector.normalized;
        return alignmentVector;
    }

    private Vector3 CalculateSeparationVector()
    {
        var separationVector = Vector3.zero;
        if(alignmentNeighbour.Count == 0){
            return Vector3.zero;
        }
        int neighboursInFOV = 0;
        for(int i = 0; i < separationNeighbour.Count; i++)
        {
            // Check if the neighbor is within the field of view
            if(IsinFOV(separationNeighbour[i].myTransform.position))
            {
                neighboursInFOV++;
                separationVector += (myTransform.position - separationNeighbour[i].myTransform.position);
            }
        }

        separationVector /= neighboursInFOV;
        separationVector = separationVector.normalized;
        return separationVector;
    }

    private Vector3 CalculateBoundsVector()
    {
        var offsetToCenter = Manager.transform.position - myTransform.position;
        bool isNearCenter = (offsetToCenter.magnitude >= Manager.boundsDistance * 0.9f); 
        return isNearCenter ? offsetToCenter.normalized : Vector3.zero;
    }

    private Vector3 CalculateObstacleVector()
    {
        var obstacleVector = Vector3.zero;
        RaycastHit hit;
        if(Physics.Raycast(myTransform.position, myTransform.forward, out hit, Manager.obstacleDistance, obstacleMask))
        {
            obstacleVector = FindAvoidanceDirection();
        }
        else
        {
            currentAvoidanceVector = Vector3.zero;
        }
        return obstacleVector;
    }

    private Vector3 FindAvoidanceDirection()
    {
        if(currentAvoidanceVector != Vector3.zero)
        {
            RaycastHit hit;
            if(Physics.Raycast(myTransform.position, myTransform.forward, out hit, Manager.obstacleDistance, obstacleMask))
            {
                return currentAvoidanceVector;
            }
        }
        float maxDistance = int.MinValue;
        var direction = Vector3.zero;
        for(int i= 0; i < directionsToCheck.Length; i++)
        {
            RaycastHit hit;
            var currentDirection = myTransform.TransformDirection(directionsToCheck[i].normalized);
            if(Physics.Raycast(myTransform.position, currentDirection, out hit, Manager.obstacleDistance, obstacleMask))
            {
                float currentDistance = (hit.point - myTransform.position).sqrMagnitude;
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    direction = currentDirection;
                }
            }
            else
            {
                direction = currentDirection;
                currentAvoidanceVector = currentDirection.normalized;
                return currentDirection.normalized;
            }
        }
        return direction.normalized;
    }

    private Vector3 CalculateWanderVector()
    {
        // Calculate the target position in world space
        this.wanderTarget = GetRandomPointInBounds();

        // Calculate the desired direction towards the target position
         Vector3 desiredDirection = (wanderTarget - myTransform.position).normalized;

        // Calculate the wander force
        Vector3 wanderVector = desiredDirection * speed - currentVelocity;
        return wanderVector;
    }

    private Vector3 GetRandomPointInBounds()
    {
        Vector3 spawnCenter = transform.position;
        Vector3 spawnSize = Manager.spawnBounds;

        // Calculate random coordinates within the spawn bounds
        float randomX = Random.Range(spawnCenter.x - spawnSize.x / 2f, spawnCenter.x + spawnSize.x / 2f);
        float randomY = Random.Range(spawnCenter.y - spawnSize.y / 2f, spawnCenter.y + spawnSize.y / 2f);
        float randomZ = Random.Range(spawnCenter.z - spawnSize.z / 2f, spawnCenter.z + spawnSize.z / 2f);

        return new Vector3(randomX, randomY, randomZ);
    }

    private bool IsinFOV(Vector3 position)
    {
        return Vector3.Angle(myTransform.forward, position - myTransform.position) <= FOVAngle;
    }

    private bool IsApproachingBounds(Vector3 newPosition)
    {
        Vector3 boundsMin = Manager.transform.position - Manager.spawnBounds / 2;
        Vector3 boundsMax = Manager.transform.position + Manager.spawnBounds / 2;
        // Margin to determine the approach distance
        float margin = 0.1f; 

        return newPosition.x < boundsMin.x + margin ||
               newPosition.x > boundsMax.x - margin ||
               newPosition.y < boundsMin.y + margin ||
               newPosition.y > boundsMax.y - margin ||
               newPosition.z < boundsMin.z + margin ||
               newPosition.z > boundsMax.z - margin;
    }
    private void OnDrawGizmos()
    {
        if (!Manager.drawGizmos)
        return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(myTransform.position, 0.1f);

        Gizmos.color = Color.green;
        // Calculate the forecasted position
        Vector3 forecastedPosition = myTransform.position + myTransform.forward * speed * Manager.forecastTime;
        Gizmos.DrawLine(myTransform.position, forecastedPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(myTransform.position, myTransform.forward);
    }
}