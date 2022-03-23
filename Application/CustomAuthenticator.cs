using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/*
    Documentation: https://mirror-networking.gitbook.io/docs/components/network-authenticators
    API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkAuthenticator.html
*/

public class CustomAuthenticator : NetworkAuthenticator
{
    #region Messages

    public List<GameObject> spawnableObjects;
    public PlayerRespawner respawn;
    public string playerName;


    public struct AuthRequestMessage : NetworkMessage 
    {
        public string authUsername;
    }

    public struct AuthResponseMessage : NetworkMessage 
    {
        public byte code;
        public string message;
    }

    public struct CreatePlayer : NetworkMessage
    {
        public string name;
    }


    #endregion

    #region Server
    /// <summary>
    /// Called on server from StartServer to initialize the Authenticator
    /// <para>Server message handlers should be registered in this method.</para>
    /// </summary>
    public override void OnStartServer()
    {
        // register a handler for the authentication request we expect from client
        NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
        NetworkServer.RegisterHandler<CreatePlayer>(OnCreateCharacter, false);
    }

    /// <summary>
    /// Called on server from OnServerAuthenticateInternal when a client needs to authenticate
    /// </summary>
    /// <param name="conn">Connection to client.</param>
    public override void OnServerAuthenticate(NetworkConnection conn) { }

    /// <summary>
    /// Called on server when the client's AuthRequestMessage arrives
    /// </summary>
    /// <param name="conn">Connection to client.</param>
    /// <param name="msg">The message payload</param>
    public void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage msg)
    {
        AuthResponseMessage authResponseMessage = new AuthResponseMessage();

        conn.Send(authResponseMessage);

        // Accept the successful authentication
        ServerAccept(conn);


    }

    void OnCreateCharacter(NetworkConnection conn, CreatePlayer message)
    {
        respawn = GameObject.Find("PlayerRespawner").GetComponent<PlayerRespawner>();

        // playerPrefab is the one assigned in the inspector in Network
        GameObject playerObject = Instantiate(spawnableObjects[0], respawn.GetRandomRespawnPoint().transform.position, Quaternion.identity);
        //Grab component to write message attributes too
        CustomAttributes playerAttributes = playerObject.GetComponent<CustomAttributes>();
        playerAttributes.userName = message.name;
        playerObject.name = playerAttributes.userName;

        //Spawn Player with custom attributes
        NetworkServer.AddPlayerForConnection(conn, playerObject);
    }

    #endregion

    #region Client

    /// <summary>
    /// Called on client from StartClient to initialize the Authenticator
    /// <para>Client message handlers should be registered in this method.</para>
    /// </summary>
    public override void OnStartClient()
    {
        // register a handler for the authentication response we expect from server
        NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
    }

    /// <summary>
    /// Called on client from OnClientAuthenticateInternal when a client needs to authenticate
    /// </summary>
    public override void OnClientAuthenticate()
    {
        AuthRequestMessage authRequestMessage = new AuthRequestMessage();
        NetworkClient.Send(authRequestMessage);

        CreatePlayer characterMessage = new CreatePlayer
        {
            name = playerName,
        };

        NetworkClient.Send(characterMessage);
    }

    /// <summary>
    /// Called on client when the server's AuthResponseMessage arrives
    /// </summary>
    /// <param name="msg">The message payload</param>
    public void OnAuthResponseMessage(AuthResponseMessage msg)
    {
        // Authentication has been accepted
        ClientAccept();
    }

    #endregion
}
