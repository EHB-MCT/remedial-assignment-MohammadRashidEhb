using UnityEngine;

// Controls bullet movement and lifetime
public class Bullet : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // Move bullet right each frame
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void Start()
    {
        // Destroy bullet after 2 seconds
        Destroy(gameObject, 2f); 
    }
}
