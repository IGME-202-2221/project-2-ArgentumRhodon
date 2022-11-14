using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PhysicsObject : MonoBehaviour
{
    // No need for Vector3's if we're using 2D physics
    // Using them just causes weird z-axis movement
    private Vector2 velocity;
    private Vector2 acceleration;
    private Vector2 direction;

    public float mass = 1f;

    public bool bounceOffWalls = false;
    public bool useGravity = false;
    public bool useFriction = false;
    public bool rotateWithDirection = false;

    public float frictionCoeff = 0.2f;

    public Vector2 Velocity => velocity;
    public Vector2 Direction => direction;
    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public float radius = 1f;

    // Start is called before the first frame update
    void Start()
    {
        direction = Random.insideUnitCircle.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (useGravity)
        {
            ApplyGravity(Physics.gravity);
        }

        if (useFriction)
        {
            ApplyFriction(frictionCoeff);
        }

        // Velocity
        velocity += acceleration * Time.deltaTime;

        // Position
        transform.position += (Vector3) velocity * Time.deltaTime;

        if(velocity.sqrMagnitude > Mathf.Epsilon)
        {
            // Direction
            direction = velocity.normalized;
        }
        // Zero Acceleration
        acceleration = Vector3.zero;

        if (rotateWithDirection)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.back, direction);
        }

        if (bounceOffWalls)
        {
            BounceOffWalls();
        }
    }

    /// <summary>
    /// Applies a force to this object following Newton's second law of motion
    /// </summary>
    /// <param name="force"></param>
    public void ApplyForce(Vector3 force)
    {
        acceleration += (Vector2) force / mass;
    }

    /// <summary>
    /// Applies friction force to this object
    /// </summary>
    /// <param name="coeff">Coefficient of friction of current interaction</param>
    private void ApplyFriction(float coeff)
    {
        Vector3 friction = velocity * -1;
        friction.Normalize();
        friction *= coeff;

        ApplyForce(friction);
    }

    /// <summary>
    /// Applies gravitational force to this object
    /// </summary>
    /// <param name="gravityForce"></param>
    private void ApplyGravity(Vector3 gravityForce)
    {
        acceleration += (Vector2) gravityForce;
    }

    // If moving off screen, change direction
    private void BounceOffWalls()
    {
        // Horizontal
        if(transform.position.x > AgentManager.Instance.maxPosition.x && velocity.x > 0)
        {
            velocity.x *= -1f;
        }
        if (transform.position.x < AgentManager.Instance.minPosition.x && velocity.x < 0)
        {
            velocity.x *= -1f;
        }

        // Vertical
        if (transform.position.y > AgentManager.Instance.maxPosition.y && velocity.y > 0)
        {
            velocity.y *= -1f;
        }
        if (transform.position.y < AgentManager.Instance.minPosition.y && velocity.y < 0)
        {
            velocity.y *= -1f;
        }
    }
}
