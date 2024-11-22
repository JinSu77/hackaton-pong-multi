using UnityEngine;
using System.Net;

[System.Serializable]
public class PaddleRightState {
    public Vector3 Position;
}

public class PaddleRightSyncServer : MonoBehaviour
{
    ServerManager ServerMan;
    float NextUpdateTimeout = -1;

    void Awake() {
        if (!Globals.IsServer) {
            enabled = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ServerMan = FindFirstObjectByType<ServerManager>();
        Debug.Log("[SERVER] PaddleRightSyncServer started");
        
        ServerMan.UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
            Debug.Log($"[SERVER] Received message: {message}");
            
            if (!message.StartsWith("PADDLE_RIGHT_UPDATE")) { return; }

            string[] tokens = message.Split('|');
            string json = tokens[1];
            PaddleRightState state = JsonUtility.FromJson<PaddleRightState>(json);
            
            transform.position = state.Position;
            Debug.Log($"[SERVER] Updated paddle position to: {state.Position}");
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > NextUpdateTimeout) {
            PaddleRightState state = new PaddleRightState {
                Position = transform.position
            };

            string json = JsonUtility.ToJson(state);
            string message = "PADDLE_RIGHT_UPDATE|" + json;
            Debug.Log($"[SERVER] Broadcasting position: {message}");
            ServerMan.BroadcastUDPMessage(message);

            NextUpdateTimeout = Time.time + 0.03f;
        }
    }
}
