using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScrollRectSnap : MonoBehaviour
{
    public RectTransform panel; //hold the scrollpanel
    public RectTransform panelParent;
    public Transform[] items; //items to scroll
    public RectTransform snapCenter; //center to compare distance to
    public float speed = 15f;

    private float[] distance; //all buttons distance to center
    private bool dragging = false; //will be true while dragging
    private int itemDistance; //holds distance between items
    private int minItemNum; //holds the index of the item closest to center

    private void Start()
    {
        int itemLength = items.Length;
        distance = new float[itemLength];

        //Get distance between buttons
        itemDistance = (int)Mathf.Abs(items[1].GetComponent<RectTransform>().localPosition.y - items[0].GetComponent<RectTransform>().localPosition.y);
        //itemDistance = (int)Mathf.Abs(items[1].GetComponent<RectTransform>().anchoredPosition.y - items[0].GetComponent<RectTransform>().anchoredPosition.y);
    }

    private void Update()
    {
        for (int i = 0; i < items.Length; i++)
        {
            distance[i] = Mathf.Abs(snapCenter.transform.position.y - items[i].transform.position.y);
        }

        float minDistance = Mathf.Min(distance); // get the minimum distance

        for(int a = 0; a < items.Length; a++)
        {
            if(minDistance == distance[a])
            {
                minItemNum = a;
            }
        }

        if (!dragging)
        {
            LerpToITem(minItemNum * -itemDistance);
        }
    }

    public void SnapTo(int snapIndex)
    {
        float newY = snapIndex * -itemDistance;
        
        Vector2 newPos = new Vector2(panel.localPosition.x, newY);

        panel.localPosition = newPos;

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
        Canvas.ForceUpdateCanvases();
        
    }

    private void LerpToITem(int pos)
    {
        float newY = Mathf.Lerp(panel.localPosition.y, pos, Time.deltaTime * speed);
        //float newY = Mathf.Lerp(panel.anchoredPosition.y, pos, Time.deltaTime * 10f);
        Vector2 newPos = new Vector2(panel.localPosition.x, newY);
        //Vector2 newPos = new Vector2(panel.anchoredPosition.x, newY);

        panel.localPosition = newPos;
        //panel.anchoredPosition = newPos;
        
    }

    public void StartDrag()
    {
        dragging = true;
    }

    public void EndDrag()
    {
        dragging = false;
    }
}
