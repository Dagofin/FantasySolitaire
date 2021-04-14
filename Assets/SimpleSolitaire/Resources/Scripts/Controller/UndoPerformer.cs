using SimpleSolitaire.Model.Config;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
	public class UndoPerformer : MonoBehaviour
	{
		[Header("Components:")]
		[SerializeField]
		private CardLogic _cardLogicComponent;
		[SerializeField]
		private GameManager _gameMgrComponent;
		[SerializeField]
		private HintManager _hintComponent;
		[SerializeField]
		private AdsManager _adsComponent;
		[SerializeField]
		private Button _undoButton;
		[SerializeField]
		private Animator _undoButtonAnim;
		[SerializeField]
		private Text _undoAvailableCountsText;

		[Header("Options:")]
		[Tooltip("If TRUE UndoLogic will be available only AvailableUndoCounts times. After that need to watch ads for getting new undo uses.")]
		public bool IsCountable = false;
		[Tooltip("How much UndoLogic uses user have from start and after ads watching.")]
		public int DefaultUndoCounts = 0;

		[Tooltip("Array with all states.")]
		public UndoData StatesData;

		private int _availableUndoCounts = 0;

		public int gameUndoCount = 0;

		private readonly string _lastGameKey = "LastGameKey";
		private readonly string _undoBtnAnimationKey = "IsAnimate";

		/// <summary>
		/// Action of Undo all decks/cards states.
		/// </summary>
		public void Undo()
		{
			if (StatesData.States.Count > 0)
			{
				//if undos are countable and there are undos to be used, subtract the undo and update the text
				if (IsCountable && _availableUndoCounts > 0)
				{
					_availableUndoCounts--;
					gameUndoCount++;
					_undoAvailableCountsText.text = _availableUndoCounts.ToString();
				}
				//else if undos are countable and there are no remaining undos, trigger an ad to reset the undo count
				else if (IsCountable && _availableUndoCounts == 0)
				{
					//_gameMgrComponent.OnClickGetUndoAdsBtn();

					//if the Player doesn't have any more undos, trigger an ad to get more
					if(_adsComponent.adsActive == true)
                    {

						_adsComponent.ShowAd("refillUndos");

						return;
					}
                    else
                    {
						return;
                    }
					
				}

				_hintComponent.IsHintWasUsed = false;
				_cardLogicComponent.IsNeedResetPack = false;

				//go through each card in the alldeckarray(master of all cards)
				for (int i = 0; i < _cardLogicComponent.AllDeckArray.Length; i++)
				{
					//assign deck to the all deck array, so all cards
					Deck deck = _cardLogicComponent.AllDeckArray[i];
					//assign the deck record to the current state -1, so the previous state
					DeckRecord deckRecord = StatesData.States[StatesData.States.Count - 1].DecksRecord[i];
					//assign deck's card list to the deckRecord cards list of the previous state
					deck.CardsArray = new List<Card>(deckRecord.Cards);

					for (int j = 0; j < deckRecord.CardsRecord.Count; j++)
					{
						Card card = deck.CardsArray[j];
						CardRecord cardRecord = deckRecord.CardsRecord[j];

						card.CardType = cardRecord.CardType;
						card.CardNumber = cardRecord.CardNumber;
						card.Number = cardRecord.Number;
						card.CardStatus = cardRecord.CardStatus;
						card.CardColor = cardRecord.CardColor;
						card.IsDraggable = cardRecord.IsDraggable;
						card.IndexZ = cardRecord.IndexZ;
						card.Deck = cardRecord.Deck;
						card.transform.localPosition = cardRecord.Position;
						card.transform.SetSiblingIndex(cardRecord.SiblingIndex);
						card.isFaceDown = cardRecord.IsFaceDown;
					}
					deck.UpdateCardsPosition(false);
				}

				_hintComponent.UpdateAvailableForDragCards();
				_cardLogicComponent.GameManagerComponent.CardMove();
				StatesData.States.RemoveAt(StatesData.States.Count - 1);
				ActivateUndoButton();
			}
		}

		/// <summary>
		/// Setup <see cref="DefaultUndoCounts"/> value to _availableUndoCounts.
		/// </summary>
		public void UpdateUndoCounts()
		{
			if (IsCountable)
			{
				_undoButtonAnim.SetBool(_undoBtnAnimationKey, false);

				_availableUndoCounts = DefaultUndoCounts;
				_undoAvailableCountsText.text = _availableUndoCounts.ToString();
				_undoAvailableCountsText.enabled = true;
			}
		}

		/// <summary>
		/// Collect new state.
		/// </summary>
		public void AddUndoState(Deck[] allDeckArray, Card[] allCards)
		{
			StatesData.States.Add(new UndoStates(allDeckArray, allCards));
		}

		/// <summary>
		/// Activate for user undo button on bottom panel.
		/// </summary>
		public void ActivateUndoButton()
		{
			bool isHasUndoState = IsHasUndoState();

			if (IsCountable && _availableUndoCounts == 0 && StatesData.States.Count > 0)
			{
				_undoButtonAnim.SetBool(_undoBtnAnimationKey, true);
			}

			_undoButton.interactable = (IsCountable) ? StatesData.States.Count != 0 : isHasUndoState;

			_undoAvailableCountsText.text = _availableUndoCounts.ToString();
			_undoAvailableCountsText.enabled = IsCountable && isHasUndoState;
		}

		/// <summary>
		/// Check for existing undo states.
		/// </summary>
		private bool IsHasUndoState()
		{
			return IsCountable && _availableUndoCounts != 0 && StatesData.States.Count > 0
				|| !IsCountable && StatesData.States.Count > 0;
		}

		/// <summary>
		/// Clear array with states
		/// </summary>
		public void ResetUndoStates()
		{
			_undoButtonAnim.SetBool(_undoBtnAnimationKey, false);
			_availableUndoCounts = DefaultUndoCounts;
			StatesData.States.Clear();
			ActivateUndoButton();
		}

		/// <summary>
		/// Save game with current game state.
		/// </summary>
		/// <param name="time"></param>
		/// <param name="steps"></param>
		/// <param name="score"></param>
		public void SaveGame(int time, int steps, int score)
		{
			StatesData.IsCountable = IsCountable;
			StatesData.AvailableUndoCounts = _availableUndoCounts;
			StatesData.Time = time;
			StatesData.Steps = steps;
			StatesData.Score = score;
			StatesData.CardsNums = _cardLogicComponent.CardNumberArray;
			string game = JsonUtility.ToJson(StatesData);
			PlayerPrefs.SetString(_lastGameKey, game);
		}

		/// <summary>
		/// Load game if it exist.
		/// </summary>
		public void LoadGame()
		{
			//if a lastGameKey exists, the Player has played a last game
			if (PlayerPrefs.HasKey(_lastGameKey))
			{
				//load the lastGame from the Json utility
				string lastGameData = PlayerPrefs.GetString(_lastGameKey);
				StatesData = JsonUtility.FromJson<UndoData>(lastGameData);

				//if StateData contains data, continue
				if (StatesData.States.Count > 0)
				{
					_hintComponent.IsHintWasUsed = false;
					_cardLogicComponent.IsNeedResetPack = false;
					IsCountable = StatesData.IsCountable;
					_availableUndoCounts = StatesData.AvailableUndoCounts;

					InitCardsNumberArray();
					for (int i = 0; i < _cardLogicComponent.AllDeckArray.Length; i++)
					{
						Deck deck = _cardLogicComponent.AllDeckArray[i];
						DeckRecord deckRecord = StatesData.States[StatesData.States.Count - 1].DecksRecord[i];
						deck.CardsArray = new List<Card>(deckRecord.Cards);

						if (deck.CardsArray.Count > 0)
						{
							for (int j = 0; j < deckRecord.CardsRecord.Count; j++)
							{
								Card card = deck.CardsArray[j];
								CardRecord cardRecord = deckRecord.CardsRecord[j];

								card.CardType = cardRecord.CardType;
								card.CardNumber = cardRecord.CardNumber;
								card.Number = cardRecord.Number;
								card.CardStatus = cardRecord.CardStatus;
								card.CardColor = cardRecord.CardColor;
								card.IsDraggable = cardRecord.IsDraggable;
								card.IndexZ = cardRecord.IndexZ;
								card.isFaceDown = cardRecord.IsFaceDown;
								card.Deck = cardRecord.Deck;
								card.transform.localPosition = cardRecord.Position;
								card.transform.SetSiblingIndex(cardRecord.SiblingIndex);

								if(card.isFaceDown == true)
                                {
									_cardLogicComponent.faceDownCardsCount++;
                                }
							}
						}
						deck.UpdateCardsPosition(false);
						//deck.UpdateCardsPosition(true);
					}

					StatesData.States.RemoveAt(StatesData.States.Count - 1);
					_hintComponent.UpdateAvailableForDragCards();
					ActivateUndoButton();
				}
			}
		}

		/// <summary>
		/// Set all numbers for cards of session. Needs to replay session whe user load last game.
		/// </summary>
		public void InitCardsNumberArray()
		{
			_cardLogicComponent.InitSpecificCardNums(StatesData.CardsNums);
		}

		/// <summary>
		/// Is has saved game process.
		/// </summary>
		public bool IsHasGame()
		{
			bool isHasGame = false;

			if (PlayerPrefs.HasKey(_lastGameKey))
			{
				string lastGameData = PlayerPrefs.GetString(_lastGameKey);
				UndoData data = JsonUtility.FromJson<UndoData>(lastGameData);

				if (data != null && data.States.Count > 0)
				{
					isHasGame = true;
				}
			}

			return isHasGame;
		}

		/// <summary>
		/// Delete last game.
		/// </summary>
		public void DeleteLastGame()
		{
			PlayerPrefs.DeleteKey(_lastGameKey);
		}
	}

	[System.Serializable]
	public class UndoData
	{
		public bool IsCountable;
		public int AvailableUndoCounts;
		public int Score;
		public int Steps;
		public int Time;
		public List<UndoStates> States = new List<UndoStates>();
		public int[] CardsNums = new int[Public.CARD_NUMS];
	}

	[System.Serializable]
	public class UndoStates
	{
		public List<DeckRecord> DecksRecord = new List<DeckRecord>();

		public UndoStates(Deck[] decksStates, Card[] cardsStates)
		{
			foreach (var deck in decksStates)
			{
				DecksRecord.Add(new DeckRecord(deck.CardsArray));
			}
		}
	}

	[System.Serializable]
	public class CardRecord
	{
		public int CardType;
		public int CardNumber;
		public int Number;
		public int CardStatus;
		public int CardColor;
		public bool IsDraggable;
		public bool IsFaceDown;

		public int IndexZ;
		public int SiblingIndex;
		public Deck Deck;
		public Vector3 Position;

		public CardRecord(Card card)
		{
			CardType = card.CardType;
			CardNumber = card.CardNumber;
			Number = card.Number;
			CardStatus = card.CardStatus;
			CardColor = card.CardColor;
			IsDraggable = card.IsDraggable;
			IndexZ = card.IndexZ;
			Deck = card.Deck;
			SiblingIndex = card.transform.GetSiblingIndex();
			Position = card.transform.localPosition;
			IsFaceDown = card.isFaceDown;
		}
	}

	[System.Serializable]
	public class DeckRecord
	{
		public List<Card> Cards = new List<Card>();
		public List<CardRecord> CardsRecord = new List<CardRecord>();

		public DeckRecord(List<Card> cards)
		{
			Cards = new List<Card>(cards);

			foreach (var item in cards)
			{
				CardsRecord.Add(new CardRecord(item));
			}
		}
	}
}