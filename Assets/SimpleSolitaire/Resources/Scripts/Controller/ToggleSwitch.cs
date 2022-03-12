using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ToggleSwitch : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private bool _isOn = false;
    public bool isOn //allows other scripts to access value without changing it
    {
        get
        {
            return _isOn;
        }
    }
    
    [SerializeField]
    private RectTransform toggleIndicator;
    [SerializeField]
    private Image backgroundImage;

    [SerializeField]
    private Color onColor;
    [SerializeField]
    private Color offColor;
    private float offX;
    private float onX;
    [SerializeField]
    private float tweenTime = 0.25f;

    //private AudioSource audiosource;

    public delegate void ValueChanged(bool value);
    public event ValueChanged valueChanged;

    // Start is called before the first frame update
    void Awake()
    {
        offX = toggleIndicator.anchoredPosition.x; // start position
        onX = backgroundImage.rectTransform.rect.width - toggleIndicator.rect.width;
        //audiosource = this.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        ToggleColor(isOn); // make sure the switch is set correctly 
        MoveIndicator(isOn);
    }

    private void Toggle(bool value)
    {
        if(value != isOn)
        {
            _isOn = value;

            ToggleColor(isOn);
            MoveIndicator(isOn);

            //if (playSFX)
                //audiosource.Play();

            if (valueChanged != null)
                valueChanged(isOn);
        }
    }

    private void ToggleColor(bool value)
    {
        if (value)
            backgroundImage.DOColor(onColor, tweenTime);
        else
            backgroundImage.DOColor(offColor, tweenTime);
    }

    private void MoveIndicator(bool value)
    {
        if (value)
            toggleIndicator.DOAnchorPosX(onX, tweenTime);
        else
            toggleIndicator.DOAnchorPosX(offX, tweenTime);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Toggle(!isOn); // flips the switch when clicked
    }
}
