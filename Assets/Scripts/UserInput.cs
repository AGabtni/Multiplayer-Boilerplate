using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class UserInput : NetworkBehaviour
{
    CharacterController controller;
    // Start is called before the first frame update
    //[Client]
    float horInput, vertInput;
    void Start()
    {

        //if (!hasAuthority) return;
        controller = GetComponent<CharacterController>();
        if (this.isLocalPlayer)
        {

            controller.enabled = true;
            controller.CreateCamera();
        }
        else
        {
            controller.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.isLocalPlayer) return;

        horInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");
        //1- Move Command
        if (horInput != 0 || vertInput != 0)
            controller.MoveStep(horInput, vertInput);

        //2- Jump Command
        if (Input.GetButtonDown("Jump"))
            controller.Jump();

        if(Input.GetKeyDown(KeyCode.Escape))
            controller.TriggerCursorLock();
    }

    [Command]
    void CmdMove()
    {
        controller.MoveStep(horInput, vertInput);
        // RpcMove();
    }
    [Command]
    void CmdJump()
    {
        controller.Jump();
        // RpcMove();
    }

    //[ClientRPC]
    //private void RpcMove() => 
}
