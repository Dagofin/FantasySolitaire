using SimpleSolitaire.Model;
using SimpleSolitaire.Model.Enum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace SimpleSolitaire.Controller
{
    public class HintManager : MonoBehaviour
    {
        [Header("Components:")]
        [SerializeField]
        private CardLogic _cardLogicComponent;
        [SerializeField]
        private AutoCompleteManager _autoCompleteComponent;
        [SerializeField]
        private Button _hintButton;

        [Header("Hint data:")]
        public List<Card> IsAvailableForMoveCardArray = new List<Card>();
        public List<HintElement> Hints = new List<HintElement>();
        public List<HintElement> AutoCompleteHints = new List<HintElement>();
        public int CurrentHintIndex = 0;
        public int CurrentHintSiblingIndex;
        public bool IsHintProcess = false;
        public bool IsHintWasUsed = false;
        [Header("Setings:")]
        public float DoubleTapTranslateTime = 0.25f;
        public float HintTranslateTime = 0.75f;

        private IEnumerator HintCoroutine;

        /// <summary>
        /// Call hint animation.
        /// </summary>
        private void Hint(float time = 0.75f, bool isNeedSetCard = false, Card card = null)
        {
            if (Hints.Count > 0 && !IsHintProcess && gameObject.activeInHierarchy)
            {
                if (HintCoroutine != null)
                {
                    IsHintProcess = false;
                    StopCoroutine(HintCoroutine);
                }
                //move card to the new location according to speed DoubleTapTranslateTime
                HintCoroutine = HintTranslate(time, isNeedSetCard, card);
                StartCoroutine(HintCoroutine);
            }
        }

        /// <summary>
		/// 
		/// </summary>
		public void HintAndSetByDoubleClick(Card card)
        {
            Hint(DoubleTapTranslateTime, true, card);
        }

        /// <summary>
        /// Called automatically whe n auto complete action is active.
        /// </summary>
        public void HintAndSet(float time = 0.75f)
        {
            Hint(time, true);
        }

        /// <summary>
        /// Called when user press hint button.
        /// </summary>
        public void HintButtonAction()
        {
            Hint(HintTranslateTime, false);
        }

        /// <summary>
        /// Hint animation and actions of setting.
        /// </summary>
        private IEnumerator HintTranslate(float time = 0.75f, bool isNeedSetCard = false, Card card = null)
        {
            IsHintProcess = true;

            List<HintElement> hints = isNeedSetCard ? AutoCompleteHints : Hints;
            if (isNeedSetCard) CurrentHintIndex = 0;
            if (card != null) CurrentHintIndex = hints.FindIndex(x => x.HintCard == card);

            if (card != null)
            {
                //if (CurrentHintIndex == -1 || hints[CurrentHintIndex].DestinationPack != DeckType.DECK_TYPE_ACE)
                if (CurrentHintIndex == -1)
                {
                    //Debug.LogWarning("After double tap! This Card: " + card.CardNumber + " is not available for complete to ace pack.");
                    IsHintProcess = false;
                    yield break;
                }
            }
            var t = 0f;
            Card hintCard = hints[CurrentHintIndex].HintCard;
            hintCard.Deck.UpdateCardsPosition(false);

            CurrentHintSiblingIndex = hintCard.transform.GetSiblingIndex();

            hintCard.Deck.SetCardsToTop(hintCard);

            while (t < 1)
            {
                t += Time.deltaTime / time;
                hintCard.transform.localPosition = Vector3.Lerp(hints[CurrentHintIndex].FromPosition, hints[CurrentHintIndex].ToPosition, t);

                yield return new WaitForEndOfFrame();
                hints[CurrentHintIndex].HintCard.Deck.SetPositionFromCard(hintCard,
                                                                     hintCard.transform.position.x,
                                                                     hintCard.transform.position.y);
            }

            if (IsHasHint() && !isNeedSetCard)
            {
                hintCard.Deck.UpdateCardsPosition(false);
                hintCard.transform.localPosition = hints[CurrentHintIndex].FromPosition;
                hintCard.transform.SetSiblingIndex(CurrentHintSiblingIndex);
                CurrentHintIndex = CurrentHintIndex == hints.Count - 1 ? CurrentHintIndex = 0 : CurrentHintIndex + 1;
            }

            if (isNeedSetCard)
            {
                _cardLogicComponent.OnDragEnd(hintCard);
            }

            IsHintProcess = false;
        }

        /// <summary>
        /// Update for user drag hints.
        /// </summary>
        /// <param name="isAutoComplete"></param>
        public void UpdateAvailableForDragCards(bool isAutoComplete = false)
        {
            IsAvailableForMoveCardArray = new List<Card>();

            Card[] cards = _cardLogicComponent.CardsArray;

            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].IsDraggable)
                {
                    if (cards[i].Deck.Type != DeckType.DECK_TYPE_ACE)
                    {
                        IsAvailableForMoveCardArray.Add(cards[i]);
                    }
                    else if (!isAutoComplete)
                    {
                        if (cards[i].Deck.GetTopCard() == cards[i])
                        {
                            IsAvailableForMoveCardArray.Add(cards[i]);
                        }
                    }
                }
            }

            GenerateHints();
        }

        /// <summary>
        /// Update auto complete hints
        /// </summary>
        public void UpdateAvailableForAutoCompleteCards()
        {
            IsAvailableForMoveCardArray = new List<Card>();

            Card[] cards = _cardLogicComponent.CardsArray;

            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].IsDraggable)
                {
                    if (cards[i].Deck.Type != DeckType.DECK_TYPE_ACE)
                    {
                        IsAvailableForMoveCardArray.Add(cards[i]);
                    }
                }
            }

            GenerateHints(true);
        }

        /// <summary>
        /// Generate new hint depending on available for move cards.
        /// </summary>
        private void GenerateHints(bool isAutoComplete = false)
        {
            if (!isAutoComplete)
            {
                CurrentHintIndex = 0;
            }

            AutoCompleteHints = new List<HintElement>();
            Hints = new List<HintElement>();
            bool isHasAutoCompleteHints = false;

            if (IsAvailableForMoveCardArray.Count > 0)
            {
                foreach (var card in IsAvailableForMoveCardArray)
                {
                    for (int i = 0; i < _cardLogicComponent.AllDeckArray.Length; i++)
                    {
                        isHasAutoCompleteHints = true;
                        Deck targetDeck = _cardLogicComponent.AllDeckArray[i];
                        if (targetDeck.Type == DeckType.DECK_TYPE_BOTTOM || targetDeck.Type == DeckType.DECK_TYPE_ACE)
                        {
                            if (card != null)
                            {
                                Card topTargetDeckCard = targetDeck.GetTopCard();
                                Card topDeckCard = card.Deck.GetPreviousFromCard(card);


                                if (card.Deck.Type == DeckType.DECK_TYPE_ACE)
                                {
                                    continue;
                                }

                                if (topDeckCard == null && topTargetDeckCard == null && targetDeck.Type != DeckType.DECK_TYPE_ACE)
                                {
                                    if (card.Deck.Type != DeckType.DECK_TYPE_WASTE)
                                    {
                                        isHasAutoCompleteHints = false;

                                        if (isAutoComplete)
                                        {
                                            continue;
                                        }
                                    }
                                }

                                if (topDeckCard != null && topTargetDeckCard != null && topDeckCard.Number == topTargetDeckCard.Number && topDeckCard.CardStatus == 1 && card.Deck.Type != DeckType.DECK_TYPE_WASTE)
                                {
                                    isHasAutoCompleteHints = false;

                                    if (isAutoComplete)
                                    {
                                        continue;
                                    }
                                }

                                if (targetDeck.AcceptCard(card))
                                {
                                    if (isHasAutoCompleteHints)
                                    {
                                        AutoCompleteHints.Add(new HintElement(card, card.transform.localPosition, topTargetDeckCard != null ? topTargetDeckCard.transform.localPosition : targetDeck.transform.localPosition, targetDeck.Type));
                                    }

                                    Hints.Add(new HintElement(card, card.transform.localPosition, topTargetDeckCard != null ? topTargetDeckCard.transform.localPosition : targetDeck.transform.localPosition, targetDeck.Type));
                                }
                            }
                        }
                    }
                }
            }

            ActivateHintButton(IsHasHint());
            //TEMP FLAG TESTING AUTOCOMPLETE FUNCTIONALITY 11111111111111111111111111111111111111111111111111111111111111111
            //ActivateAutoCompleteHintButton(IsHasAutoCompleteHint());
        }

        public Card GetCurrentHintCard(bool isAutoComplete)
        {
            List<HintElement> hints = isAutoComplete ? AutoCompleteHints : Hints;

            return hints.Count > 0 ? hints[0].HintCard : null;
        }

        /// <summary>
        /// Reset all hints.
        /// </summary>
        public void ResetHint()
        {
            if (HintCoroutine != null)
            {
                StopCoroutine(HintCoroutine);
            }
            IsHintProcess = false;

            if (IsHintWasUsed)
            {
                Hints[CurrentHintIndex].HintCard.Deck.UpdateCardsPosition(false);

                Hints[CurrentHintIndex].HintCard.transform.localPosition = Hints[CurrentHintIndex].FromPosition;
                Hints[CurrentHintIndex].HintCard.transform.SetSiblingIndex(CurrentHintSiblingIndex);
            }
        }

        /// <summary>
        /// Activate for user hint button on bottom panel.
        /// </summary>
        private void ActivateHintButton(bool isActive)
        {
            _hintButton.interactable = isActive;
        }

        //TEMP FLAG TESTING AUTOCOMPLETE FUNCTIONALITY 11111111111111111111111111111111111111111111111111111111111111111
        /*
        /// <summary>
        /// Activate auto complete button if auto complete hints is available. 
        /// </summary>
        private void ActivateAutoCompleteHintButton(bool isActive)
        {
            if (isActive)
            {
                _autoCompleteComponent.ActivateAutoCompleteAvailability();
            }
            else
            {
                _autoCompleteComponent.DectivateAutoCompleteAvailability();
            }
        }
        */

        /// <summary>
        /// Is has available hints for user.
        /// </summary>
        public bool IsHasHint()
        {
            return Hints.Count > 0;
        }

        /// <summary>
        /// Check for availability of auto complete hints.
        /// </summary>
        /// <returns></returns>
        public bool IsHasAutoCompleteHint()
        {
            return AutoCompleteHints.Count > 0;
        }

        private void OnDestroy()
        {
            if (HintCoroutine != null)
            {
                StopCoroutine(HintCoroutine);
            }
        }
    }
}