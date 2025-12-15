using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);



    [SerializeField] public NetworkObject rocket;
    private RocketScript rocketScript;
    public TextMeshProUGUI healthText;

    private Transform target;

    private void Start()
    {


        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Only set target for *this* local client
        if (clientId != NetworkManager.Singleton.LocalClientId) return;

        TryAssignLocalPlayerAsTarget();
    }

    private void TryAssignLocalPlayerAsTarget()
    {
        var playerObj = NetworkManager.Singleton.LocalClient?.PlayerObject;
        if (playerObj != null)
        {
            rocket = playerObj;
            target = playerObj.transform;

            rocketScript = playerObj.GetComponent<RocketScript>();

            Debug.Log($"Camera target set to local player: {target.name}");
        }
    }

    private void LateUpdate()
    {
        if (target == null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient)
            TryAssignLocalPlayerAsTarget();

        if (target == null) return;
        var p = target.position + offset;
        p.z = -10;

        transform.position = p;

        // health stuff
        if (rocketScript != null)
        {
            healthText.text = $"Health: \n {rocketScript.Health.Value:0}%";
        }

        // Zoom
        if (Keyboard.current != null)
        {
            var cam = Camera.main;
            if (cam != null)
            {
                if (Keyboard.current.qKey.isPressed) cam.orthographicSize -= 0.1f;
                if (Keyboard.current.eKey.isPressed) cam.orthographicSize += 0.1f;
            }
        }
    }
}
