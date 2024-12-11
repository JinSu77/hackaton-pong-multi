using System.Collections.Generic;
using System.Collections;
using System.Net;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public UDPService UDP;
    public int ListenPort = 25000;
    private PongBall ball;
    private const int REQUIRED_PLAYERS = 2;

    public Dictionary<string, IPEndPoint> Clients = new Dictionary<string, IPEndPoint>(); 
    private Dictionary<string, Team> playerTeams = new Dictionary<string, Team>();

    void Awake() {
        // Desactiver mon objet si je ne suis pas le serveur
        if (!Globals.IsServer) {
            gameObject.SetActive(false);
        }
    }

    private Team AssignTeam()
    {
        int leftCount = 0;
        int rightCount = 0;
        
        foreach (var team in playerTeams.Values)
        {
            if (team == Team.Left) leftCount++;
            if (team == Team.Right) rightCount++;
        }

        return rightCount < leftCount ? Team.Right : Team.Left;
    }

    void Start()
    {
        UDP.Listen(ListenPort);
        ball = FindFirstObjectByType<PongBall>();

        UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
            string addr = sender.Address.ToString() + ":" + sender.Port;
            
            switch (message) {
                case "coucou":
                    if (!Clients.ContainsKey(addr)) 
                    {
                        Team assignedTeam = AssignTeam();
                        playerTeams[addr] = assignedTeam;
                        Clients.Add(addr, sender);
                        
                        UDP.SendUDPMessage($"TEAM_ASSIGNED|{assignedTeam}", sender);
                        Debug.Log($"[SERVER] New player assigned to team {assignedTeam}");
                        
                        CheckPlayersAndStartGame();
                    }
                    UDP.SendUDPMessage("welcome!", sender);
                    break;

                case "disconnect":
                    RemoveClient(addr);
                    break;
            }
        };
    }

    private void CheckPlayersAndStartGame()
    {
        if (Clients.Count == REQUIRED_PLAYERS)
        {
            Debug.Log("[SERVER] Two players connected - Starting game!");
            ball.StartMoving();
            BroadcastUDPMessage("GAME_START");
        }
        else if (Clients.Count < REQUIRED_PLAYERS && ball != null)
        {
            ball.StopMoving();
        }
    }

    // Méthode pour gérer la déconnexion d'un client
    public void RemoveClient(string clientAddr)
    {
        if (Clients.ContainsKey(clientAddr))
        {
            playerTeams.Remove(clientAddr);
            Clients.Remove(clientAddr);
            CheckPlayersAndStartGame();
            Debug.Log($"[SERVER] Client {clientAddr} disconnected");
        }
    }

    public void BroadcastUDPMessage(string message) {
        foreach (KeyValuePair<string, IPEndPoint> client in Clients) {
            UDP.SendUDPMessage(message, client.Value);
        }
    }

    public void StopServer()
    {
        Debug.Log("[SERVER] Stopping server...");
        
        BroadcastUDPMessage("SERVER_STOPPING");
        
        // Attendre un court instant pour que le message soit envoyé
        StartCoroutine(StopServerCoroutine());
    }

    private IEnumerator StopServerCoroutine()
    {
        // Attendre un court instant pour que le message soit envoyé
        yield return new WaitForSeconds(0.1f);
        
        // Fermer proprement l'UDP
        UDP.Close();
        
        Clients.Clear();
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("PongMenu");
    }
}
