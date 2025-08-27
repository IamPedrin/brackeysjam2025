using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f; // Você pode pegar isso do inimigo que atirou se quiser
    public string playerTag = "Player";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player atingido!");
            Destroy(gameObject);
        }
        
        else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            Destroy(gameObject);
        }
    }
}
