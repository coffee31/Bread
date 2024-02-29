using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bread : MonoBehaviour
{
    //Bread ½Ì±ÛÅæ
    public static Bread instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    //µ¨¸®°ÔÀÌÆ®
    public delegate void OnSlotCountChange(int val);
    public OnSlotCountChange onSlotCountChange;

    private int slotCnt;
    public int SlotCnt
    {
        get => slotCnt;
        set
        {
            slotCnt = value;
            onSlotCountChange.Invoke(slotCnt);
        }
    }

    void Start()
    {
        SlotCnt = 1;
    }
    public void SaveSlotCount()
    {
        PlayerPrefs.SetInt("SlotCountKey", slotCnt);
        PlayerPrefs.Save();
    }

    public void LoadSlotCount()
    {
        slotCnt = PlayerPrefs.GetInt("SlotCountKey");
    }
}
