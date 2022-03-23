using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    [SerializeField] float mouseSensitivity = 100f;
    public float clampAngle = 80.0f;
    [SerializeField] float mouseX;
    [SerializeField] float mouseY;
    [SerializeField] float xRotation;

    [SerializeField] Transform playerBody;
    [SerializeField] Rigidbody playerRB;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Camera playerCamera;
    [SerializeField] bool isZoomed;

    [SerializeField] GameObject lookAtTarget;
    [SerializeField] Transform torsoPivotBone;
    [SerializeField] Vector3 TorsoLookAtOffset;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis

    // Start is called before the first frame update
    void Start()
    {

        cameraTransform = GameObject.Find("Camera").transform;
        playerCamera = Camera.main;
        playerRB = GetComponent<Rigidbody>();

        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) { return; }


        if (Input.GetMouseButtonDown(1))
        {
            Zoom();
        }

        if (Input.GetMouseButtonUp(1))
        {
            Zoom();
        }


        //Get mouse Inputs
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
        /*lookAtTarget.transform.position = transform.position + new Vector3(0, -xRotation / 100, 5);*/

    }

    private void LateUpdate()
    {
/*        torsoPivotBone.LookAt(lookAtTarget.transform.position);
        torsoPivotBone.Rotate(TorsoLookAtOffset);*/
    }

    void Zoom ()
    {
        if (!isZoomed)
        {
            isZoomed = true;
            playerCamera.fieldOfView = 30f;
        } 
        else
        {
            isZoomed = false;
            playerCamera.fieldOfView = 80f;
        }
    }
}
