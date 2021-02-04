using System.Collections;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public class AutoCompleteManager : MonoBehaviour
    {
        [Tooltip("The state of auto complete actions.")]
        public bool IsAutoCompleteActive = false;

        [Tooltip("Time between cards sets on correct place. (Transition)")]
        public float HintSetTransitionTime = 0.2f;

        [Header("Components")]
        public HintManager HintComponent;
        public GameObject AutoCompleteHintButtonObj;

        private IEnumerator _doubleClickAutoCompleteCoroutine;
        private IEnumerator _autoCompleteCoroutine;
        private bool _isCanComplete = true;
        public int faceDownCardsCount;
        public int packDeckCardsCount;
        public int wastePackCardsCount;

        public void Awake()
        {
            faceDownCardsCount = 21;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                CompleteGame();
            }
        }


        //Disable autocomplete button toggling constantly, only trigger when deck is empty and all cards are face up
        
        /// <summary>
        /// Activate autocomplete availability with button.
        /// </summary>
        public void ActivateAutoCompleteAvailability()
        {
            _isCanComplete = true;
            AutoCompleteHintButtonObj.SetActive(true);
            Debug.Log("Turn on Autocomplete Button");
        }

        
        /// <summary>
        /// Deactivate autocomplete availability with button.
        /// </summary>
        public void DectivateAutoCompleteAvailability()
        {
            AutoCompleteHintButtonObj.SetActive(false);
            _isCanComplete = false;
            Debug.Log("Turn off Autocomplete Button");
        }
        
        

        /// <summary>
        /// Call auto complete action.
        /// </summary>
        public void CompleteGame()
        {
            if (_isCanComplete)
            {
                _isCanComplete = false;
                StopAutoComplete();
                _autoCompleteCoroutine = CompleteCoroutine();
                StartCoroutine(_autoCompleteCoroutine);
            }
        }

        /// <summary>
        /// Auto complete actions in coroutine.
        /// </summary>
        private IEnumerator CompleteCoroutine()
        {
            IsAutoCompleteActive = true;
            HintComponent.UpdateAvailableForAutoCompleteCards();

            while (HintComponent.IsHasHint())
            {
                HintComponent.HintAndSet(HintSetTransitionTime);

                yield return new WaitWhile(() => HintComponent.IsHintProcess);
            }

            IsAutoCompleteActive = false;
            HintComponent.UpdateAvailableForDragCards();
        }

        /// <summary>
        /// Deactivate auto complete coroutine.
        /// </summary>
        private void StopAutoComplete()
        {
            if (_autoCompleteCoroutine != null)
            {
                StopCoroutine(_autoCompleteCoroutine);
            }
        }

        /// <summary>
		/// Deactivate double click auto complete coroutine.
		/// </summary>
		private void StopDoubleClickAutoComplete()
        {
            if (_doubleClickAutoCompleteCoroutine != null)
            {
                StopCoroutine(_autoCompleteCoroutine);
            }
        }

        /// <summary>
        /// Auto complete actions in coroutine.
        /// </summary>
        private IEnumerator DoubleClickAutoCompleteCoroutine(Card card)
        {
            IsAutoCompleteActive = true;
            _isCanComplete = false;
            HintComponent.UpdateAvailableForAutoCompleteCards();
            AutoCompleteHintButtonObj.SetActive(false);

            HintComponent.HintAndSet(HintSetTransitionTime);
            yield return new WaitWhile(() => HintComponent.IsHintProcess);

            _isCanComplete = true;
            IsAutoCompleteActive = false;
            HintComponent.UpdateAvailableForDragCards();
        }
    }
}