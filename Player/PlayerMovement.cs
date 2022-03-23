using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed;
    public float maxMoveSpeed;
    public float currentMoveSpeed;

    [Header("Inputs")]
    [SerializeField] Vector3 _playerInput;


    [Header("Components / Gameobjects")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] Transform _camera;
    [SerializeField] PlayerGroundChecker _groundChecker;


    #region Callbacks
    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            PlayerInput();

            if (_groundChecker.isGrounded)
            {

            }
        }
    }

    private void Start()
    {
        if (isLocalPlayer)
        {

        }
    }
    #endregion

    #region Methods
    void PlayerInput()
    {
        _playerInput.x = Input.GetAxis("Horizontal");
        _playerInput.y = Input.GetAxis("Jump");
        _playerInput.z = Input.GetAxis("Vertical");


    }

    void AddForces()
    {

    }
    #endregion
}
