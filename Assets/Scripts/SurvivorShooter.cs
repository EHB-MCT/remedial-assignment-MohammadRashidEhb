using UnityEngine;

// Makes the survivor shoot bullets automatically
public class SurvivorShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootInterval = 1f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= shootInterval)
        {
            Shoot();
            timer = 0f;
        }
    }

    // This is a test, to spot bullet/firpoint issues
    void Shoot()
    {
        // Check references before shooting
        if (bulletPrefab == null || firePoint == null) return;
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    }
}
