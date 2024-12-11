using System.Net;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public UDPService UDP;
    public int ServerPort = 25000;
    public Team AssignedTeam { get; private set; } = Team.None;


    private float NextCoucouTimeout = -1;
    public IPEndPoint ServerEndpoint { get; private set; }

    void Awake() {
        // Désactiver mon objet si je ne suis pas le client
        if (Globals.IsServer) {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        UDP.InitClient();
        ServerEndpoint = new IPEndPoint(IPAddress.Parse(Globals.ServerIP), ServerPort);
        Debug.Log($"[CLIENT] Connexion au serveur : {Globals.ServerIP}:{ServerPort}");
            
        UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
            Debug.Log("[CLIENT] Message received: " + message);

            if (message.StartsWith("TEAM_ASSIGNED|"))
            {
                string teamStr = message.Split('|')[1];
                AssignedTeam = (Team)System.Enum.Parse(typeof(Team), teamStr);
                Debug.Log($"[CLIENT] Assigned to team: {AssignedTeam}");
                EnableCorrectPaddle();
            }
            else if (message == "SERVER_STOPPING")
            {
                Debug.Log("[CLIENT] Server is stopping, returning to menu...");
                UnityEngine.SceneManagement.SceneManager.LoadScene("PongMenu");
            }
        };
    }

    private void EnableCorrectPaddle()
    {
        var leftPaddle = FindFirstObjectByType<PaddleLeftSyncClient>();
        var rightPaddle = FindFirstObjectByType<PaddleRightSyncClient>();
        var input = new PongInput();

        // Désactiver tous les contrôles d'abord
        input.Pong.Player1.Disable();
        input.Pong.Player2.Disable();

        if (AssignedTeam == Team.Left)
        {
            if (leftPaddle != null)
            {
                leftPaddle.enabled = true;
                input.Pong.Player1.Enable();  // Activer uniquement les contrôles ZQSD
            }
            if (rightPaddle != null)
                rightPaddle.enabled = false;
        }
        else if (AssignedTeam == Team.Right)
        {
            if (rightPaddle != null)
            {
                rightPaddle.enabled = true;
                input.Pong.Player2.Enable();  // Activer uniquement les flèches
            }
            if (leftPaddle != null)
                leftPaddle.enabled = false;
        }

        Debug.Log($"[CLIENT] Enabled paddle and controls for team {AssignedTeam}");
    }

    void Update()
    {
        if (Time.time > NextCoucouTimeout) {
            UDP.SendUDPMessage("coucou", ServerEndpoint);
            NextCoucouTimeout = Time.time + 0.5f;
        }
    }

    public void Disconnect()
    {
        Debug.Log("[CLIENT] Disconnecting from server...");
        UDP.SendUDPMessage("disconnect", ServerEndpoint);
        UnityEngine.SceneManagement.SceneManager.LoadScene("PongMenu");
    }
}
