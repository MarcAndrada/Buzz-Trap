using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGrass : MonoBehaviour
{
    public GameObject grassPrefab;
    public Terrain terrain;
    public int numberOffGrassObjects = 10000;
    public float height = 1.0f;

    void Start()
    {
        SpawnDetails();
    }

    private void SpawnDetails()
    {
        for (int i = 0; i < numberOffGrassObjects; i++)
        {
            float x = Random.Range(0, terrain.terrainData.size.x);
            float z = Random.Range(0, terrain.terrainData.size.z);
            float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.transform.position.y;

            Vector3 position = new Vector3(x, y, z);
            Instantiate(grassPrefab, position, Quaternion.identity);

        }
    }
}
