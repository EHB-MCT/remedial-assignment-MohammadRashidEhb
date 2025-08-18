using UnityEngine;

// Makes the survivor shoot bullets automatically
public class SurvivorShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootInterval = 1f;
    public int bulletDamage = 1;
    private float timer;
    public bool canShoot = true;

    void Update()
    {
        if (!canShoot)
            return;

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
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        
        // This is to set the Bullet DMG
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = bulletDamage; // This line sets unique damage
        }
        
    }
}
