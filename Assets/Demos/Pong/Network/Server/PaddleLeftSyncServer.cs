using UnityEngine;
using System.Net;

public class PaddleLeftSyncServer : MonoBehaviour
{
    ServerManager ServerMan;
    float NextUpdateTimeout = -1;

    void Awake() {
        if (!Globals.IsServer) {
            enabled = false;
        }
    }

    void Start()
    {
        ServerMan = FindFirstObjectByType<ServerManager>();
        Debug.Log("[SERVER] PaddleLeftSyncServer started");
        
        ServerMan.UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
            Debug.Log($"[SERVER] Received message: {message}");
            
            if (!message.StartsWith("PADDLE_LEFT_MOVE")) { return; }

            string[] tokens = message.Split('|');
            string json = tokens[1];
            PaddleLeftState state = JsonUtility.FromJson<PaddleLeftState>(json);
            
            transform.position = state.Position;
            Debug.Log($"[SERVER] Updated paddle position to: {state.Position}");
        };
    }

    void Update()
    {
        if (Time.time > NextUpdateTimeout) {
            PaddleLeftState state = new PaddleLeftState{
                Position = transform.position
            };

            string json = JsonUtility.ToJson(state);
            string message = "PADDLE_LEFT_UPDATE|" + json;
            Debug.Log($"[SERVER] Broadcasting position: {message}");
            ServerMan.BroadcastUDPMessage(message);

            NextUpdateTimeout = Time.time + 0.03f;
        }
    }
}
