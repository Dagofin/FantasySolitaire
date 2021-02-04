using SimpleSolitaire.Model.Enum;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
    public class Deck : MonoBehaviour, IPointerClickHandler
    {
        public CardLogic CardLogicComponent;
        public int DeckNum = 0;
        public DeckType Type = 0;
        public List<Card> CardsArray = new List<Card>();

        private float _deckWidth;
        private float _deckHeight;
        private float _verticalSpace;
        private float _wasteHorizontalSpace;

        [Space(5f)]
        [SerializeField]
        private Image _backgroundImage;
        [SerializeField]
        private GameManager _gameManagerComponent;
        private AutoCompleteManager _autoCompleteManager;

        private void Awake()
        {
            _autoCompleteManager = GameObject.Find("AutoCompleteManager").GetComponent<AutoCompleteManager>();
            _deckHeight = _gameManagerComponent.Corners[2].y - _gameManagerComponent.Corners[0].y;
            _deckWidth = _gameManagerComponent.Corners[2].x - _gameManagerComponent.Corners[0].x;
            _verticalSpace = _deckHeight / 3.5f;
            _wasteHorizontalSpace = _deckWidth / 3.0f;
        }

        /// <summary>
        /// Set up background image for deck <see cref="_backgroundImage"/>
        /// </summary>
        /// <param name="str">string name of deck</param>
        public void SetBackgroundImg(string str)
        {
            Sprite tempType = Resources.Load("Sprites/decks/" + str, typeof(Sprite)) as Sprite;
            _backgroundImage.overrideSprite = tempType;
        }

        /// <summary>
        /// Show/Add in game  new card from pack.
        /// </summary>
        /// <param name="card"></param>
        public void PushCard(Card card)
        {
            card.Deck = this;
            card.IsDraggable = true;
            card.CardStatus = 1;
            CardsArray.Add(card);
        }

        /// <summary>
        /// Show/Add in game new card array from pack.
        /// </summary>
        /// <param name="card"></param>
        public void PushCardArray(Card[] cardArray)
        {
            for (int i = 0; i < cardArray.Length; i++)
            {
                cardArray[i].Deck = this;
                cardArray[i].IsDraggable = true;
                cardArray[i].CardStatus = 1;
                CardsArray.Add(cardArray[i]);
            }
        }

        /// <summary>
        /// Return last card from pack.
        /// </summary>
        public Card Pop()
        {
            Card retCard = null;
            int count = CardsArray.Count;
            if (count > 0)
            {
                retCard = (Card)CardsArray[count - 1];
                retCard.Deck = null;
                CardsArray.Remove(retCard);
            }
            return retCard;
        }

        /// <summary>
        /// Get card array from pop.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Card[] PopFromCard(Card card)
        {
            int i = 0;
            int count = CardsArray.Count;
            while (i < count)
            {
                if ((Card)CardsArray[i] == card)
                {
                    break;
                }
                i++;
            }
            Card[] cardArray = new Card[count - i];
            int k = 0;
            for (int j = i; j < count; j++)
            {
                cardArray[count - i - 1 - (k++)] = Pop();
            }
            return cardArray;
        }

        //TODO нужно запоминать деки и их карты и тогда будет ундо

        /// <summary>
        /// Update card position in game by solitaire piramide style
        /// </summary>
        /// <param name="firstTime">If it first game update</param>
        public void UpdateCardsPosition(bool firstTime)
        {
            for (int i = 0; i < CardsArray.Count; i++)
            {
                Card card = (Card)CardsArray[i];
                //card.CardRect.sizeDelta = new Vector2(_deckWidth, _deckHeight);
                card.transform.SetAsLastSibling();
                if (Type == DeckType.DECK_TYPE_PACK)
                {   //reset card to facedown in the deck pack 
                    card.IsDraggable = false;
                    card.gameObject.transform.position = gameObject.transform.position;
                    card.RestoreBackView();
                }
                else
                {
                    if (Type == DeckType.DECK_TYPE_BOTTOM)
                    {
                        card.gameObject.transform.position = gameObject.transform.position - i * new Vector3(0, _verticalSpace, 0);
                    }
                    else if (Type == DeckType.DECK_TYPE_ACE)
                    {
                        card.gameObject.transform.position = gameObject.transform.position;
                        //card.gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles);
                    }
                    else
                    {
                        card.IsDraggable = false;
                        card.gameObject.transform.position = gameObject.transform.position;
                        if (CardsArray.Count == 2)
                        {
                            if (i == 1)
                            {
                                card.gameObject.transform.position = gameObject.transform.position + new Vector3(_wasteHorizontalSpace, 0, 0);
                                card.IsDraggable = true;
                            }
                        }
                        else if (CardsArray.Count >= 3)
                        {
                            if (i == CardsArray.Count - 1)
                            {
                                card.gameObject.transform.position = gameObject.transform.position + new Vector3(2 * _wasteHorizontalSpace, 0, 0);
                                card.IsDraggable = true;
                            }
                            else if (i == CardsArray.Count - 2)
                            {
                                card.gameObject.transform.position = gameObject.transform.position + new Vector3(_wasteHorizontalSpace, 0, 0);
                            }
                        }
                    }
                    if (i == this.CardsArray.Count - 1)
                    {
                        //if this is the first card in the bottom deck, set it as draggable update the facedowncardscount. 
                        card.IsDraggable = true;
                        card.CardStatus = 1;
                        card.UpdateCardImg();
                        //tell card to play the Card Flip animation
                        //card.CardFlipAnim();

                        card.SetFaceUp();
                        if (!firstTime)
                        {
                            //CardLogicComponent.faceDownCardsCount--;
                            //card.SetFaceUp();
                        }
                    }
                    else
                    {
                        if (firstTime)
                        {
                            //if initializing the bottom decks and this card is the first in the stack, set it to be facedown and not draggable. 
                            card.IsDraggable = false;
                            card.CardStatus = 0;
                            //card.isFaceDown = true;
                            card.SetFaceDown();
                            //CardLogicComponent.faceDownCardsCount++;
                        }
                        //'Flip card facedown' aka update sprite to the card back
                        card.UpdateCardImg();
                    }
                }
            }
            UpdateCardsActiveStatus();
        }



            /// <summary>
            /// After set positions <see cref="UpdateCardsPosition(bool)"/> game show for user available cards and not available.
            /// </summary>
            public void UpdateCardsActiveStatus()
            {
                int compareNum = 4;
                if (Type == DeckType.DECK_TYPE_ACE || Type == DeckType.DECK_TYPE_WASTE || Type == DeckType.DECK_TYPE_PACK)
                {
                    if (CardsArray.Count > 0)
                    {
                        int j = 0;
                        if (Type == DeckType.DECK_TYPE_PACK)
                        {
                            compareNum = 2;
                        }
                        for (int i = CardsArray.Count - 1; i >= 0; i--)
                        {
                            Card card = (Card)CardsArray[i];
                            if (j < compareNum)
                            {
                                card.gameObject.SetActive(true);
                                j++;
                            }
                            else
                            {
                                card.gameObject.SetActive(false);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = CardsArray.Count - 1; i >= 0; i--)
                    {
                        ((Card)CardsArray[i]).gameObject.SetActive(true);
                    }
                }
            }

            /// <summary>
            /// Set new position for card holder.
            /// </summary>
            /// <param name="card">Card for change position</param>
            /// <param name="x">Position by X axis</param>
            /// <param name="y">Position by Y axis</param>
            public void SetPositionFromCard(Card card, float x, float y)
            {
                int i;
                for (i = 0; i < CardsArray.Count; i++)
                {
                    if ((Card)CardsArray[i] == card)
                    {
                        break;
                    }
                }
                int m = 0;
                for (int j = i; j < CardsArray.Count; j++)
                {
                    ((Card)CardsArray[j]).SetPosition(new Vector3(x, y - m++ * _verticalSpace, 0));
                }
            }

            /// <summary>
            /// Collect card on aceDeck.
            /// </summary>
            /// <param name="card">Card for collect.</param>
            public void SetCardsToTop(Card card)
            {
                bool found = false;
                for (int i = 0; i < CardsArray.Count; i++)
                {
                    if ((Card)CardsArray[i] == card)
                    {
                        found = true;
                    }
                    if (found)
                    {
                        ((Card)CardsArray[i]).transform.SetAsLastSibling();
                    }
                }
            }

            /// <summary>
            /// Get last card on aceDeck.
            /// </summary>
            /// <returns></returns>
            public Card GetTopCard()
            {
                if (this.CardsArray.Count > 0)
                {
                    return (Card)CardsArray[CardsArray.Count - 1];
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Get previous last card on deck.
            /// </summary>
            /// <returns></returns>
            public Card GetPreviousFromCard(Card fromCard)
            {
                int index = CardsArray.IndexOf(fromCard);

                if (index >= 1)
                {
                    return (Card)CardsArray[index - 1];
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// If we can drop card to other card it will be true.
            /// </summary>
            /// <param name="card">Checking card</param>
            /// <returns>We can drop or no</returns>
            public bool AcceptCard(Card card)
            {
                Card topCard = GetTopCard();
                switch (Type)
                {
                    case DeckType.DECK_TYPE_BOTTOM:
                        if (topCard != null)
                        {
                            if (topCard.CardColor != card.CardColor && topCard.Number == card.Number + 1)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (card.Number == 13)
                            {
                                return true;
                            }
                            return false;
                        }
                    case DeckType.DECK_TYPE_ACE:
                        Deck srcDeck = card.Deck;
                        if (srcDeck.GetTopCard() != card)
                        {
                            return false;
                        }
                        if (topCard != null)
                        {
                            if (topCard.CardType == card.CardType && topCard.Number == card.Number - 1)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (card.Number == 1)
                            {
                                return true;
                            }
                            return false;
                        }
                }
                return false;
            }

            /// <summary>
            /// If we drop our card on other card this method return true.
            /// </summary>
            /// <param name="card">Checking card</param>
            public bool OverlapWithCard(Card card)
            {
                if (card.Deck == this)
                {
                    return false;
                }
                bool bOverlaped = false;
                float x1 = transform.position.x;
                float x2 = x1 + _deckWidth;
                float y1 = transform.position.y;
                Card topCard = GetTopCard();
                if (topCard)
                {
                    y1 = topCard.transform.position.y;
                }
                float y2 = y1 + _deckHeight;

                float x11 = card.transform.position.x;
                float x21 = x11 + _deckWidth;
                float y11 = card.transform.position.y;
                float y21 = y11 + _deckHeight;

                float INTERSECT_SPACE = 10;
                if ((x2 >= (x11 + INTERSECT_SPACE) && x1 <= x11) || (x1 >= x11 && x1 <= (x21 - INTERSECT_SPACE)))
                {
                    if ((y1 >= y11 && y1 <= y21) || (y1 <= y11 && y2 >= y11))
                    {
                        bOverlaped = true;
                    }
                }
                return bOverlaped;
            }

            public void OnPointerClick(PointerEventData eventData)
            {
                if (Type == DeckType.DECK_TYPE_PACK)
                {
                    CardLogicComponent.WriteUndoState();
                    CardLogicComponent.OnClickPack();
                }
            }

            /// <summary>
            /// Get available card count.
            /// </summary>
            /// <returns></returns>
            public int GetCardNums()
            {
                return CardsArray.Count;
            }

            /// <summary>
            /// Initialize first game state.
            /// </summary>
            public void RestoreInitialState()
            {
                for (int i = 0; i < CardsArray.Count; i++)
                {
                    Card card = (Card)CardsArray[i];
                    card.RestoreBackView();
                }
                CardsArray.Clear();
            }
        }
    }