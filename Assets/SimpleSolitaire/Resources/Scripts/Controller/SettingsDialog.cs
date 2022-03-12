using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsDialog : MonoBehaviour
{
    public Image newCollectionMarker;
    public bool isNewCollection;
    public bool hasSeenNewCollection;


    //
    void Start()
    {
        /*if(isNewCollection == true)
        {
            newCollectionMarker.gameObject.SetActive(true);
        }
        else
        {
            newCollectionMarker.gameObject.SetActive(false);
        }*/

        //
    }

    public void NewCollectionCheck()
    {
        if(isNewCollection == true && hasSeenNewCollection == false)
        {
            //set the flag to active
        }

    }

    public void ClearCollectionMarker()
    {
        //newCollectionMarker.gameObject.SetActive(false);
        //hasSeenNewCollection = true;
    }

    public void SetCollectionCheck()
    {
        //isNewCollection = true;
        //hasSeenNewCollection = false;
    }
}
