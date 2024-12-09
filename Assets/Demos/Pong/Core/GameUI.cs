using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Button disconnectButton;
    public Button stopServerButton;

    void Start()
    {
        // Configure la visibilité des boutons selon le rôle
        if (Globals.IsServer)
        {
            disconnectButton.gameObject.SetActive(false);
            stopServerButton.gameObject.SetActive(true);

            // Attache l'événement d'arrêt du serveur
            stopServerButton.onClick.AddListener(() => {
                FindFirstObjectByType<ServerManager>()?.StopServer();
            });
        }
        else
        {
            disconnectButton.gameObject.SetActive(true);
            stopServerButton.gameObject.SetActive(false);

            // Attache l'événement de déconnexion du client
            disconnectButton.onClick.AddListener(() => {
                FindFirstObjectByType<ClientManager>()?.Disconnect();
            });
        }
    }
}