using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickupSpawner : NetworkBehaviour
{
    [SyncVar]
    public bool spawning;
    [SyncVar]
    public int respawnTime;

    [SyncVar]
    public bool initialSpawn;

    public GameObject spawnedItem;
    public GameObject spawnLocation;
    public GameObject pickupPrefab;

    [Space(10)]
    [Header("Audio Effect Components")]
    [SerializeField] private AudioSource speaker;
    [SerializeField] public List<AudioClip> sounds;



    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(WaitForFirstPlayerRoutine());
    }

    #region Pickup Spawn Logic

    IEnumerator WaitForFirstPlayerRoutine()
    {
        while (!initialSpawn)
        {
            var players = GameObject.FindGameObjectsWithTag("Player");

            if (players.Length >= 1 && !initialSpawn) //If game is started by a host
            {
                if (!isServer)
                {
                    CmdSpawnPickup();
                    initialSpawn = true;
                }
                else
                {
                    ServerSpawnPickup();
                    initialSpawn = true;
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void SpawnPickupRoutine()
    {
        StartCoroutine(SpawnRoutine());
    }

    [Command (requiresAuthority = false)]
    public void CmdSpawnPickup()
    {
        Debug.Log("Spawning Health Pickup");
        spawnedItem = Instantiate(pickupPrefab, spawnLocation.transform.position, Quaternion.identity);
        spawnedItem.GetComponent<HealthPickup>().mySpawner = this;

        if (!isServer)
        {
            CmdPlaySound(0);
        }
        else
        {
            RpcSyncAudio(0);
        }

        NetworkServer.Spawn(spawnedItem);
        spawning = false;
    }

    [Server]
    public void ServerSpawnPickup()
    {
        spawnedItem = Instantiate(pickupPrefab, spawnLocation.transform.position, Quaternion.identity);
        spawnedItem.GetComponent<HealthPickup>().mySpawner = this;
        RpcSyncAudio(0);

        NetworkServer.Spawn(spawnedItem);
        spawning = false;
    }

    IEnumerator SpawnRoutine()
    {
        spawning = true;
        yield return new WaitForSeconds(respawnTime);
        if (!isServer)
        {
            CmdSpawnPickup();
        }
        else if (isServer)
        {
            ServerSpawnPickup();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            spawnedItem = other.gameObject;
        }
    }
    #endregion

    #region Audio Logic

    [Command(requiresAuthority = false)]
    public void CmdPlaySound(int clipId)
    {
        RpcSyncAudio(clipId);
    }

    [ClientRpc]
    public void RpcSyncAudio(int clipId)
    {
        speaker.PlayOneShot(sounds[clipId], 0.5f);
    }

    #endregion
}
