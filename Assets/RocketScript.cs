using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Netcode.Components;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class RocketScript : NetworkBehaviour
{
    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spriteFire;

    [SerializeField] private SpriteRenderer thrustLeft;   
    [SerializeField] private SpriteRenderer thrustRight;  

    [Header("Tuning")]
    [SerializeField] private float thrustVal = 0.01f;
    [SerializeField] private float rotationSpeed = 360f; 

    [Header("Health")]
    [SerializeField] private float healthLoss = 10f;
    public NetworkVariable<float> Health = new NetworkVariable<float>(
        100f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    [Header("Missiles")]
    [SerializeField] private NetworkObject missilePrefab;     // MUST have NetworkObject
    [SerializeField] private Transform missileSpawnPoint;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private float turnInput;
    private float thrustInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (!thrustLeft || !thrustRight)
        {
            foreach (var r in GetComponentsInChildren<SpriteRenderer>(true))
            {
                var n = r.gameObject.name.ToLowerInvariant();
                if (!thrustLeft && n.Contains("left")) thrustLeft = r;
                if (!thrustRight && n.Contains("right")) thrustRight = r;
            }
        }

        if (thrustLeft) thrustLeft.enabled = false;
        if (thrustRight) thrustRight.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        
        if (OwnerClientId == 1)
        {
            transform.position = new Vector3(8f, -9f, transform.position.z);
        }
    }

    private void Update()
    {
        if (IsOwner && Keyboard.current != null)
        {
            turnInput = 0f;
            if (Keyboard.current.aKey.isPressed) turnInput = 1f;
            if (Keyboard.current.dKey.isPressed) turnInput = -1f;

            thrustInput = Keyboard.current.wKey.isPressed ? 1f : 0f;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                FireMissileServerRpc();
        }
        else
        {
            turnInput = 0f;
            thrustInput = 0f;
        }

        float av = rb ? rb.angularVelocity : 0f;

        bool turningLeft = IsOwner ? (turnInput > 0f) : (av > 1f);
        bool turningRight = IsOwner ? (turnInput < 0f) : (av < -1f);

        if (thrustLeft) thrustLeft.enabled = turningRight; // turning right -> left thruster fires
        if (thrustRight) thrustRight.enabled = turningLeft;  // turning left  -> right thruster fires

        if (IsOwner)
            sr.sprite = thrustInput > 0f ? spriteFire : spriteNormal;
        else
            sr.sprite = (rb && rb.linearVelocity.sqrMagnitude > 0.01f) ? spriteFire : spriteNormal;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        rb.angularVelocity = turnInput * rotationSpeed;

        if (thrustInput > 0f)
            rb.linearVelocity += (Vector2)transform.up * (thrustVal);
    }

    // SERVER spawns the missile so everyone gets the same missile
    [ServerRpc]
    private void FireMissileServerRpc()
    {
        Debug.Log($"Fire pressed. IsOwner={IsOwner} OwnerClientId={OwnerClientId} LocalClientId={NetworkManager.Singleton.LocalClientId}");

        if (!missilePrefab || !missileSpawnPoint) return;

        NetworkObject m = Instantiate(missilePrefab, missileSpawnPoint.position, missileSpawnPoint.rotation);
        m.Spawn(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Missile"))
        {
            Health.Value -= healthLoss;
            Debug.Log($"Rocket hit by missile. Health={Health.Value}");
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!IsServer) return;

        if (col.collider.CompareTag("Background"))
        {
            Health.Value -= healthLoss;
            Debug.Log($"Rocket hit background. Health={Health.Value}");
        }
    }
}


public class ClientNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}