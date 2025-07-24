using UnityEngine;

public class DeactivateObstacles : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.SetActive(false);
    }
}
