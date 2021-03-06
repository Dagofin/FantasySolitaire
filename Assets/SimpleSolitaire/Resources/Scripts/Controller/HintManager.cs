using SimpleSolitaire.Model;
using SimpleSolitaire.Model.Enum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using UnityEngine.UI;

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
        [Header("Settings:")]
        public float DoubleTapTranslateTime = 0.25f;
        public float HintTranslateTime = 0.75f;
        public float hintTimerDefaultValue = 5.0f;
        public float hintTimer = 5.0f;
        public bool hintTimerCount = false;
        public GameObject hintTimerCard;
        public Deck packDeck;
        public Deck wasteDeck;
        public GameObject hintGlow;
        [SerializeField]
        Vector3 hintGlowStartPos;

        private IEnumerator HintCoroutine;

        /// <summary>
        /// Call hint animation.
        /// </summary>
        private void Hint(float time = 0.75f, bool isNeedSetCard = false, Card card = null)
        {
            //if Hints list has hints and the HintProcess isn't running, and this object is active
            if (Hints.Count > 0 && !IsHintProcess && gameObject.activeInHierarchy)
            {
                //if HintCoroutine is currently running, cancel this action
                if (HintCoroutine != null)
                {
                    IsHintProcess = false;
                    StopCoroutine(HintCoroutine);
                }
                //if HintCoroutine isn't running, start a HintCoroutine as HintTranslate, send the card to the hint spot
                //move card to the new location according to speed DoubleTapTranslateTime
                HintCoroutine = HintTranslate(time, isNeedSetCard, card);
                StartCoroutine(HintCoroutine);
            }
        }

        public void Update()
        {
            if (hintTimerCount)
            {
                //decrease hintTimer with time
                hintTimer -= Time.deltaTime;

                //if several seconds go by without Player input
                if (hintTimer <= 0)
                {
                    hintTimerCount = false;
                    //if a valid hint is available in the Autocomplete hints list(more important), activate a glow effect behind the hint card
                    if (AutoCompleteHints.Count > 0)
                    {
                        //set glow card to hintCard of hints list
                        int hintIndex = Random.Range(0, AutoCompleteHints.Count);
                        hintTimerCard = AutoCompleteHints[hintIndex].HintCard.gameObject;
                        //-------TEMP: Change card color instead of glow effect------------------------------------
                        //hintTimerCard.gameObject.GetComponentInChildren<Image>().color = Color.red;
                        hintGlow.transform.position = hintTimerCard.transform.position;
                        hintGlow.transform.SetAsLastSibling();

                    }
                    /*
                    //if a valid hint is available in the all Hints list(second priority), activate a glow effect behind the hint card
                    else if (Hints.Count > 0)
                    {
                        //set glow card to hintCard of hints list
                        int hintIndex = Random.Range(0, Hints.Count);
                        hintTimerCard = Hints[hintIndex].HintCard;
                        //-------TEMP: Change card color instead of glow effect------------------------------------
                        hintTimerCard.gameObject.GetComponentInChildren<Image>().color = Color.red;
                    }
                    */
                    //else if no valid hint is available, active a glow effect behind the top card of the Pack Deck, or Waste deck if the pack deck is empty
                    else
                    {
                        if(packDeck.CardsArray.Count > 0)
                        {
                            hintTimerCard = packDeck.CardsArray[packDeck.CardsArray.Count - 1].gameObject;
                        }
                        else
                        {
                            //hintTimerCard = wasteDeck.CardsArray[wasteDeck.CardsArray.Count - 1];
                            hintTimerCard = packDeck.gameObject;
                        }
                        //-------TEMP: Change card color instead of glow effect------------------------------------
                        //hintTimerCard.GetComponentInChildren<Image>().color = Color.red;
                        hintGlow.transform.position = hintTimerCard.transform.position;
                        hintGlow.transform.SetAsLastSibling();
                    }
                }
            }


        }

        public void ResetHintTimer()
        {
            if(hintTimerCard != null)
            {
                //Reset hintTimer to default value
                hintTimer = hintTimerDefaultValue;
                //turn off the Hint effect -----------------------------------------------------------------------
                //hintTimerCard.GetComponentInChildren<Image>().color = Color.white;
                hintGlow.transform.position = hintGlowStartPos;
                

                //enable the hintTimer
                hintTimerCount = true;
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
            //time = speed of translate movement
            //isNeedSetCard = 
            //card = card being moved
            IsHintProcess = true;

            //Which list of hints are used is determined by isNeedSetCard
            //if the Player clicked this card, AutoCompleteHints is used. If the Hint button was pressed, Hints is used. 
            //TEMP CHANGED
            //List<HintElement> hints = isNeedSetCard ? AutoCompleteHints : Hints;
            List<HintElement> hints = Hints;
            //if the Player clicked this card to move it, set the CurrentHintIndex to 0(first in the list)
            if (isNeedSetCard) 
            {
                CurrentHintIndex = 0;
            }

            if (card != null) 
            {
                int checkIfValidAutoComplete = hints.FindIndex(x => x.HintCard == card);
                //if we click on a card that doesn't have a valid autocomplete, CurrentHintIndex = -1.
                if (checkIfValidAutoComplete == -1)
                {
                    IsHintProcess = false;
                    yield break;
                }
                else
                {
                    //CurrentHintIndex = hints.FindIndex(x => x.HintCard == card);
                    CurrentHintIndex = checkIfValidAutoComplete;
                } 
            }

            //if the Player clicked this card to move it, set the CurrentHintIndex to 0(first in the list)
            //if (isNeedSetCard)
            //{
                //CurrentHintIndex = 0;
            //}

            //if (card != null)
            //{
                //if (CurrentHintIndex == -1 || hints[CurrentHintIndex].DestinationPack != DeckType.DECK_TYPE_ACE)
            //    if (CurrentHintIndex == -1)
            //    {
                    //Debug.LogWarning("After double tap! This Card: " + card.CardNumber + " is not available for complete to ace pack.");
            //        IsHintProcess = false;
            //        yield break;
            //    }
            //}

            //Variable for the Lerp move function
            var t = 0f;
            //set the hintCard to be moved as the correct card in the List
            Card hintCard = hints[CurrentHintIndex].HintCard;
            //set cards position(why?)
            hintCard.Deck.UpdateCardsPosition(false);

            //store the card's current order in the hierarchy before moving so it can be reset later
            CurrentHintSiblingIndex = hintCard.transform.GetSiblingIndex();

            //set the hintCard to the top of it's current Deck(why?)
            hintCard.Deck.SetCardsToTop(hintCard);


            //Physically move the hintCard where it needs to go
            while (t < 1)
            {
                //use Time.deltaTime and time to progress the Lerp function from 0 to 1
                t += Time.deltaTime / time;
                //Lerp the hintCard's position from it's current position to it's destination
                //hintCard.transform.localPosition = Vector3.Lerp(hints[CurrentHintIndex].FromPosition, hints[CurrentHintIndex].ToPosition - new Vector3(0,hintCard.Deck._verticalSpace * 3,0), t);
                hintCard.transform.localPosition = Vector3.Lerp(hints[CurrentHintIndex].FromPosition, hints[CurrentHintIndex].ToPosition, t);
                //hintCard.transform.localPosition = Vector3.Lerp(hints[CurrentHintIndex].FromPosition, hints[CurrentHintIndex].ToPosition - new Vector3(0, 32, 0), t);

                yield return new WaitForEndOfFrame();
                //sets the position of the card, but we're already updating it via the Lerp? Maybe a reset function due to the local position above
                hints[CurrentHintIndex].HintCard.Deck.SetPositionFromCard(hintCard,
                                                                     hintCard.transform.position.x,
                                                                     hintCard.transform.position.y);
            }

            //---------------------------------------------- Not going to use, Hint function is going to change ---------------------------------------------------------------------------
            /*
            //If IsHasHint returns true AND isNeedSetCard is false(aka the Player didn't click this card), reset this card to it's original state(before it was translated)
            if (IsHasHint() && !isNeedSetCard)
            {
                //set cards position(why?)
                hintCard.Deck.UpdateCardsPosition(false);
                //set the hintCard's position to the original starting position
                hintCard.transform.localPosition = hints[CurrentHintIndex].FromPosition;
                //set the hintCard's order in the heirarchy to it's original order
                hintCard.transform.SetSiblingIndex(CurrentHintSiblingIndex);
                //Increment the index for the next Hint so we're not giving the same hint over and over. If we reach the end of the List, reset back to the beginning
                CurrentHintIndex = CurrentHintIndex == hints.Count - 1 ? CurrentHintIndex = 0 : CurrentHintIndex + 1;
            }
            */
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //If the Player DID click this card use OnDragEnd to permanently set the moved card to it's new stack
            if (isNeedSetCard)
            {
                //call the logic to set the hintCard to it's Deck permanently
                _cardLogicComponent.OnDragEnd(hintCard);
            }
            //end the process
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
                                        // -------------------------------------------------- DIAL IN THE TARGET LOCATION OF THE CARD SOMEHOW -----------------------------------------------------------------
                                        AutoCompleteHints.Add(new HintElement(card, card.transform.localPosition, topTargetDeckCard != null ? topTargetDeckCard.transform.localPosition - new Vector3(0, card.Deck._verticalSpace, 0) : targetDeck.transform.localPosition, targetDeck.Type));
                                    }

                                    Hints.Add(new HintElement(card, card.transform.localPosition, topTargetDeckCard != null ? topTargetDeckCard.transform.localPosition : targetDeck.transform.localPosition, targetDeck.Type));
                                }
                            }
                        }
                    }
                }
            }

            ActivateHintButton(IsHasHint());
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