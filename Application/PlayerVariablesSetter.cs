using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerVariablesSetter : MonoBehaviour
{
    public CustomRoomPlayer _roomPlayer;
    public InputField userName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            _roomPlayer.userName = userName.text;
        }
    }
}
