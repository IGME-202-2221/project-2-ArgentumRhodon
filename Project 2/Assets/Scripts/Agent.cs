 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    protected void StayInBounds(float bufferSize = 0f, float weight = 1f)
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
                float weight = sqrPersonalSpace + (sqrDistance + 0.1f);
                Flee(other.physicsObject.Position, weight);
            }
        }
    }

    public Vector2 GetFuturePosition(float timeToLookAhead = 1f)
    {
        return (Vector2)physicsObject.Position + physicsObject.Velocity * timeToLookAhead;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, physicsObject.radius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, personalSpace);
    }
}
