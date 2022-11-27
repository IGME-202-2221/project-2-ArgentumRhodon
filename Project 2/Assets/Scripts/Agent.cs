using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PhysicsObject))]
public abstract class Agent : MonoBehaviour
{
    public PhysicsObject physicsObject;

    public float maxSpeed = 5f;
    public float maxForce = 5f;

    private Vector2 totalForce = Vector2.zero;

    private float wanderAngle = 0f;

    public float maxWanderAngle = 45f;

    public float maxWanderChangePerSecond = 10f;

    public float personalSpace = 1f;

    public float visionRange = 2f;

    private void Awake()
    {
        if(physicsObject == null)
        {
            physicsObject = GetComponent<PhysicsObject>();
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        CalculateSteeringForces();

        totalForce = Vector2.ClampMagnitude(totalForce, maxForce);
        physicsObject.ApplyForce(totalForce);

        totalForce = Vector2.zero; 
    }

    protected abstract void CalculateSteeringForces(); 

    protected void Seek(Vector2 targetPos, float weight = 1f)
    {
        // Calcluate desired velocity
        Vector2 desiredVelocity = targetPos - (Vector2) physicsObject.Position;


        // Set desired velocity magnitude to max speed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculate seek steering force
        Vector2 seekingForce = desiredVelocity - physicsObject.Velocity;

        // Apply the seek steering force
        totalForce += seekingForce * weight;

    }

    protected void Flee(Vector2 targetPos, float weight = 1)
    {
        Vector2 desiredVelocity = (Vector2) physicsObject.Position - targetPos;

        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        Vector2 fleeingForce = desiredVelocity - physicsObject.Velocity;

        totalForce += fleeingForce * weight; 
    }

    protected void Wander(float weight = 1f)
    {
        // Update the angle of our current wander
        float maxWanderChange = maxWanderChangePerSecond * Time.deltaTime;
        wanderAngle += Random.Range(-maxWanderChange, maxWanderChange);

        wanderAngle = Mathf.Clamp(wanderAngle, -maxWanderAngle, maxWanderAngle);

        // Get a position that is defined by the wander angle
        Vector2 wanderTarget = (Vector2)(Quaternion.Euler(0, 0, wanderAngle) * physicsObject.Direction.normalized) + (Vector2)physicsObject.Position;

        // Seek towards our wander position
        Seek(wanderTarget, weight);
    }

    protected void StayInBounds(float weight = 1f)
    {
        Vector2 futurePosition = GetFuturePosition(1);

        if(futurePosition.x > AgentManager.Instance.maxPosition.x ||
           futurePosition.x < AgentManager.Instance.minPosition.x ||
           futurePosition.y > AgentManager.Instance.maxPosition.y ||
           futurePosition.y < AgentManager.Instance.minPosition.y )
        {
            Seek(Vector2.zero, weight);
        }
    }

    protected void Separate<T>(List<T> agents) where T : Agent
    {
        float sqrPersonalSpace = Mathf.Pow(personalSpace, 2);

        foreach(T other in agents){
            float sqrDistance = Vector3.SqrMagnitude(other.physicsObject.Position - physicsObject.Position);

            // If no distance, we are comparing the object to itself, so do nothing
            if(sqrDistance < float.Epsilon)
            {
                continue;
            }

            if(sqrDistance < sqrPersonalSpace)
            {
                float weight = sqrPersonalSpace / (.05f * sqrDistance + 0.05f);
                Flee(other.physicsObject.Position, weight);
            }
        }
    }

    protected void AvoidObstacle(Obstacle obstacle)
    {
        // Get a vector from this agent to the obstacle
        Vector2 toObstacle = obstacle.Position - physicsObject.Position;

        // Check if the obstacle is behind
        float fwdToObstacleDot = Vector2.Dot(physicsObject.Direction, toObstacle);

        // Object is behind us, do nothing.
        if(fwdToObstacleDot < 0)
        {
            return;
        }

        // Check if the obstacle is too far to the left or right
        float rightToObstacleDot = Vector2.Dot(physicsObject.Right, toObstacle);
        if (Math.Abs(rightToObstacleDot) > physicsObject.radius + obstacle.radius)
        {
            return;
        }

        // Check if the obstacle is within vision range
        if(rightToObstacleDot > visionRange)
        {
            return;
        }

        // Passed all checks, avoid obstacle
        Vector2 desiredVelocity;

        if(rightToObstacleDot > 0)
        {
            // If on the right, steer left
            desiredVelocity = physicsObject.Right * -maxSpeed;
        }
        else
        {
            // If on the left, steer right
            desiredVelocity = physicsObject.Right * maxSpeed;
        }

        // Create a weight based on obstacle proximity
        float weight = visionRange / (0.05f * fwdToObstacleDot + 0.05f);

        // Calculate steering force from the desired velocity
        Vector2 steeringForce = (desiredVelocity - physicsObject.Velocity) * weight;

        // Apply the steering force to the total force
        totalForce += steeringForce;
    }

    protected void AvoidAllObstacles()
    {
        foreach(Obstacle obstacle in ObstacleManager.Instance.Obstacles)
        {
            AvoidObstacle(obstacle);
        }
    }

    public Vector2 GetFuturePosition(float timeToLookAhead = 1f)
    {
        return (Vector2)physicsObject.Position + physicsObject.Velocity * timeToLookAhead;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(physicsObject.Position, physicsObject.Position + physicsObject.Right);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(physicsObject.Position, physicsObject.Position + (Vector2)physicsObject.transform.forward);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, physicsObject.radius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, personalSpace);
    }
}
