using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public static ObstacleManager Instance;
    public Obstacle hayBalePrefab;

    public List<Obstacle> Obstacles = new List<Obstacle>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void AddObstacle(Vector3 position)
    {
        Obstacles.Add(Instantiate(hayBalePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity));
    }
}
