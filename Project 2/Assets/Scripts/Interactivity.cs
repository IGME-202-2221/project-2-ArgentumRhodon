using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactivity : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ObstacleManager.Instance.AddObstacle(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}
