using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class JumpPad : NetworkBehaviour
{
    [Header("Jump Pad Controls")]
    [Space(5)]
    [Tooltip("Set force per vector and the player will be sent in that direction.")]
    [SyncVar]
    public Vector3 padForce;

    public bool jumpPadReady;

    [Space(10)]

    [Tooltip("How long will this jumpPad bypass the players input + character control scheme.")]
    [SyncVar]
    public float inputBlockTime;

    [Space(10)]
    [Header("Audio Effect Components")]
    [SerializeField] private AudioSource speaker;
    [SerializeField] private AudioClip jumpPadSound;

    #region Jump Pad Logic

    private void Start()
    {
        jumpPadReady = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && jumpPadReady)
        {
            //Fetch necessary components on player
            var playerRb = other.gameObject.GetComponent<Rigidbody>();
            var player = other.gameObject.GetComponent<PlayerFPSMovement>();

            StartCoroutine(JumpPadRoutine(playerRb, player));
        }
    }


    IEnumerator JumpPadRoutine(Rigidbody playerRb, PlayerFPSMovement player)
    {
        //Set JumpPad State
        jumpPadReady = false;

        //Stop rigidbody, set it to Kinematic
        playerRb.velocity = new Vector3(0, 0, 0);
        playerRb.isKinematic = true;

        //Block inputs so jump can't be messed with
        player.JumpPadInputBlock(inputBlockTime);
        
        //Wait a small moment to have player rigidbody behave
        yield return new WaitForSeconds(0.2f);

        //Send the player flying
        playerRb.isKinematic = false;
        playerRb.AddForce(padForce.x, padForce.y, padForce.z, ForceMode.Impulse);

        //Play Jump Pad Sound Effect
        CmdPlaySound();

        //Wait a second for jump pad to "Re-Arm"
        yield return new WaitForSeconds(1f);
        jumpPadReady = true;
    }
    #endregion

    #region Jump Pad Audio

    [Command(requiresAuthority = false)]
    void CmdPlaySound()
    {
        RpcSyncAudio();
    }

    [ClientRpc]
    void RpcSyncAudio()
    {
        speaker.PlayOneShot(jumpPadSound, 0.30f);
    }

    #endregion
}
