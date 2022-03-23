using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CustomAttributes : NetworkBehaviour
{
    [SyncVar]
    public string hello;

    [SyncVar(hook = nameof(UpdateNameText))]
    public string userName;

    [SerializeField] TextMesh usernameText;

    // Start is called before the first frame update
    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            gameObject.name = userName;
            usernameText.text = userName;
        }
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            gameObject.name = userName;
            usernameText.text = userName;
        }
    }

    private void UpdateNameText(string oldString, string newString)
    {
        usernameText.text = newString;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
