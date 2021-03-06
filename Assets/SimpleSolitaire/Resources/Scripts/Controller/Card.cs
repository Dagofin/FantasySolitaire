using SimpleSolitaire.Model.Enum;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
    public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public int CardType = 0;
        public int CardNumber = 0;
        public int Number = 0;
        public int CardStatus = 0;
        public int CardColor = 0;
        public bool IsDraggable = false;
        public bool IsDraggingCurrently = false;
        public bool isFaceDown = false;

        public int IndexZ;

        public CardLogic CardLogicComponent;
        public Image BackgroundImage;
        public Animator animator;
        public Animator childAnimator;

        private Vector3 _lastMousePosition = Vector3.zero;
        private Vector3 _offset;
        private IEnumerator _coroutine;
        private IEnumerator _doubleClickTimer;

        public RectTransform CardRect;
        private Vector3 _newPosition;
        private int _tapCount;
        private float _newTime = 0;
        //private float _maxDubbleTapTime = 0.5f;
        
        private bool IsDoubleCLickActionStarted = false;

        private Deck _deck;
        public Deck Deck
        {
            get
            {
                return _deck;
            }
            set
            {
                _deck = value;
            }
        }

        private readonly string _spadeTextureName = "spade";
        private readonly string _diamondTextureName = "diamond";
        private readonly string _clubTextureName = "club";
        private readonly string _heartTextureName = "heart";

        private void Start()
        {
            IndexZ = transform.GetSiblingIndex();
        }

        //private void Update()
        //{
            
        //}

        /// <summary>
        /// Set new background image for card.
        /// </summary>
        /// <param name="str"></param>
        public void SetBackgroundImg(string str)
        {
            Sprite tempType = Resources.Load("Sprites/cards/" + str, typeof(Sprite)) as Sprite;
            BackgroundImage.overrideSprite = tempType;
        }

        /// <summary>
        /// Show star particles.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ActivateParticle()
        {
            yield return new WaitForSeconds(0.1f);
            CardLogicComponent.ParticleStars.Play();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (CardLogicComponent.AutoCompleteComponent.IsAutoCompleteActive || !IsDraggable)
            {
                return;
            }

            IndexZ = transform.GetSiblingIndex();
            _deck.SetCardsToTop(this);
            CardLogicComponent.ParticleStars.transform.SetParent(gameObject.transform);
            CardLogicComponent.ParticleStars.transform.SetAsFirstSibling();

            _coroutine = ActivateParticle();
            StartCoroutine(_coroutine);

            IsDraggingCurrently = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (CardLogicComponent.AutoCompleteComponent.IsAutoCompleteActive || !IsDraggable)
            {
                return;
            }

            RectTransformUtility.ScreenPointToWorldPointInRectangle(CardRect, Input.mousePosition, eventData.enterEventCamera, out _newPosition);
            if (_lastMousePosition != Vector3.zero)
            {
                Vector3 offset = _newPosition - _lastMousePosition;
                transform.position += offset;
                CardLogicComponent.ParticleStars.transform.position = new Vector3(transform.position.x, transform.position.y - 20f, transform.position.z);
                _deck.SetPositionFromCard(this, transform.position.x, transform.position.y);
            }
            _lastMousePosition = _newPosition;
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if (CardLogicComponent.AutoCompleteComponent.IsAutoCompleteActive || !IsDraggable)
            {
                return;
            }

            transform.SetSiblingIndex(IndexZ);
            _lastMousePosition = Vector3.zero;

            if (_coroutine != null)
                StopCoroutine(_coroutine);
            CardLogicComponent.ParticleStars.Stop();

            CardLogicComponent.OnDragEnd(this);
            _deck.UpdateCardsPosition(false);
            IsDraggingCurrently = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(IsDraggingCurrently == true)
            {
                return;
            }
            //if the card is in the Main Deck
            if (Deck.Type == DeckType.DECK_TYPE_PACK)
            {
                if (CardLogicComponent.AutoCompleteComponent.IsAutoCompleteActive)
                {
                    return;
                }
                Deck.OnPointerClick(eventData);
                UnityEngine.Debug.Log("Clicked on Deck");
            }
            //if the card is in the card stacks
            else
            {
                //detect if the card is currently being dragged
                CardLogicComponent.HintManagerComponent.HintAndSetByDoubleClick(this);
            }
            
        }

        /// <summary>
        /// Get card texture by type.
        /// </summary>
        /// <param name="backTexture"></param>
        /// <returns> Texture string type</returns>
        public string GetTexture(string backTexture)
        {
            string texture = backTexture;
            if (CardStatus != 0)
            {
                texture = GetTypeName() + Number;
            }
            return texture;
        }

        /// <summary>
        /// Set default card status and background image <see cref="SetBackgroundImg"/>.
        /// </summary>
        public void RestoreBackView()
        {
            CardStatus = 0;
            SetBackgroundImg(CardShirtManager.Instance.ShirtName);
        }

        /// <summary>
        /// Set card position.
        /// </summary>
        /// <param name="position">New card position.</param>
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        /// <summary>
        /// Initialize card by number.
        /// </summary>
        /// <param name="cardNum">Card number.</param>
        public void InitWithNumber(int cardNum)
        {
            CardNumber = cardNum;
            CardType = Mathf.FloorToInt(cardNum / 13);
            if (CardType == 1 || CardType == 3)
            {
                CardColor = 1;
            }
            else
            {
                CardColor = 0;
            }
            Number = (cardNum % 13) + 1;
            CardStatus = 0;

            SetBackgroundImg(GetTexture(CardShirtManager.Instance.ShirtName));
        }

        /// <summary>
        /// Update card background <see cref="SetBackgroundImg"/>.
        /// </summary>
        public void UpdateCardImg()
        {
            SetBackgroundImg(GetTexture(CardShirtManager.Instance.ShirtName));
        }

        public void CardFlipAnim()
        {
            animator.SetBool("flip", true);
            childAnimator.SetBool("flip", true);
        }
        public void KillFlipAnim()
        {
            animator.SetBool("flip", false);
            childAnimator.SetBool("flip", false);
        }

        public string GetTypeName()
        {
            switch (CardType)
            {
                case 0:
                    return _spadeTextureName;
                case 1:
                    return _heartTextureName;
                case 2:
                    return _clubTextureName;
                case 3:
                    return _diamondTextureName;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Logic to track face down and face up cards
        /// </summary>
        public void SetFaceDown(bool firstTime, Deck deck)
        {
            if (firstTime)
            {
                isFaceDown = true;
                CardLogicComponent.faceDownCardsCount++;
            }

            else
            {
                if (isFaceDown == false)
                {
                    isFaceDown = true;
                    CardLogicComponent.faceDownCardsCount++;
                    //print("Flip Card Down/Deck: " + deck.transform.name + ". " + "Face Down Count is: " + CardLogicComponent.faceDownCardsCount);
                }

            }


        }
        public void SetFaceUp(bool firstTime)
        {
            //if (firstTime)
            //{


            //}

            if (isFaceDown == true)
            {
                CardFlipAnim();
                CardLogicComponent.faceDownCardsCount--;
                //print("Flip Card Up: Face Down Count is: " + CardLogicComponent.faceDownCardsCount);
                isFaceDown = false;
            }
        }
    }
}