using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
	public class CardShirtManager : MonoBehaviour
	{
		public static CardShirtManager Instance;

		[Header("BackGrounds")]
		public Image GameBG;
		public GameObject CurrentBackGround;
		public List<VisualiseElement> BackGroundImages;

		public string BackGroundName;
		public string DefaultBackGround;

		[Header("Shirts")]
		public List<Sprite> Shirts;
		public List<VisualiseElement> ShirtsImage;

		public GameObject CurrentShirt;

		public string ShirtName;
		public string DefaultShirtName;

		public CardLogic CLComponent;

		public bool FirstOpenSettings = true;

		private const string CURRENT_SHIRT = "CurrentShirt";
		private const string CURRENT_BG = "CurrentBackGround";

		private readonly string _animationActionTrigger = "Action";
		private readonly string _animationNonActionTrigger = "NonAction";

		private void Awake()
		{
			Instance = this;
			SetImageObjects();
			GetSettings();
		}

		/// <summary>
		/// Get settings values from player prefs.
		/// </summary>
		public void GetSettings()
		{
			if (PlayerPrefs.HasKey(CURRENT_SHIRT))
			{
				ShirtName = PlayerPrefs.GetString(CURRENT_SHIRT);
			}
			else
			{
				ShirtName = DefaultShirtName;
				PlayerPrefs.SetString(CURRENT_SHIRT, DefaultShirtName);
			}

			if (PlayerPrefs.HasKey(CURRENT_BG))
			{
				BackGroundName = PlayerPrefs.GetString(CURRENT_BG);
			}
			else
			{
				BackGroundName = DefaultBackGround;
				PlayerPrefs.SetString(CURRENT_BG, DefaultBackGround);
			}
		}

		private void Start()
		{
			SetShirtForCards(ShirtsImage.Find(x => x.name == ShirtName).VisualImage);
			SetBackGround(BackGroundImages.Find(y => y.name == BackGroundName).VisualImage);
		}

		/// <summary>
		/// Apply settings for game.
		/// </summary>
		public void SetSettings()
		{
			ActionWithAnimationForObjects(ShirtsImage, ShirtName, 10f);
			ActionWithAnimationForObjects(BackGroundImages, BackGroundName, 20f);
		}

		/// <summary>
		/// Set up background of game.
		/// </summary>
		/// <param name="bgObject">Background image component</param>
		public void SetBackGround(Image bgObject)
		{
			CurrentBackGround = bgObject.gameObject;
			BackGroundName = CurrentBackGround.name;
			PlayerPrefs.SetString(CURRENT_BG, BackGroundName);

			GameBG.sprite = bgObject.sprite;

			ActionWithAnimationForObjects(BackGroundImages, BackGroundName, 20f);
		}

		/// <summary>
		/// Set up shirt for card objects.
		/// </summary>
		/// <param name="shirtObject">Shirt image component.</param>
		public void SetShirtForCards(Image shirtObject)
		{
			CurrentShirt = shirtObject.gameObject;
			ShirtName = CurrentShirt.name;
			PlayerPrefs.SetString(CURRENT_SHIRT, ShirtName);

			ActionWithAnimationForObjects(ShirtsImage, ShirtName, 10f);

			foreach (var item in CLComponent.CardsArray)
			{
				item.BackgroundImage.sprite = shirtObject.sprite;
				item.UpdateCardImg();
			}
		}

		/// <summary>
		/// Activate animation which highlight choosen background <see cref="currentBackground"/>.
		/// </summary>
		/// <param name="curList"></param>
		/// <param name="currentBackground">Current choosen background</param>
		/// <param name="_name">Name of shirt or background</param>
		private void ActionWithAnimationForObjects(List<VisualiseElement> curList, string _name, float sizeOfIncrease)
		{
			curList.ForEach(a =>
			{
				var animatorOfCurList = GetObjectAnimator(a.gameObject);

				if (a.name == _name)
				{
					a.ActivateCheckmark();
					animatorOfCurList.enabled = true;
					animatorOfCurList.speed = 1f;
					animatorOfCurList.SetTrigger(_animationActionTrigger);
				}
				else
				{
					a.DeactivateCheckmark();
					animatorOfCurList.enabled = false;
					a.transform.localRotation = Quaternion.identity;
				}
			});
		}

		/// <summary>
		/// Get animator of object.
		/// </summary>
		/// <param name="go">Object</param>
		/// <returns>Animator of object.</returns>
		public Animator GetObjectAnimator(GameObject go)
		{
			return go.GetComponent<Animator>();
		}

		/// <summary>
		/// Set up image components.
		/// </summary>
		public void SetImageObjects()
		{
			for (int i = 0; i < Shirts.Count; i++)
			{
				ShirtsImage[i].VisualImage.sprite = Shirts[i];
				ShirtsImage[i].name = Shirts[i].name;
			}
		}
	}
}