using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public static AgentManager Instance;

    public List<FarmAnimal> farmAnimalPrefabs;
    public int numFarmAnimals = 10;
    [HideInInspector]
    public List<FarmAnimal> farmAnimals = new List<FarmAnimal>();

    public Wolf wolfPrefab;
    public int numWolves = 1;
    [HideInInspector]
    public List<Wolf> wolves = new List<Wolf>();

    [HideInInspector]
    public Vector2 maxPosition = Vector2.one;
    [HideInInspector]
    public Vector2 minPosition = -Vector2.one;

    public float edgePadding = 1f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        Camera cam = Camera.main;

        if(cam != null)
        {
            Vector3 camPosition = cam.transform.position;

            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;

            maxPosition.x = camPosition.x + halfWidth - edgePadding;
            maxPosition.y = camPosition.y + halfHeight - edgePadding;

            minPosition.x = camPosition.x - halfWidth + edgePadding;
            minPosition.y = camPosition.y - halfHeight + edgePadding;
        }

        // Spawn the farm animals
        for(int i = 0; i < numFarmAnimals; i++)
        {
            FarmAnimal randomFarmAnimalPrefab = farmAnimalPrefabs[(int)(Random.value * farmAnimalPrefabs.Count)];
            farmAnimals.Add(Spawn(randomFarmAnimalPrefab));
        }

        // Spawn the hostile wolves
        for(int i = 0; i < numWolves; i++)
        {
            wolves.Add(Spawn(wolfPrefab));
        }
    }

    private T Spawn<T>(T prefabToSpawn) where T : Agent
    {
        float xPos = Random.Range(minPosition.x, maxPosition.x);
        float yPos = Random.Range(minPosition.y, maxPosition.y);

        return Instantiate(prefabToSpawn, new Vector3(xPos, yPos), Quaternion.identity);
    }
}
