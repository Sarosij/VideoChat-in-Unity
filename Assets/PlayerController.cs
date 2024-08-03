using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    Rigidbody2D rb;
    Vector2 directionInput;
    [SerializeField] float speed = 10;
    private joinChannelVideo _videoChat;
    private int collisionCount = 0;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void Spawned()
    {
        // Find the joinChannelVideo script on the canvas
        _videoChat = FindObjectOfType<joinChannelVideo>();
        if (_videoChat == null) {
            Debug.LogError("joinChannelVideo script not found in the scene!");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            directionInput = data.direction;
        }
        else
        {
            directionInput = Vector2.zero;
        }

        rb.velocity = directionInput * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collisionCount++;
            if (collisionCount == 1) // First collision
            {
                Debug.Log("Player collision detected. Joining video chat...");
                RPC_JoinVideoChat();
                collision.gameObject.GetComponent<PlayerController>().RPC_JoinVideoChat();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collisionCount--;
            if (collisionCount == 0) // No more collisions
            {
                Debug.Log("Player collision ended. Leaving video chat...");
                RPC_LeaveVideoChat();
                collision.gameObject.GetComponent<PlayerController>().RPC_LeaveVideoChat();
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_JoinVideoChat()
    {
        if (_videoChat != null) {
            _videoChat.Join();
        } else {
            Debug.LogError("joinChannelVideo script reference is null!");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_LeaveVideoChat()
    {
        if (_videoChat != null) {
            _videoChat.Leave();
        } else {
            Debug.LogError("joinChannelVideo script reference is null!");
        }
    }
}