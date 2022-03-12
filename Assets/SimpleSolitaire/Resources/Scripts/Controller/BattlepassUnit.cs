using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire
{
    public class BattlepassUnit : MonoBehaviour
    {
        public int maximumXP;
        public int minimumXP;
        public int currentXP;
        public int currentLevel;
        public int myLevel;
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

        [SerializeField]
        private Image freeReward;
        [SerializeField]
        private Image paidReward;
        [SerializeField]
        private Image freeLock;
        [SerializeField]
        private Image paidLock;

        public Text currentXPText;
        public Text levelText;

        //lerp variables
        float lerpDuration = 1;
        float valueToLerp;

        public void Awake()
        {
            //if (levelUpAnimObj != null)
            //{
            //    levelUpAnimObj.SetActive(false);
            //}

        }

        //public void InitializeLevelData(int setXP, int setMaxXP, int setLevel)
        public void InitializeLevelData(int setXP, int setLevel)
        {
            //levelText.text = myLevel.ToString();
            
            if(myLevel < setLevel)
            {
                mask.fillAmount = 1;
                UnlockItems();
            }
            else if( myLevel == setLevel)
            {
                currentXP = setXP;
                GetCurrentFill(currentXP);
            }
            else
            {
                mask.fillAmount = 0;
            }

            //currentXP = setXP;
            //maximumXP = setMaxXP;
            //currentLevel = setLevel;
            //GetCurrentFill(currentXP);
            //currentXPText.text = currentXP.ToString() + " / " + maximumXP;
            //levelText.text = currentLevel.ToString();
        }

        public void GetCurrentFill(float setXP)
        {
            float fillAmount = (float)setXP / (float)maximumXP;
            mask.fillAmount = fillAmount;
            //currentXPText.text = setXP.ToString() + " / " + maximumXP;
        }

        public void UnlockItems()
        {
            freeLock.enabled = false;

            //if(purchase has been made){
                paidLock.enabled = false;
            //}
        }

        /*
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


        //-------------------------------------------------------------------------------------------------------------------------------
        /*public void LevelUp(int tempLevelText)
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
        }*/

    }
}
