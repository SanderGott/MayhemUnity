using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using TMPro;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("UI (local)")]
    [SerializeField] private TextMeshProUGUI healthText;

    private Transform target;
    private RocketScript rocketScript;

    private void LateUpdate()
    {
        // Bind to local player when it exists
        if (target == null || rocketScript == null)
            TryAssignLocalPlayerAsTarget();

        // Follow
        if (target != null)
        {
            var p = target.position + offset;
            p.z = offset.z; // keep -10
            p.z = -10;
            transform.position = p;
        }

        // Update health UI
        if (healthText != null && rocketScript != null)
            healthText.text = $"Health: \n {rocketScript.Health.Value:0}%";

        // Optional zoom
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

    private void TryAssignLocalPlayerAsTarget()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient) return;

        var playerObj = NetworkManager.Singleton.LocalClient?.PlayerObject;
        if (playerObj == null) return;

        target = playerObj.transform;
        rocketScript = playerObj.GetComponent<RocketScript>();

        if (rocketScript == null)
            Debug.LogError("Local PlayerObject has no RocketScript component.");
    }
}
