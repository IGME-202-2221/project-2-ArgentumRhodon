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

        for(int i = 0; i < numFarmAnimals; i++)
        {
            FarmAnimal randomFarmAnimalPrefab = farmAnimalPrefabs[(int)(Random.value * farmAnimalPrefabs.Count)];
            farmAnimals.Add(Spawn(randomFarmAnimalPrefab));
        }
    }

    private T Spawn<T>(T prefabToSpawn) where T : Agent
    {
        float xPos = Random.Range(minPosition.x, maxPosition.x);
        float yPos = Random.Range(minPosition.y, maxPosition.y);

        return Instantiate(prefabToSpawn, new Vector3(xPos, yPos), Quaternion.identity);
    }
}
