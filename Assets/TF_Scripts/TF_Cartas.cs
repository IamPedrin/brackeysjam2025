using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TF_Cartas : MonoBehaviour
{

    public enum Cartas
    {
        Paus,
        Copas,
        Espadas,
        Ouros
    }

    //CARTAS UI
    public UnityEngine.UI.Image cartaPausUI;
    public UnityEngine.UI.Image cartaCopasUI;
    public UnityEngine.UI.Image cartaEspadasUI;
    public UnityEngine.UI.Image cartaOurosUI;
    public Sprite pausVazio, pausCheio;
    public Sprite copasVazio, copasCheio;
    public Sprite espadasVazio, espadasCheio;
    public Sprite ourosVazio, ourosCheio;
    public bool canUseCopas = true;

    public class MaoCartas
    {
        private HashSet<Cartas> cartasMao = new HashSet<Cartas>();
        private TF_Cartas uiScript;

        public MaoCartas(TF_Cartas uiScript)
        {
            this.uiScript = uiScript;
        }

        public bool AddCarta(Cartas naipe)
        {
            if (cartasMao.Contains(naipe)) return false;
            cartasMao.Add(naipe);
            uiScript.AtualizarUI();
            return true;
        }

        public bool UsarCarta(Cartas naipe)
        {
            Debug.Log($"{naipe}");
            if (!cartasMao.Contains(naipe)) return false;
            Debug.Log($"{naipe}");
            cartasMao.Remove(naipe);
            switch (naipe)
            {
                case Cartas.Paus:
                    var player = GameObject.FindWithTag("Player");
                    if (player != null)
                    {
                        var ph = player.GetComponent<PlayerHealth>();
                        if (ph != null)
                            ph.Heal(Mathf.RoundToInt(ph.stats.maxHearts * 0.25f));
                    }
                    break;
                case Cartas.Copas:
                    if (!uiScript.canUseCopas) return false;
                    var playerObj = GameObject.FindWithTag("Player");
                    if (playerObj != null)
                    {
                        var shield = playerObj.GetComponent<TF_shield>();
                        var playerController = playerObj.GetComponent<PlayerController>();
                        if (shield != null && playerController != null)
                            shield.SpawnShield(-playerController.LastMoveDirection, 3f);
                    }
                    break;
                case Cartas.Espadas:
                    var playerEspadas = GameObject.FindWithTag("Player");
                    if (playerEspadas != null)
                    {
                        var stats = playerEspadas.GetComponent<PlayerStats>();
                        if (stats != null)
                            uiScript.StartCoroutine(uiScript.IncreaseDamageForSeconds(stats, 1.25f, 45f));
                    }
                    break;
                case Cartas.Ouros:
                    var playerOuros = GameObject.FindWithTag("Player");
                    if (playerOuros != null)
                    {
                        var stats = playerOuros.GetComponent<PlayerStats>();
                        if (stats != null)
                            uiScript.StartCoroutine(uiScript.IncreaseChipGainForSeconds(stats, 0.015f, 45f));
                    }
                    break;
            }
            uiScript.AtualizarUI();
            return true;
        }

        public bool TodosNaipes()
        {
            return cartasMao.Count == 4;
        }

        public void BIGGAMBA()
        {
            cartasMao.Clear();
            uiScript.AtualizarUI();
            //buff permanente e chances de tal
        }

        public HashSet<Cartas> GetCartasMao() => cartasMao;
    }

    private MaoCartas maoCartas;

    void Awake()
    {
        maoCartas = new MaoCartas(this);
    }

    void Start()
    {
        AtualizarUI();
    }

    public void OnDescartarPaus(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            maoCartas.UsarCarta(Cartas.Paus);
    }
    public void OnDescartarCopas(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            maoCartas.UsarCarta(Cartas.Copas);
    }
    public void OnDescartarEspadas(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            maoCartas.UsarCarta(Cartas.Espadas);
    }
    public void OnDescartarOuros(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            maoCartas.UsarCarta(Cartas.Ouros);
    }
    public void OnDescartarTodas(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            maoCartas.BIGGAMBA();
    }

    void AtualizarUI()
    {
        cartaPausUI.sprite = maoCartas.GetCartasMao().Contains(Cartas.Paus) ? pausCheio : pausVazio;
        cartaCopasUI.sprite = maoCartas.GetCartasMao().Contains(Cartas.Copas) ? copasCheio : copasVazio;
        cartaEspadasUI.sprite = maoCartas.GetCartasMao().Contains(Cartas.Espadas) ? espadasCheio : espadasVazio;
        cartaOurosUI.sprite = maoCartas.GetCartasMao().Contains(Cartas.Ouros) ? ourosCheio : ourosVazio;
    }

    private System.Collections.IEnumerator IncreaseDamageForSeconds(PlayerStats stats, float multiplier, float seconds)
    {
        float original = stats.damageMultiplier;
        stats.damageMultiplier *= multiplier;
        yield return new WaitForSeconds(seconds);
        stats.damageMultiplier = original;
    }

    private System.Collections.IEnumerator IncreaseChipGainForSeconds(PlayerStats stats, float percent, float seconds)
    {
        float original = stats.chipGainMultiplier;
        stats.chipGainMultiplier += percent;
        yield return new WaitForSeconds(seconds);
        stats.chipGainMultiplier = original;
    }
}
