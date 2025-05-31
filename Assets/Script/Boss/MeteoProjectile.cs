using UnityEngine;

public class MeteoProjectile : MonoBehaviour
{
    public float fallSpeed = 5f;

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.Damaged();
            }
        }
        Destroy(gameObject);
    }
}
