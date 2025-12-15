using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;


    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;


    private void Start()
    {
        hostButton.onClick.AddListener(HostButtonOnClick);
        clientButton.onClick.AddListener(ClientButtonOnClick);

    }

    private void HostButtonOnClick()
    {
        NetworkManager.Singleton.StartHost();
    }
    private void ClientButtonOnClick()
    {
        NetworkManager.Singleton.StartClient();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = transform.position;
        pos.x = target.position.x;
        pos.y = target.position.y;
        transform.position = pos;


        if (Keyboard.current == null) return;
        Camera cam = Camera.main;



        if (Keyboard.current.qKey.isPressed) cam.orthographicSize -= 0.1f;
        if (Keyboard.current.eKey.isPressed) cam.orthographicSize += 0.1f;


        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            Debug.Log(
                $"IsHost={NetworkManager.Singleton.IsHost}, " +
                $"IsServer={NetworkManager.Singleton.IsServer}, " +
                $"IsClient={NetworkManager.Singleton.IsClient}, " +
                $"IsConnectedClient={NetworkManager.Singleton.IsConnectedClient}, " +
                $"LocalClientId={NetworkManager.Singleton.LocalClientId}"
            );
        }


    }
}
