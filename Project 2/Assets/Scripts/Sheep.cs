using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : FarmAnimal
{
    protected override void CalculateSteeringForces()
    {
        base.CalculateSteeringForces();
        if (CurrentState == AnimalState.Wandering)
        {
            Flock(AgentManager.Instance.farmAnimals.FindAll(a => a is Sheep), 2, 5);
        }
    }
}
