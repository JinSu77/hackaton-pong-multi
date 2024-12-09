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

    void Awake() {
        // Desactiver mon objet si je ne suis pas le serveur
        if (!Globals.IsServer) {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        UDP.Listen(ListenPort);
        ball = FindFirstObjectByType<PongBall>();

        UDP.OnMessageReceived +=  
            (string message, IPEndPoint sender) => {
                Debug.Log("[SERVER] Message received from " + 
                    sender.Address.ToString() + ":" + sender.Port 
                    + " =>" + message);
                
                switch (message) {
                    case "coucou":
                        // Ajouter le client à mon dictionnaire
                        string addr = sender.Address.ToString() + ":" + sender.Port;
                        if (!Clients.ContainsKey(addr)) {
                            Clients.Add(addr, sender);
                            CheckPlayersAndStartGame();
                        }
                        Debug.Log("There are " + Clients.Count + " clients present.");

                        UDP.SendUDPMessage("welcome!", sender);
                        break;
                }
                
                //@todo : do something with the message that has arrived! 
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
            Clients.Remove(clientAddr);
            CheckPlayersAndStartGame();
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
