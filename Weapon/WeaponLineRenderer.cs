using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponLineRenderer : NetworkBehaviour
{
    public LineRenderer lineRenderer;
    public GameObject bulletTrailStart;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetMouseButtonDown(0))
            {

            }
        }
    }

    public void DrawBulletTrail(Vector3 end)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, bulletTrailStart.transform.position);
        lineRenderer.SetPosition(0, end);
    }
}
