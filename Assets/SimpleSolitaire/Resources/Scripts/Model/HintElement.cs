using SimpleSolitaire.Controller;
using SimpleSolitaire.Model.Enum;
using UnityEngine;

namespace SimpleSolitaire.Model
{
    [System.Serializable]
    public class HintElement
    {
        public Card HintCard;
        public DeckType DestinationPack;
        public Vector3 FromPosition;
        public Vector3 ToPosition;

        public HintElement(Card hintCard, Vector3 fromPosition, Vector3 toPosition, DeckType destinationPack)
        {
            HintCard = hintCard;
            FromPosition = fromPosition;
            ToPosition = toPosition;
            DestinationPack = destinationPack;
        }
    }
}