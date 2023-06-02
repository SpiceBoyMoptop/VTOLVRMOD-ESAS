using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESAMFDInt : MonoBehaviour
{
    public MFDManager mfdManager;
    public List<MFD> mfds;
    public List<MFDPage> mfdPages;
    public List<MFDPage.MFDButtonInfo> mfdButtonInfos;
    private Actor ThisVehicle;
    public MFDPage newMFDHome;

    void Start()
    {
        ThisVehicle = gameObject.GetComponentInParent<Actor>();
        var MFDMS = ThisVehicle.gameObject.GetComponentsInChildren<MFDManager>(true);
        foreach(MFDManager MFDM in MFDMS)
        {
            if(MFDM.name == "MFDManager")
            {
                mfdManager = MFDM;
            }
        }

        var MFDS = mfdManager.mfds;
        foreach(MFD mfd in MFDS)
        {
            mfds.Add(mfd);
        }

        var MFDPS = ThisVehicle.gameObject.GetComponentsInChildren<MFDPage>(true);
        foreach(MFDPage MFDP in MFDPS)
        {
            mfdPages.Add(MFDP);
        }
        AddMFDButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DebugCheck()
    {
        Debug.Log("ESAS MFD Debug Check");
    }

    [ContextMenu("AddMFDButton")]
    public void AddMFDButton()
    {
    foreach(MFDPage page in mfdPages)
    {
        if(page.name == "MFDHome")
        {
            var pageMFD = page.gameObject.GetComponentInParent<MFD>(true);
            List<MFDPage.MFDButtonInfo> newButtonList = new List<MFDPage.MFDButtonInfo>();
            foreach(MFDPage.MFDButtonInfo button in page.buttons)
            {
                newButtonList.Add(button);
            }
            foreach(MFDPage.MFDButtonInfo button in mfdButtonInfos)
            {
                newButtonList.Add(button);
            }
            MFDPage.MFDButtonInfo[] newButtonArray = new MFDPage.MFDButtonInfo[newButtonList.Count];
            for(int i = 0; i < newButtonList.Count; i++)
            {
                newButtonArray[i] = newButtonList[i];
            }
            page.buttons = newButtonArray;
            page.Initialize(pageMFD);
        }
    }

    }
}
test