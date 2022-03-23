using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PhysicsPickupObject : NetworkBehaviour
{
    public Vector3 offset;
    [SyncVar]
    public GameObject targetObject;

    private Rigidbody rb;

    public override void OnStartServer()
    {
        base.OnStartServer();
        offset = new Vector3(0f, 0f, 2f);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObject != null)
        {
            transform.position = targetObject.transform.TransformDirection(Vector3.forward) + offset;
        }
    }

    [ClientRpc]
    public void SetTarget(GameObject target)
    {
        targetObject = target;
        rb.isKinematic = true;
    }

    [ClientRpc]
    public void ClearTarget()
    {
        targetObject = null;
        rb.isKinematic = false;
    }


}
