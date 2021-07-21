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
		
		[SerializeField]
		private Animator myAnimator;

		[SerializeField]
		private GameManager gameManager;

        public void Awake()
        {
			//myAnimator = GetComponent<Animator>();
			//CheckUnlockAtStart(gameManager.playerLevel);
		}

        public void OnEnable()
        {
			CheckUnlockAtStart(gameManager.playerLevel);
		}

        public void Start()
        {

		}

		public void CheckUnlockAtStart(int playerLevel)
        {
			if(playerLevel >= unlockedAtLevel)
            {
				Unlock(false);
            }

			else if(playerLevel < unlockedAtLevel)
            {
				Lock();
				return;
            }
        }

		public void Unlock(bool firstTime)
        {
			isUnlocked = true;
			myAnimator.SetBool("isUnlocked", isUnlocked);
			LockedTile.SetActive(false);
			//change animation state to idle
			//myAnimator.Play("CurrentShirt");
			
        }

		public void Lock()
        {
			isUnlocked = false;
			levelText.text = unlockedAtLevel.ToString();
			LockedTile.SetActive(true);
			//change animation state to locked
			//myAnimator.Play("CollectionDialog_CardLocked");
			myAnimator.SetBool("isUnlocked", isUnlocked);
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