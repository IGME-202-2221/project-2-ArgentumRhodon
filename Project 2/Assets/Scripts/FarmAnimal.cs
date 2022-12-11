using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmAnimal : Agent
{
    public enum AnimalState
    {
        Wandering,
        Panicking
    }

    private AnimalState currentState = AnimalState.Wandering;
    public AnimalState CurrentState => currentState;

    public Wolf attacker;

    protected override void CalculateSteeringForces()
    {
        if(attacker != null)
        {
            maxSpeed = 3.5f;
            currentState = AnimalState.Panicking;
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            maxSpeed = 1.25f;
            currentState = AnimalState.Wandering;
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        switch (currentState)
        {
            case AnimalState.Wandering:
            {
                Wander();
                break;
            }
            case AnimalState.Panicking:
            {
                Evade(attacker, 5f);
                break;
            }
        }

        AvoidAllObstacles();
        StayInBounds(2f);
    }
}
