using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ScoreBoard : NetworkBehaviour
{
    public List<GameObject> playerObjects;
    public GameObject panel;
    public Transform playerScoreTransform;
    public GameObject scorePlayerPrefab;

    //public Text scoreBoardText;
    
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
        gameObject.transform.SetParent(GameObject.Find("PlayerUI").transform);
        gameObject.transform.position = new Vector3(0, 0, 0);
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        StartCoroutine(UpdateScoreBoardRoutine());
    }

    public void UpdateScoreboard(string name, GameObject player)
    {
        //Create Object
        GameObject playerScore = Instantiate(scorePlayerPrefab);
        playerScore.transform.SetParent(playerScoreTransform);
        playerScore.name = name;
        playerScore.transform.localScale = new Vector3(1, 1, 1);

        //Get ScoreBoardPlayer script
        var scoreBoardPlayer = playerScore.GetComponent<ScoreBoardPlayer>();
        scoreBoardPlayer.InitializePlayer(name, player);

        player.GetComponent<PlayerScore>().scoreBoardPlayer = scoreBoardPlayer;
    }

    public void CreateScoreBoardPlayer(string playerName)
    {
        CmdCreateScoreBoardPlayer(playerName);
    }

    [Command]
    public void CmdCreateScoreBoardPlayer(string playerName)
    {

    }

    public void RpcUpdateScores()
    {
        for (int i = 0; i < playerObjects.Count; i++)
        {
            if (playerObjects[i] != null)
            {
                //var playerObject = playerObjects[i].GetComponent<ScoreBoardPlayer>();
                var playerObject = playerObjects[i];
                var scoreBoardPlayer = playerObject.GetComponent<PlayerScore>().scoreBoardPlayer;
                scoreBoardPlayer.playerKills = playerObjects[i].GetComponent<PlayerScore>().kills;
                scoreBoardPlayer.playerDeaths = playerObjects[i].GetComponent<PlayerScore>().deaths;
                scoreBoardPlayer.killsText.text = $"{scoreBoardPlayer.playerKills}";
                scoreBoardPlayer.deathsText.text = $"{scoreBoardPlayer.playerDeaths}";
            }
        }
    }

    IEnumerator UpdateScoreBoardRoutine()
    {
        yield return new WaitForSeconds(1); //Initial wait for players components to initialize.
        while (gameObject)
        {
            RpcUpdateScores();
            yield return new WaitForSeconds(10);
        }
    }
}
