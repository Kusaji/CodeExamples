using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IPFieldUpdater : MonoBehaviour
{
    /*    [SerializeField] custom _roomManager;*/
    [SerializeField] CustomAuthenticator authenticator;
    [SerializeField] TestNetworkManager networkManager;
    [SerializeField] InputField _serverIPField;
    [SerializeField] InputField usernameField;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            networkManager.networkAddress = _serverIPField.text;
            authenticator.playerName = usernameField.text;

            if (_serverIPField.text == "")
            {
                _serverIPField.text = "localhost";
            }

        }
    }
}
