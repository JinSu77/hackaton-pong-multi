using System.Net;
using UnityEngine;

public class PaddleRightSyncClient : MonoBehaviour
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UDP = FindFirstObjectByType<UDPService>();
        clientManager = FindFirstObjectByType<ClientManager>();

        UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
            if (!message.StartsWith("PADDLE_RIGHT_UPDATE")) { return; }

            string[] tokens = message.Split('|');
            string json = tokens[1];
            PaddleRightState state = JsonUtility.FromJson<PaddleRightState>(json);
            
            transform.position = state.Position;
        };
    }

    // Update is called once per frame
    void Update()
    {
        float movement = input.Pong.Player2.ReadValue<float>();
        
        if (movement != 0) {
            Debug.Log($"[CLIENT] Movement detected: {movement}");

            Vector3 newPosition = transform.position;
            newPosition.y += movement * moveSpeed * Time.deltaTime;
            transform.position = newPosition;

            PaddleRightState state = new PaddleRightState {
                Position = newPosition
            };
            string json = JsonUtility.ToJson(state);
            string message = "PADDLE_RIGHT_UPDATE|" + json;
            Debug.Log($"[CLIENT] Sending message to server: {message}");
            UDP.SendUDPMessage(message, clientManager.ServerEndpoint);
        }
    }
}
