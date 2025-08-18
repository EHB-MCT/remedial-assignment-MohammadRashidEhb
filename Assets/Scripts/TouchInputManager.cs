using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputManager : MonoBehaviour
{
    void Update()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                    
                    // Raycast to detect if player tapped on an object
                    RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
                    if (hit.collider != null)
                    {
                        // Example: send message to object that was touched
                        hit.collider.gameObject.SendMessage("OnTouch", SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }
    }
}
