using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickupSpawnController : NetworkBehaviour
{
    public List<GameObject> spawners;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject spawner in spawners)
        {
            //Instantiate(spawner, spawner.gameObject.transform.position, Quaternion.identity, gameObject.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
