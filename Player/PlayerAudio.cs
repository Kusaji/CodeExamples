using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAudio : NetworkBehaviour
{
    [SerializeField] private AudioSource speaker;
    [SerializeField] private List<AudioClip> gunSounds;


    // Start is called before the first frame update
    void Start()
    {
        speaker = this.GetComponent<AudioSource>();
    }

    [Command]
    public void CmdPlaySound(int clipId)
    {
        RpcSyncAudio(clipId);
    }

    [ClientRpc]
    public void RpcSyncAudio(int clipId)
    {
        speaker.PlayOneShot(gunSounds[clipId], 0.3f);
    }
    
}
