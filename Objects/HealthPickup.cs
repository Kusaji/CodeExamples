using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HealthPickup : NetworkBehaviour
{
    [Header("Health Pack Settings")]
    [SyncVar]
    public float healAmount;
    [SyncVar]
    public PickupSpawner mySpawner;
    
    private Vector3 spawnPos;

    // Start is called before the first frame update
    void Start()
    {
        spawnPos = transform.position;
        if (isServer)
        {
            CheckForSpawner();
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 50f * Time.deltaTime, 0f);
        transform.position = new Vector3(spawnPos.x, spawnPos.y + Mathf.PingPong(Time.time / 4, 0.25f), spawnPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth.currentHealth != playerHealth.maxHealth)
            {
                other.GetComponent<PlayerHealth>().AddHealth(healAmount);
                mySpawner.SpawnPickupRoutine();
                if (!isServer)
                {
                    mySpawner.CmdPlaySound(1);
                }
                else
                {
                    mySpawner.RpcSyncAudio(1);
                }
                CmdDestroyPickup();
            }
        }
    }

    [Command(requiresAuthority = false)]
    void CmdDestroyPickup()
    {
        NetworkServer.Destroy(gameObject);
    }

    void CheckForSpawner()
    {
        Collider[] hitCoolliders = Physics.OverlapSphere(transform.position, 5);
        foreach (var hitCollider in hitCoolliders)
        {
            
            if (hitCollider.gameObject.CompareTag("Spawner"))
            {
                //Debug.Log(hitCollider.gameObject);
                mySpawner = hitCollider.gameObject.GetComponent<PickupSpawner>();
            }
        }
    }
}
