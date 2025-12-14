using UnityEngine;
using UnityEngine.InputSystem;

public class RocketScript : MonoBehaviour
{

    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spriteFire;

    [SerializeField] private SpriteRenderer thrustLeft;
    [SerializeField] private SpriteRenderer thrustRight;

    public float thrustVal = 0.1f;
    public float direction;
    public float rotationSpeed = 70;

    public float health = 100;
    public float healthLoss = 10;

    public Rigidbody2D thisRigidBody;

    private SpriteRenderer sr;


    [Header("Missiles")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private Transform missileSpawnPoint;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        

        // Auto-wire if not set in Inspector
        if (!thrustLeft || !thrustRight)
        {
            foreach (var r in GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (r.gameObject.name.Contains("left")) thrustLeft = r;
                if (r.gameObject.name.Contains("right")) thrustRight = r;
            }
        }

        // start hidden
        if (thrustLeft) thrustLeft.enabled = false;
        if (thrustRight) thrustRight.enabled = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thrustVal = 0.01f;
        rotationSpeed = 360;

        

    }

    // Update is called once per frame
    void Update()
    {
        bool turnLeft = false;
        bool turnRight = false;

        if (Keyboard.current != null)
        {
            

            if (Keyboard.current.aKey.isPressed)
            {
                //transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
                thisRigidBody.angularVelocity += rotationSpeed * Time.deltaTime;

                turnLeft = true;
            }
            if (Keyboard.current.dKey.isPressed)
            {
                //transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
                thisRigidBody.angularVelocity -= rotationSpeed * Time.deltaTime;
                turnRight = true;
            }

            if (Keyboard.current.wKey.isPressed)
            {
                Vector2 forward = transform.up;
                thisRigidBody.linearVelocity += forward * thrustVal;
                sr.sprite = spriteFire;
            } else
            {
                sr.sprite = spriteNormal;
        
            }

            if ( Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                FireMissile();
            }
            {
                
            }
        }

        var renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var r in renderers)
        {
            if (r.gameObject.name.Contains("Left")) thrustLeft = r;
            if (r.gameObject.name.Contains("Right")) thrustRight = r;
        }

        if (thrustLeft) thrustLeft.enabled = turnRight; // turning right -> left thruster
        if (thrustRight) thrustRight.enabled = turnLeft;  // turning left  -> right thruster




    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Missile"))
        {
            Debug.Log("Rocket hit by missile");
            // damage / explode / etc
            health -= healthLoss;

        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Background"))
        {
            Debug.Log("Rocket hit background");
            health -= healthLoss;
        }
    }

    private void FireMissile()
    {
        Instantiate(
            missilePrefab,
            missileSpawnPoint.position,
            missileSpawnPoint.rotation
        );
    }
}
