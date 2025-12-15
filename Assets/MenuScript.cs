using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using TMPro;

public class MenuScript : MonoBehaviour
{
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
        SetPort(port);

        NetworkManager.Singleton.StartHost();
        gameObject.SetActive(false); // hide menu
    }

    private void StartClient()
    {
        ushort port = GetPort();
        SetPort(port);

        NetworkManager.Singleton.StartClient();
        gameObject.SetActive(false); // hide menu
    }

    private ushort GetPort()
    {
        if (ushort.TryParse(portInput.text, out ushort port))
            return port;

        return 7777; // fallback
    }

    private void SetPort(ushort port)
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Port = port;
    }
}
