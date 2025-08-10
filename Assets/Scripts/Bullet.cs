using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1; // Each bullet deals 1 damage

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void Start()
    {
        Destroy(gameObject, 2f); 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null)
        {
            zombie.TakeDamage(damage);
            Destroy(gameObject); // Destroy bullet on hit
        }
    }
}
