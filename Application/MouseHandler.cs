using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class MouseHandler : NetworkBehaviour
{
    [SerializeField] string _currentScene;
    [SerializeField] bool isPaused;
    // Start is called before the first frame update

    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            _currentScene = SceneManager.GetActiveScene().name;

            if (_currentScene == "Offline")
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (_currentScene == "MultiplayerLobbyRoom")
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowCursor();
        }
    }

    void ShowCursor()
    {
        if (!isPaused)
        {
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (isPaused)
        {
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
