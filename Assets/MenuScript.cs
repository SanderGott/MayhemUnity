using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using TMPro;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
    }

    private void StartHost()
    {
        


        ushort port = GetPort();
        SetTransport("0.0.0.0", port); // listen on all interfaces

        var t = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        t.ConnectionData.Address = "0.0.0.0";   // bind all interfaces
        t.ConnectionData.Port = port;
        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.StartHost();
        gameObject.SetActive(false);
    }

    private void StartClient()
    {
        string ip = ipInput.text.Trim();
        ushort port = GetPort();

        var nm = NetworkManager.Singleton;
        Debug.Log($"StartClient clicked. NM null? {nm == null}");

        var transport = nm.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        Debug.Log($"Transport found? {transport != null}");

        transport.ConnectionData.Address = ip;
        transport.ConnectionData.Port = port;

        Debug.Log($"Trying connect to {transport.ConnectionData.Address}:{transport.ConnectionData.Port}");

        nm.OnClientConnectedCallback += id => Debug.Log($"CLIENT CONNECTED. id={id}");
        nm.OnClientDisconnectCallback += id => Debug.Log($"CLIENT DISCONNECTED. id={id} reason={nm.DisconnectReason}");

        bool ok = nm.StartClient();
        Debug.Log($"StartClient() returned: {ok}");
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
