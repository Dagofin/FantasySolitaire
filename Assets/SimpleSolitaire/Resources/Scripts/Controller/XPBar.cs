using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SimpleSolitaire
{
    public class XPBar : MonoBehaviour
    {
        
        public int maximumXP;
        public int currentXP;
        public int currentLevel;
        public int xpToAdd;
        public Image mask;
        //public Image fill;
        //public Color color;
        [SerializeField]
        private Text levelUpDialogText;
        [SerializeField]
        private Animator levelUpAnim;
        [SerializeField]
        private GameObject levelUpAnimObj;

        public Text currentXPText;
        public Text levelText;

        //lerp variables
        float lerpDuration = 1;
        float valueToLerp;

        public void Awake()
        {
            if(levelUpAnimObj!= null)
            {
                levelUpAnimObj.SetActive(false);
            }

        }

        public void InitializeLevelData(int setXP, int setMaxXP, int setLevel)
        {
            currentXP = setXP;
            maximumXP = setMaxXP;
            currentLevel = setLevel;
            GetCurrentFill(currentXP);
            currentXPText.text = currentXP.ToString() + " / " + maximumXP;
            levelText.text = currentLevel.ToString();
        }

        public void GetCurrentFill(float setXP)
        {
            float fillAmount = (float)setXP / (float)maximumXP;
            mask.fillAmount = fillAmount;
            currentXPText.text = setXP.ToString() + " / " + maximumXP;
        }

        //public IEnumerator LerpXPBar(int targetXP)
        /*public IEnumerator LerpXPBar()
        {
            print("lerp the XP bar");
            float timeElapsed = 0;
            //targetXP = setXP;
            startXP = currentXP;

            int xpToLevelUp = startXP - maximumXP;
            int xpToAddTemp = xpToAdd - startXP;
            int xpLevelUpRemainder = xpToAdd - xpToLevelUp;

            //increase the XP bar over the course of 1 second from the current XP to the new XP according to XP gained.
            while (timeElapsed < lerpDuration)
            {


                valueToLerp = (int)Mathf.Lerp(startXP, xpToAdd, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;

                //set the xpBarFill to the current lerp value
                GetCurrentFill(valueToLerp);
                currentXP = (int)valueToLerp;
                //currentXPText.text = currentXP.ToString() + " / " + maximumXP;

                print("valueToLerp: " + valueToLerp);

                //check to see if the XP has maxed out
                if(valueToLerp >= maximumXP)
                {
                    print("xp bar max level");
                    xpToLevelUp = startXP - maximumXP;
                    xpToAddTemp = xpToAdd - startXP;
                    xpLevelUpRemainder = xpToAdd - xpToLevelUp;

                    xpToAdd = xpLevelUpRemainder;

                    //level up 

                    //reset the XP bar to 0
                    currentXP = 0;
                    //currentXPText.text = currentXP.ToString();
                    GetCurrentFill(0f);

                    //resume filling with remaining XP
                    //LerpXPBar(xpLevelUpRemainder);
                    StartCoroutine(LerpXPBar());

                    yield break;
                }

                print("lerp up finished");
                yield return null;
            }

            valueToLerp = xpToAddTemp;

            print("valueToLerp: " + valueToLerp);

            GetCurrentFill(valueToLerp);
            //currentXPText.text = currentXP.ToString();
        }*/

        //function to lerp the xpBar
        //startXP is going to always be equal to playerXP's pre-levelupXP
        //endXP is going to be equal to 
        public IEnumerator AnimateXPBar(int startXP, int endXP, float waitDuration)
        {
            yield return new WaitForSeconds(waitDuration);

            float timeElapsed = 0;


            while (timeElapsed < lerpDuration)
            {
                valueToLerp = (int)Mathf.Lerp(startXP, endXP, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;
                //set the XP bar's fill to the current value and update the text
                GetCurrentFill(valueToLerp);

                //when this is finished, continue the coroutine
                yield return null;
            }
            
            //safety check to set the bar/text to the correct ending value
            GetCurrentFill(endXP);
        }

        public void LevelUp(int tempLevelText)
        {
            currentLevel = tempLevelText;
            

            levelUpDialogText.text = currentLevel.ToString();
            //play level up dialog animation
            levelUpAnimObj.SetActive(true);
            levelUpAnim.SetBool("playLevelUp", true);
            //levelUpAnim.Play("LevelUp");

            levelText.text = currentLevel.ToString();
        }

        public void KillLevelUpAnim()
        {
            levelUpAnimObj.SetActive(false);
        }

    }
}
