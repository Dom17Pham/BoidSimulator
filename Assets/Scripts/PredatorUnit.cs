using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorUnit : MonoBehaviour
{
    private PredatorManager Manager;
    private float speed;
    private Vector3 currentVelocity;
    public Transform myTransform {get; set;}
    [SerializeField] private float smoothDamp;
    [SerializeField] private LayerMask obstacleMask;
    private Vector3 currentAvoidanceVector;
    [SerializeField] private Vector3[] directionsToCheck;
    private void Awake()
    {
        myTransform = transform;
    }

    public void AssignManager(PredatorManager manager)
    {
        Manager = manager;
    }

    public void InitializeSpeed(float speed)
    {
        this.speed = speed;
    }

    public void MovePredator()
    {
        speed = Mathf.Clamp(speed, Manager.minSpeed, Manager.maxSpeed);
        Vector3 obstacleVector = CalculateObstacleVector() * Manager.obstacleFactor;
        var moveVector = new Vector3();
        moveVector += obstacleVector;

        // Find all prey objects with the "prey" tag
        GameObject[] preyObjects = GameObject.FindGameObjectsWithTag("prey");
        Vector3 averagePreyPosition = Vector3.zero;

        // Calculate the average position of all prey objects
        foreach (GameObject preyObject in preyObjects)
        {
            averagePreyPosition += preyObject.transform.position;
        }

        if (preyObjects.Length > 0)
        {
            // Calculate the direction to the average prey position
            Vector3 preyDirection = averagePreyPosition / preyObjects.Length - myTransform.position;

            // Dampen the boid's forward vector to reduce sharp movements and jitteriness
            moveVector = Vector3.SmoothDamp(myTransform.forward, preyDirection, ref currentVelocity, smoothDamp);
        }

        // Scale move vector to the predator's speed
        moveVector = moveVector.normalized * speed;
        if (moveVector == Vector3.zero)
        {
            moveVector = myTransform.forward;
        }

        // Rotate the predator towards the direction of the move vector 
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

        Gizmos.DrawSphere(myTransform.position, 0.1f);

        Gizmos.color = Color.green;
        // Calculate the forecasted position
        Vector3 forecastedPosition = myTransform.position + myTransform.forward * speed * Manager.forecastTime;
        Gizmos.DrawLine(myTransform.position, forecastedPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(myTransform.position, myTransform.forward);
    }
}
