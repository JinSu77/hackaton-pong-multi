using System.Net;
using UnityEngine;

public class PaddleLeftSyncClient : MonoBehaviour
{
    UDPService UDP;
    PongInput input;
    float moveSpeed = 5f;
    ClientManager clientManager;

    void Awake() {
        if (Globals.IsServer) {
            enabled = false;
        }
        input = new PongInput();
        input.Enable();
    }

    void Start()
    {
        UDP = FindFirstObjectByType<UDPService>();
        clientManager = FindFirstObjectByType<ClientManager>();

        UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
            if (!message.StartsWith("PADDLE_LEFT_UPDATE")) { return; }

            string[] tokens = message.Split('|');
            string json = tokens[1];
            PaddleLeftState state = JsonUtility.FromJson<PaddleLeftState>(json);
            
            transform.position = state.Position;
        };
    }

    void Update()
    {
        float movement = input.Pong.Player1.ReadValue<float>();
        
        if (movement != 0) {
            Debug.Log($"[CLIENT] Movement detected: {movement}");

            Vector3 newPosition = transform.position;
            newPosition.y += movement * moveSpeed * Time.deltaTime;
            transform.position = newPosition;

            PaddleLeftState state = new PaddleLeftState {
                Position = newPosition
            };
            string json = JsonUtility.ToJson(state);
            string message = "PADDLE_LEFT_MOVE|" + json;
            Debug.Log($"[CLIENT] Sending message to server: {message}");
            UDP.SendUDPMessage(message, clientManager.ServerEndpoint);
        }
    }
}
