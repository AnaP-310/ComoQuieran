using UnityEngine;

public class CubePool : MonoBehaviour
{

    public GameObject cubePrefab;

    private GameObject[] cubes = new GameObject[43];
    private float[] returnIn = new float[43];
    private float nextSpawn;

    void Start()
    {
        //Prepare the cubes
        for(int i = 0; i < cubes.Length; i++)
        {
            cubes[i] = Instantiate(cubePrefab, transform);
            cubes[i].SetActive(false);
        }    
    }

    // Update is called once per frame
    void Update()
    {
        //Return. Deactivate the cubes
        for( int i = 0; i < cubes.Length; i++)
        {
            if(cubes[i].activeSelf && Time.time >= returnIn[i])
            {
                cubes[i].SetActive(false);
            }
        }

        //Spawn
        if(Time.time >= nextSpawn)
        {
            Spawn();
            nextSpawn = Time.time + 1f;
        }

    }

    void Spawn()
    {
        for( int i = 0; i < cubes.Length; i++)
        {
            if(!cubes[i].activeSelf)
            {
                cubes[i].transform.position = new Vector3(0f, Random.Range(-5f, 5f), 0f);
                cubes[i].SetActive(true);
                returnIn[i] = Time.time + 6f;
                return;
            }
        }
    }
}
