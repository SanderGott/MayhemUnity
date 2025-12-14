using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MissileScript : MonoBehaviour
{
    [SerializeField] private float missileSpeed = 3f;

    private Rigidbody2D rb;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + (Vector2)transform.up * missileSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Rocket"))
        {
            Debug.Log("Missile hit rocket");
            Destroy(gameObject);
        }
        else if (other.CompareTag("Background"))
        {
            Debug.Log("Missile hit wall");
            Destroy(gameObject);
        }
    }
}
