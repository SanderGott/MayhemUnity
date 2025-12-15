using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NetworkObject))]
public class MissileScript : NetworkBehaviour
{
    [SerializeField] private float missileSpeed = 6f;
    [SerializeField] private float lifetimeSeconds = 5f;

    private Rigidbody2D rb;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    public override void OnNetworkSpawn()
    {
        // "No physics" but still collision callbacks
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.angularVelocity = 0f;

        if (IsServer && lifetimeSeconds > 0f)
            Invoke(nameof(DespawnSelf), lifetimeSeconds);
    }

    private void FixedUpdate()
    {
        if (!IsServer) return; // server authoritative movement

        rb.MovePosition(rb.position + (Vector2)transform.up * (missileSpeed * Time.fixedDeltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Rocket") || other.CompareTag("Background"))
        {
            Debug.Log($"Missile hit {other.tag}");
            DespawnSelf();
        }
    }

    private void DespawnSelf()
    {
        if (!IsServer) return;

        var no = GetComponent<NetworkObject>();
        if (no != null && no.IsSpawned)
            no.Despawn(true); // true = destroy GameObject
        else
            Destroy(gameObject);
    }
}
