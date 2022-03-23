using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerScore : NetworkBehaviour
{

    [SyncVar]
    public int kills;

    [SyncVar]
    public int deaths;

    [SyncVar]
    public float kd;

    public ScoreBoardPlayer scoreBoardPlayer;
    public ScoreBoard scoreBoard;

    public GameObject[] scoreBoardPlayers;




    // Start is called before the first frame update
    void Start()
    {
        scoreBoard = GameObject.Find("Scoreboard(Clone)").GetComponent<ScoreBoard>();
        scoreBoard.playerObjects.Add(gameObject);
        scoreBoard.UpdateScoreboard(GetComponent<CustomAttributes>().userName, this.gameObject);

    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                scoreBoard.panel.SetActive(true);

                UpdateScoreBoardText();

                scoreBoard.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                scoreBoard.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                scoreBoard.panel.SetActive(false);
            }
        }
    }

    public void UpdateScoreBoardText()
    {
        scoreBoardPlayers = GameObject.FindGameObjectsWithTag("ScoreBoardPlayer");

        if (scoreBoardPlayers.Length > 0)
        {
            foreach (GameObject player in scoreBoardPlayers)
            {
                Debug.Log(player);
                if (player.gameObject != null)
                {
                    player.GetComponent<ScoreBoardPlayer>().SetPlayerKills(player.GetComponent<ScoreBoardPlayer>().playerObject.GetComponent<PlayerScore>().kills);
                    player.GetComponent<ScoreBoardPlayer>().SetPlayerDeaths(player.GetComponent<ScoreBoardPlayer>().playerObject.GetComponent<PlayerScore>().deaths);
                }
            }
        }
    }


    public void AddKill()
    {
        kills += 1;
        UpdateScoreBoardText();
    }

    public void AddDeath()
    {
        deaths += 1;
        UpdateScoreBoardText();
    }

    private void OnDestroy()
    {
        //NetworkServer.Destroy(scoreBoardPlayer);
    }
}
