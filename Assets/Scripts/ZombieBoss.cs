using UnityEngine;
using UnityEngine.Events;

public class ZombieBoss : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    public UnityEvent onBossDefeated;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
    #if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR || UNITY_STANDALONE
        bool inputDetected = false;
        Vector2 inputPosition = Vector2.zero;

        // Touch input (for mobile)
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    inputDetected = true;
                    inputPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    break; // Use first touch that began
                }
            }
        }
        // Mouse input (for Editor/Standalone)
        else if (Input.GetMouseButtonDown(0))
        {
            inputDetected = true;
            inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (inputDetected)
        {
            Collider2D hitCollider = Physics2D.OverlapPoint(inputPosition);
            if (hitCollider != null && hitCollider.gameObject == this.gameObject)
            {
                Debug.Log("ZombieBoss tapped!");
                OnTapped();
            }
        }
#endif
    }

    void OnTapped()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BossClicked();
        }

        TakeDamage(1);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            if (onBossDefeated != null)
                onBossDefeated.Invoke();

            Destroy(gameObject);
        }
    }
}
