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

    public float visionRange = 3f;

    public float arriveDistance = 3f;

    public float visionConeAngle = 45f;

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

    protected void Pursue(Agent other, float timeToLookAhead = 1f, float weight = 1f)
    {
        Vector2 otherFuturePosition = other.GetFuturePosition(timeToLookAhead);

        float futurePositionDist = Vector2.SqrMagnitude(otherFuturePosition - other.physicsObject.Position);
        float distToOther = Vector2.SqrMagnitude(physicsObject.Position - other.physicsObject.Position);

        if(futurePositionDist < distToOther)
        {
            Seek(otherFuturePosition, weight);
        }
        else
        {
            Seek(other.physicsObject.Position, weight);
        }
    }

    protected void Flee(Vector2 targetPos, float weight = 1)
    {
        Vector2 desiredVelocity = (Vector2) physicsObject.Position - targetPos;

        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        Vector2 fleeingForce = desiredVelocity - physicsObject.Velocity;

        totalForce += fleeingForce * weight; 
    }

    protected void Evade(Agent other, float timeToLookAhead = 1f, float weight = 1f)
    {
        Vector2 otherFuturePosition = other.GetFuturePosition(timeToLookAhead);

        float futurePositionDist = Vector2.SqrMagnitude(otherFuturePosition - other.physicsObject.Position);
        float distToOther = Vector2.SqrMagnitude(physicsObject.Position - other.physicsObject.Position);

        if (futurePositionDist < distToOther)
        {
            Flee(otherFuturePosition, weight);
        }
        else
        {
            Flee(other.physicsObject.Position, weight);
        }
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

    protected void WanderTo(Vector2 wanderTarget, float pathEfficiency, float weight = 1f)
    {
        Vector2 wanderDirection = wanderTarget - physicsObject.Position;

        if(pathEfficiency <= 0f)
        {
            Debug.Log("Path Efficiency cannot be less than or equal to zero!");
            return;
        }

        float maxWanderChange = maxWanderChangePerSecond * Time.deltaTime / pathEfficiency;
        wanderAngle += Random.Range(-maxWanderChange, maxWanderChange);

        wanderAngle = Mathf.Clamp(wanderAngle, -maxWanderAngle, maxWanderAngle);

        Vector2 wanderPosition = (Vector2)(Quaternion.Euler(0, 0, wanderAngle) * wanderDirection.normalized) + (Vector2)physicsObject.Position;

        Seek(wanderPosition, weight);
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
                float weight = sqrPersonalSpace / (sqrDistance + 0.1f);
                Flee(other.physicsObject.Position, weight);
            }
        }
    }

    protected void Align<T>(List<T> agents, float weight = 1f) where T : Agent
    {
        // Find sum of the direction my neighbors are moving in
        Vector2 flockDirection = Vector2.zero;

        foreach(T agent in agents)
        {
            if(IsVisible(agent))
            {
                flockDirection += agent.physicsObject.Direction;
            }
        }

        // Early out if no other agents are visible
        if(flockDirection == Vector2.zero)
        {
            return;
        }

        // Normalize our found flock direction
        flockDirection = flockDirection.normalized;

        // Calculate our steering force 
        Vector2 steeringForce = flockDirection - physicsObject.Velocity;

        totalForce += steeringForce * weight;
    }

    protected void Cohere<T>(List<T> agents, float weight = 1f) where T : Agent
    {
        // Calculate the average position of the flock
        Vector2 flockPosition = Vector2.zero;
        int totalVisibleAgents = 0;

        foreach(T agent in agents)
        {
            if (IsVisible(agent))
            {
                totalVisibleAgents++;
                flockPosition += agent.physicsObject.Position;
            }
        }

        // Early out if we can't see anyone
        if(totalVisibleAgents == 0)
        {
            return;
        }

        // Average flock Position
        flockPosition /= totalVisibleAgents;

        // Seek the center of the flock 
        Seek(flockPosition, weight);
    }

    private bool IsVisible(Agent agent)
    {

        // Check if the other agent is within our vision range
        float sqrDistance = Vector2.SqrMagnitude(physicsObject.Position - agent.physicsObject.Position);

        // Skip the other agent if it is actually this one
        if (sqrDistance < float.Epsilon)
        {
            return false;
        }

        // Vision cone
        float angle = Vector2.Angle(physicsObject.Direction, agent.physicsObject.Position - physicsObject.Position);

        if (angle > visionConeAngle)
        {
            return false;
        }

        // Return true if the other agent is within vision range
        return sqrDistance < visionRange * visionRange;
    } 

    protected void Flock<T>(List<T> agents, float cohereWeight = 1f, float alignWeight = 1f) where T : Agent
    {
        Separate(agents);
        Cohere(agents, cohereWeight);
        Align(agents, alignWeight);
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
        if(fwdToObstacleDot > visionRange)
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
        float weight = visionRange / (fwdToObstacleDot + 0.1f);

        // Calculate steering force from the desired velocity
        Vector2 steeringForce = (desiredVelocity - physicsObject.Velocity) * weight;

        // Apply the steering force to the total force
        totalForce += steeringForce;
    }

    protected void Arrive(Vector2 destination, float weight = 1f)
    {
        Vector2 desiredVelocity = destination - physicsObject.Position;

        float sqrDist = desiredVelocity.magnitude;

        desiredVelocity.Normalize();

        float sqrArriveDistance = Mathf.Pow(arriveDistance, 2);
        if(sqrDist < sqrArriveDistance)
        {
            desiredVelocity *= Mathf.Lerp(0f, maxSpeed, sqrDist / sqrArriveDistance);
        }
        else
        {
            desiredVelocity *= maxSpeed;
        }

        Vector2 steeringVector = desiredVelocity - physicsObject.Velocity;

        totalForce += steeringVector * weight;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(physicsObject.Position, physicsObject.radius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(physicsObject.Position, personalSpace);
    }
}
