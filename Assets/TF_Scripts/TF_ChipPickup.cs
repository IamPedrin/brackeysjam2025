using UnityEngine;

public class TF_ChipPickup : MonoBehaviour
{
    public int chipValue = 100;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var playerController = other.GetComponent<PlayerController>();
            var playerStats = playerController != null ? playerController.stats : null;
            if (playerStats != null)
                playerStats.chips += chipValue;
            Destroy(gameObject);
        }
    }
}
