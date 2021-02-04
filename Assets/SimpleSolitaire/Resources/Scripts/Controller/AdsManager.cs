using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller {
    public class AdsManager : MonoBehaviour, IUnityAdsListener
    {
        string placement = "rewardedVideo";
        [SerializeField]
        private UndoPerformer undoPerformer;

        [SerializeField]
        private GameObject tempNoAdsButton;
        [SerializeField]
        private GameObject tempNoAdsTimer;
        [SerializeField]
        private Text tempNoAdsTimerText;

        //Ads Data
        public bool permanentNoAds;
        public bool tempNoAds;
        public System.DateTime tempAdsStartTime;
        public float noAdsDurationSeconds = 1800f;
        public int adsWatched;

        [SerializeField]
        private float noAdsTimer = 0f;

        public bool adsActive = true;


        // Start is called before the first frame update
        void Start()
        {
            Advertisement.AddListener(this);
            Advertisement.Initialize("3921877", true);

            InitiateAdsData();

            //Advertisement.Show(placement);
        }

        private void InitiateAdsData()
        {
            PlayerData data = SaveLoadManager.LoadAdsData();

            if(data != null)
            {
                permanentNoAds = data.permanentNoAds;
                tempNoAds = data.tempNoAds;
                tempAdsStartTime = data.tempAdsStartTime;
                adsWatched = data.adsWatched;

                CheckNoAdsOnStart();
            }



            //if (permanentNoAds == true || tempNoAds == true)
            //{
            //    adsActive = false;
            //}
        }

        void Update()
        {
            if (tempNoAds == true)
            {
                noAdsTimer -= Time.deltaTime;

                int seconds = (int)(noAdsTimer % 60);
                int minutes = (int)(noAdsTimer / 60) % 60;

                string noAdsTimerString = string.Format("{0:00}:{1:00}", minutes, seconds);

                tempNoAdsTimerText.text = noAdsTimerString;

                //When the no ads timer expires, turn ads back on and save to AdsDAta
                if(noAdsTimer <= 0)
                {
                    //turn ads back on
                    tempNoAds = false;
                    //save to AdsData
                    SaveLoadManager.SaveAdsInfo(this);

                    //revert NoAdsTimer to NoAdsButton
                    //disable NoAdsTimer
                    tempNoAdsTimer.SetActive(false);
                    //enable NoAdsButton
                    tempNoAdsButton.SetActive(true);
                }
            }
            else
                return;
        }

        public void ShowAd(string p)
        {
            //if (adsActive == true)
            if(permanentNoAds == true || tempNoAds == true)
            {
                return;
            }
            else
            {
                Advertisement.Show(p);
                //return;
            }
        }

        public void CheckNoAdsOnStart()
        {
            //check if tempNoAds was true when the Player left the game, if so:
            if (tempNoAds == true)
            {
                //check current DateTime against tempAdsStartTime DateTime
                int secondsPassed = SecondsSinceTempNoAds(tempAdsStartTime, System.DateTime.Now);

                //if more than 30 minutes have passed, turn ads back on
                if(secondsPassed >= 1800)
                {
                    tempNoAds = false;
                    //Save to AdsData
                    SaveLoadManager.SaveAdsInfo(this);
                }
                //if less than 30 minutes have passed, set the timer to the appropriate time and begin counting down
                else if(secondsPassed < 1800)
                {
                    //set noAdsTimer to 30 minutes
                    noAdsTimer = noAdsDurationSeconds;
                    //subract elapsed seconds from timer
                    noAdsTimer = noAdsTimer - secondsPassed;
                    
                    //toggle button/timer
                    //disable TempDisableAdsButton
                    tempNoAdsButton.SetActive(false);
                    //enable TempDisabledAdsTimer
                    tempNoAdsTimer.SetActive(true);
                }

            }

            //if tempNoAds is false, then ads weren't off when the Player exited the game, no action.
            else
                return;
        }

        public static int SecondsSinceTempNoAds(System.DateTime from, System.DateTime to)
        {
            if(from > to)
            {
                Debug.LogError("From isn't after to");
            }

            //Trim to minutes
            System.DateTime fromTrimmed = new System.DateTime(from.Year, from.Month, from.Day, from.Hour, from.Minute, from.Second);
            System.DateTime toTrimmed = new System.DateTime(to.Year, to.Month, to.Day, to.Hour, to.Minute, to.Second);

            int seconds = (int)(toTrimmed - fromTrimmed).TotalSeconds;

            return seconds;
        }

        public void TempDisableAds()
        {
            //disable TempDisableAdsButton
            tempNoAdsButton.SetActive(false);
            //enable TempDisabledAdsTimer
            tempNoAdsTimer.SetActive(true);

            //set timer to 30 minutes
            noAdsTimer = noAdsDurationSeconds;
            //disable ads, start timer
            tempNoAds = true;
            //Set the time that no ads was started, save to the AdsData
            tempAdsStartTime = System.DateTime.Now;
            tempNoAds = true;

            //set Undos to Unlimited
            undoPerformer.IsCountable = false;
            //Save Undo Info
            //---------------------------------------------------------------------------------------------------------------------

            SaveLoadManager.SaveAdsInfo(this);
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            //use this to check that the placement finished correctly and award stuff accordingly
            //example
            //if(placementID == "addUndos")
            //check if finished, add undos
            if (placementId == "rewardedVideo")
            {
                if (showResult == ShowResult.Finished)
                {
                    //add undos
                    undoPerformer.UpdateUndoCounts();
                }
                else if (showResult == ShowResult.Failed)
                {
                    //inform the Player they must watch the full ad to get undos
                }
            }
            //Watch a rewarded video to temporarily disable ads
            else if(placementId == "tempDisableAds")
            {
                if(showResult == ShowResult.Finished)
                {
                    //check if finished, disable ads for 30 minutes
                    TempDisableAds();
                }
                else if(showResult == ShowResult.Failed)
                {
                    //inform the Player they must watch the full ad to disable ads temporarily
                }

            }
        }

        public void OnUnityAdsDidError(string message)
        {

        }

        public void OnUnityAdsDidStart(string placementId)
        {

        }

        public void OnUnityAdsReady(string placementId)
        {

        }
    }
}
