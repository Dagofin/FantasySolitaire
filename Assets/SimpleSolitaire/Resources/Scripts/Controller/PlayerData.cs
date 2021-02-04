using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    [System.Serializable]
    public class PlayerData
    {
        //Ads Data
        public bool permanentNoAds;
        public bool tempNoAds;
        public System.DateTime tempAdsStartTime;
        public int adsWatched;

        //Game Data
        public int gamesPlayed;
        public int gamesWon;

        public PlayerData(AdsManager adsManager)
        {
            permanentNoAds = adsManager.permanentNoAds;
            tempNoAds = adsManager.tempNoAds;
            tempAdsStartTime = adsManager.tempAdsStartTime;
            adsWatched = adsManager.adsWatched;
        }
    }
}
