using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SocketConnectionManager;

public class AnimationCustomController : SocketConnectionManager
{
    Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        /* while (playerUpdates.TryDequeue(out var playerUpdate))
        {

        } */
        // if J is pressed is true
        // bool JPressed = Input.GetKey(KeyCode.J);

        // missing approvedAction from SocketManager

        // Both need to be true in order to trigger the animation

        // animator.SetBool("ApprovedAttack", JPressed);
    }
}
