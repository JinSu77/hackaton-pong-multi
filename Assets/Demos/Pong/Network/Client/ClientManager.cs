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
    ServerEndpoint = new IPEndPoint(IPAddress.Parse(Globals.ServerIP), ServerPort);
    Debug.Log($"[CLIENT] Connexion au serveur : {Globals.ServerIP}:{ServerPort}");
        
    UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
        Debug.Log("[CLIENT] Message received from " + 
            sender.Address.ToString() + ":" + sender.Port 
            + " =>" + message);

        // Ajouter la gestion du message d'arrêt du serveur
        if (message == "SERVER_STOPPING")
        {
            Debug.Log("[CLIENT] Server is stopping, returning to menu...");
            UnityEngine.SceneManagement.SceneManager.LoadScene("PongMenu");
        }
    };
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
