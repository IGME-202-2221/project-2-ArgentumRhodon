using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FarmAnimal;

public class Wolf : Agent
{
    public enum WolfState
    {
        Wandering,
        Attacking
    }

    private WolfState currentState = WolfState.Wandering;
    public WolfState CurrentState => currentState;

    private FarmAnimal target;
    private FarmAnimal lastTarget;

    protected override void CalculateSteeringForces()
    {
        switch(currentState)
        {
            case WolfState.Wandering:
            {
                Wander();
                GetComponent<SpriteRenderer>().color = Color.white;
                break;
            }
            case WolfState.Attacking:
            {
                GetComponent<SpriteRenderer>().color = Color.red;
                Pursue(target, 1.5f);
                break;
            }
        }

        foreach(FarmAnimal farmAnimal in AgentManager.Instance.farmAnimals)
        {
            float sqrDistance = Vector2.SqrMagnitude(farmAnimal.physicsObject.Position - physicsObject.Position);

            if(sqrDistance <= visionRange)
            {
                target = farmAnimal;
                if(target != lastTarget && lastTarget != null)
                {
                    lastTarget.attacker = null;
                }
                lastTarget = target;
                farmAnimal.attacker = this;
                currentState = WolfState.Attacking;
            }
            else
            {
                if(farmAnimal == target)
                {
                    target.attacker = null;
                    target = null;
                    currentState = WolfState.Wandering;
                }
            }
        }

        AvoidAllObstacles();
        Separate(AgentManager.Instance.wolves);
        StayInBounds(2f);
    }
}
