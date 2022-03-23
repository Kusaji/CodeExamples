using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Collections;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

// NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.

public class PlayerRespawner : NetworkBehaviour
{
	public List<GameObject> respawnPoints;
	public float respawnTime;

	public GameObject GetRandomRespawnPoint()
    {
		var respawnPoint = respawnPoints[Random.Range(0, respawnPoints.Count)];
		return respawnPoint;
    }

	



	public void RespawnPlayer(GameObject player)
    {
		CmdRespawnPlayer(player);
	}

	
	void CmdRespawnPlayer(GameObject player)
    {
		StartCoroutine(RespawnPlayerRoutine(player));
    }

	IEnumerator RespawnPlayerRoutine(GameObject player)
    {
		player.GetComponent<MeshRenderer>().enabled = false;
		yield return new WaitForSeconds(respawnTime);
		player.GetComponent<PlayerHealth>().InitPlayerHealth();
		player.gameObject.transform.position = GetRandomRespawnPoint().transform.position;
		player.GetComponent<MeshRenderer>().enabled = true;
	}


	public void HideMesh(GameObject player, bool isHidden)
	{
		RPCHideMesh(player, isHidden);
	}

	[ClientRpc]
	public void RPCHideMesh(GameObject player, bool isHidden)
    {
		player.GetComponent<MeshRenderer>().enabled = isHidden;
    }

}
