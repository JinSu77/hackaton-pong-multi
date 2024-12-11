using UnityEngine;
using Pong.Constants;
using Pong.Core;
using Pong.Core.Data;

/// <summary>
/// Synchronizes the right paddle state on the server.
/// </summary>
public class PaddleRightSyncServer : MonoBehaviour
{
    private ServerManager ServerMan;
    private float NextUpdateTimeout = -1;

    void Awake()
    {
        if (!Globals.IsServer)
        {
            enabled = false;
        }
    }

    void Start()
    {
        ServerMan = FindFirstObjectByType<ServerManager>();
    }

    void Update()
    {
        if (Time.time > NextUpdateTimeout)
        {
            PaddleState state = new PaddleState { Position = transform.position };
            string json = JsonUtility.ToJson(state);

            if (ServerMan != null)
            {
                ServerMan.BroadcastUDPMessage($"{MessageType.PaddleRightUpdate}|{json}");
            }

            NextUpdateTimeout = Time.time + 0.03f;
        }
    }
}
