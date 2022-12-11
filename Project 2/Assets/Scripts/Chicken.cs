using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : FarmAnimal
{
    protected override void CalculateSteeringForces()
    {
        base.CalculateSteeringForces();
        if(CurrentState == AnimalState.Wandering)
        {
            Flock(AgentManager.Instance.farmAnimals.FindAll(a => a is Chicken), 2, 5);
        }
    }
}
