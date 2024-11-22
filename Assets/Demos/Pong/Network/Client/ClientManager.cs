using System.Net;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public UDPService UDP;
    public int ServerPort = 25000;

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

        // Utiliser l'IP stockée dans Globals
        ServerEndpoint = new IPEndPoint(IPAddress.Parse(Globals.ServerIP), ServerPort);
        Debug.Log($"[CLIENT] Connexion au serveur : {Globals.ServerIP}:{ServerPort}");
            
        UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
            Debug.Log("[CLIENT] Message received from " + 
                sender.Address.ToString() + ":" + sender.Port 
                + " =>" + message);
        };
    }

    void Update()
    {
        if (Time.time > NextCoucouTimeout) {
            UDP.SendUDPMessage("coucou", ServerEndpoint);
            NextCoucouTimeout = Time.time + 0.5f;
        }
    }
}
