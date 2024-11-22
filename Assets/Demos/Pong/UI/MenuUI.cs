using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuUI : MonoBehaviour
{
    public TMP_InputField InpIP;

    void Start()
    {
        // Initialiser l'input avec l'IP par défaut
        InpIP.text = "127.0.0.1";
    }

    public void SetRole(bool isServer) {
        Globals.IsServer = isServer;
        
        // Si c'est un client, sauvegarder l'IP
        if (!isServer && !string.IsNullOrEmpty(InpIP.text)) {
            Globals.ServerIP = InpIP.text;
            Debug.Log($"[MENU] IP serveur définie sur : {Globals.ServerIP}");
        }
    }

    public void StartGame() {
        // Vérification supplémentaire pour le client
        if (!Globals.IsServer && string.IsNullOrEmpty(InpIP.text)) {
            Debug.LogWarning("[MENU] Veuillez entrer une IP valide");
            return;
        }
        
        SceneManager.LoadScene("Pong");
    }
}
