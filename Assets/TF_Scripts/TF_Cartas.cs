using System.Collections.Generic;
using UnityEngine;

public class TF_Cartas : MonoBehaviour
{
    public enum Cartas
    {
        Paus,
        Copas,
        Espadas,
        Ouros
    }

    public class MaoCartas
    {
        private HashSet<Cartas> cartasMao = new HashSet<Cartas>();

        public bool AddCarta(Cartas naipe)
        {
            if (cartasMao.Contains(naipe)) return false;
            cartasMao.Add(naipe);
            return true;
        }

        public bool UsarCarta(Cartas naipe)
        {
            if (!cartasMao.Contains(naipe)) return false;
            cartasMao.Remove(naipe);
            switch (naipe)
            {
                case Cartas.Paus:
                    //recuperar 25% da vida
                    break;
                case Cartas.Copas:
                    //escudo em uma direcao temporario por 45 secs
                    break;
                case Cartas.Espadas:
                    //+25% de dano por 45 secs
                    break;
                case Cartas.Ouros:
                    //+1.5% de fichas derrotando inimigos por 45 secs
                    break;

            }
            return true;
        }

        public bool TodosNaipes()
        {
            return cartasMao.Count == 4;
        }

        public void BIGGAMBA()
        {
            cartasMao.Clear();
            //buff permanente e chances de tal
        }
    }
}
