using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
    public class BattlePassController : MonoBehaviour
    {
        [SerializeField]
        private GameManager _gameManagerComponent;
        [SerializeField]
        private ScrollRectSnap scrollRectSnap;

        [SerializeField]
        private RectTransform container;

        [SerializeField]
        private List<GameObject> passBars;

        [SerializeField]
        private GameObject currentPassUnit;


        // Start is called before the first frame update
        void Start()
        {
            InitializePassBars();
        }

        public void InitializePassBars()
        {
            //add each battlepass unit to the battlepass container list
            foreach (Transform child in container.transform)
            {
                passBars.Add(child.gameObject);
                child.GetComponent<BattlepassUnit>().InitializeLevelData(_gameManagerComponent.playerXP, _gameManagerComponent.playerLevel);
            }

            //set each battlepass units XP bar based on Player Level and XP
            //for(int i = 0; i < passBars.Count; i++)
            //{
                //passBars[i].GetComponent<BattlepassUnit>().InitializeLevelData(_gameManagerComponent.playerXP, _gameManagerComponent.playerLevel);
            //}

            //set currentPassUnit to the Player's current level, set XP of passUnits accordingly
            currentPassUnit = passBars[_gameManagerComponent.playerLevel - 1].gameObject;
            SetPassUnitStats(currentPassUnit.GetComponent<BattlepassUnit>());
            scrollRectSnap.SnapTo(_gameManagerComponent.playerLevel-1);
        }
        
        public void SetPassUnitStats(BattlepassUnit tempCurrentPassUnit)
        {
            tempCurrentPassUnit.currentXP = _gameManagerComponent.playerXP;
            //tempCurrentPassUnit.currentXPText.text = tempCurrentPassUnit.currentXP.ToString();
        }

        private void Update()
        {
            //print(container.localPosition);
        }


    }
}
