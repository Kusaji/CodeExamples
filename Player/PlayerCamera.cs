using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerCamera : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField]Camera mainCam;
    [SerializeField] Vector3 _cameraOffset;
    [SerializeField] GameObject player;
    [SerializeField] SkinnedMeshRenderer playerMesh;

    void Awake()
    {
        mainCam = Camera.main;
    }

    public override void OnStartLocalPlayer()
    {
        if (mainCam != null)
        {
            // configure and make camera a child of player with 3rd person offset
            /*            mainCam.orthographic = false;*/
            mainCam.transform.SetParent(transform);
            mainCam.transform.localPosition = _cameraOffset;
            /*            mainCam.transform.localEulerAngles = new Vector3(10f, 0f, 0f);
                        mainCam.transform.LookAt(gameObject.transform.position);*/
        }

        if (isLocalPlayer)
        {
            playerMesh.enabled = false;
        }
    }

    public override void OnStopClient()
    {
        if (isLocalPlayer && mainCam != null)
        {
            mainCam.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(mainCam.gameObject, SceneManager.GetActiveScene());
            /*            mainCam.orthographic = true;
                        mainCam.transform.localPosition = new Vector3(0f, 70f, 0f);
                        mainCam.transform.localEulerAngles = new Vector3(90f, 0f, 0f);*/
        }
    }
}