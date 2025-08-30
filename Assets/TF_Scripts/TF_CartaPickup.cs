using UnityEngine;

public class TF_CartaPickup : MonoBehaviour
{
    public TF_Cartas.Cartas cartaTipo;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var cartas = other.GetComponent<TF_Cartas>();
            var mao = typeof(TF_Cartas).GetField("maoCartas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(cartas) as TF_Cartas.MaoCartas;
            if (mao != null)
                mao.AddCarta(cartaTipo);
            Destroy(gameObject);
        }
    }
}
