using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmAnimal : Agent
{
    protected override void CalculateSteeringForces()
    {
        Wander();
        StayInBounds(3f);
        Separate(AgentManager.Instance.farmAnimals);
        AvoidAllObstacles();
    }
}
