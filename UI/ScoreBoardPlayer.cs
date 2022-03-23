using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ScoreBoardPlayer : NetworkBehaviour
{
    //Variables
    [SyncVar]
    public string playerName;

    [SyncVar]
    public int playerKills;

    [SyncVar]
    public int playerDeaths;

    public GameObject playerObject;
    public Text nameText;
    public Text killsText;
    public Text deathsText;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    }

    //Initialize Player

    public void InitializePlayer(string name, GameObject _playerObject)
    {
        CmdInitializePlayer(name);
        playerName = name;
        nameText.text = name;
        killsText.text = $"{playerKills}";
        deathsText.text = $"{playerDeaths}";
        playerObject = _playerObject;
    }

    [Command(requiresAuthority = false)]
    public void CmdInitializePlayer(string name)
    {
        playerName = name;
        playerKills = 0;
        playerDeaths = 0;
    }

    //Syncvar Hooks
    

    public void SetPlayerKills(int kills)
    {
        killsText.text = $"{kills}";
    }

    public void SetPlayerDeaths(int deaths)
    {
        playerDeaths += 1;
        deathsText.text = $"{deaths}";
    }
}
