using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private const string GameSceneName = "SampleScene";

    private void Awake()
    {
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
        Debug.Log("Menu script woke!");
    }

    private void StartHost()
    {
        ushort port = GetPort();
        SetTransport("0.0.0.0", port); // listen on all interfaces

        bool ok = NetworkManager.Singleton.StartHost();
        Debug.Log($"StartHost() returned: {ok}");

        if (!ok) return;

        // If using NGO Scene Management, host should initiate scene changes:
        if (NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
        }
        else
        {
            // Fallback (not recommended for multiplayer sync):
            SceneManager.LoadScene(GameSceneName);
        }

        gameObject.SetActive(false);
    }

    private void StartClient()
    {
        string ip = ipInput.text.Trim();
        ushort port = GetPort();

        var nm = NetworkManager.Singleton;

        var transport = nm.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ip;
        transport.ConnectionData.Port = port;

        Debug.Log($"Trying connect to {transport.ConnectionData.Address}:{transport.ConnectionData.Port}");

        // Load the scene only after we are connected
        nm.OnClientConnectedCallback -= OnClientConnected;
        nm.OnClientConnectedCallback += OnClientConnected;

        nm.OnClientDisconnectCallback -= OnClientDisconnected;
        nm.OnClientDisconnectCallback += OnClientDisconnected;

        bool ok = nm.StartClient();
        Debug.Log($"StartClient() returned: {ok}");
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"CLIENT CONNECTED. id={clientId}");

        // In NGO, clients typically do NOT call LoadScene.
        // The host loads via NetworkManager.SceneManager and clients follow automatically.
        gameObject.SetActive(false);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"CLIENT DISCONNECTED. id={clientId} reason={NetworkManager.Singleton.DisconnectReason}");
    }

    private ushort GetPort()
    {
        if (ushort.TryParse(portInput.text, out ushort port))
            return port;

        return 7777;
    }

    private void SetTransport(string ip, ushort port)
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ip;
        transport.ConnectionData.Port = port;
    }
}
