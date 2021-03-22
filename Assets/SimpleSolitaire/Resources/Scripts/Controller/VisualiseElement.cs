using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
	public class VisualiseElement : MonoBehaviour
	{
		public Image VisualImage;
		public Image CheckMark;

		public GameObject LockedTile;
		public Text levelText;

		public int unlockedAtLevel;
		public bool isUnlocked;
		
		//[SerializeField]
		private Animator myAnimator;

		[SerializeField]
		private GameManager gameManager;

        public void Start()
        {
			CheckUnlockAtStart(gameManager.playerLevel);
			myAnimator = GetComponent<Animator>();
        }

		public void CheckUnlockAtStart(int playerLevel)
        {
			if(playerLevel >= unlockedAtLevel)
            {
				Unlock(false);
            }

			else
            {
				Lock();
            }
        }

		public void Unlock(bool firstTime)
        {
			isUnlocked = true;

			LockedTile.SetActive(false);
			//change animation state to idle
			myAnimator.Play("CurrentShirt");
			//myAnimator.enabled = false;
        }

		public void Lock()
        {
			isUnlocked = false;
			levelText.text = unlockedAtLevel.ToString();
			LockedTile.SetActive(true);
			//change animation state to locked
			myAnimator.Play("CollectionDialog_CardLocked");
		}


		public void ActivateCheckmark()
		{
			CheckMark.enabled = true;
		}

		public void DeactivateCheckmark()
		{
			CheckMark.enabled = false;
		}
	}
}