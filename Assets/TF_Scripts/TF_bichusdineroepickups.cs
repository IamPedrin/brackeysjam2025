using UnityEngine;

public class TF_bichusdineroepickups : MonoBehaviour
{
    public GameObject chipPrefab;
    public GameObject cartaPrefab;
    private EnemyStats stats;

    void Awake()
    {
        var health = GetComponent<EnemyHealth>();
        if (health != null)
            stats = health.stats;
    }

    public void OnEnemyDefeated()
    {
        if (chipPrefab)
        {
            var chip = Instantiate(chipPrefab, transform.position, Quaternion.identity);
            var chipPickup = chip.GetComponent<TF_ChipPickup>();
            if (chipPickup != null)
                chipPickup.chipValue = stats.chipsDrop;
        }
        if (cartaPrefab && Random.value < stats.cartaDropChance)
        {
            var carta = Instantiate(cartaPrefab, transform.position, Quaternion.identity);
            var cartaPickup = carta.GetComponent<TF_CartaPickup>();
            if (cartaPickup != null)
                cartaPickup.cartaTipo = stats.cartaDropType;
        }
    }
}
