using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerPrefabSpawner : NetworkBehaviour
{
    public GameObject scoreBoard;
    public GameObject bulletPrefab;
    public Transform scoreboardParent;
    private void Start()
    {
        if (isServer)
        {
            Debug.Log("Spawner is server");
            var instantiatedScoreboard = Instantiate(scoreBoard, scoreboardParent);
            NetworkServer.Spawn(instantiatedScoreboard);
        }
    }

/*    public void SpawnBullet(Vector3 activemuzzle, Vector3 viewmodel)
    {
        if (isServer)
        {
            ServerSpawnBullet(activemuzzle, viewmodel);
            Debug.Log("RPCSpawnBullet Called");
        }
        else
        {
            Debug.Log("CmdSpawnBullet Called");
            CmdSpawnBullet(activemuzzle, viewmodel);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSpawnBullet(Vector3 activemuzzle, Vector3 viewmodel)
    {

        var bullet = Instantiate(bulletPrefab, activemuzzle, Quaternion.Euler(viewmodel));
        NetworkServer.Spawn(bullet);

    }

    [Server]
    public void ServerSpawnBullet(Vector3 activemuzzle, Vector3 viewmodel)
    {

        var bullet = Instantiate(bulletPrefab, activemuzzle, Quaternion.Euler(viewmodel));
        NetworkServer.Spawn(bullet);

    }*/
}
