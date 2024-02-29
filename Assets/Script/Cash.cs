using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cash : MonoBehaviour
{
    public static int Jewel = 500000;
    public static int BaseGold;
    public int MaxMonthPlus;



    void Start()
    {
        MaxMonthPlus = 1;
        BaseGold = 0;

        if (PlayerPrefs.HasKey("BaseMoney"))
            Cash.BaseGold = PlayerPrefs.GetInt("BaseMoney");
        else
            Cash.BaseGold = 0;

        if (PlayerPrefs.HasKey("MaxDayValue"))
            GameManager.MaxMonths = PlayerPrefs.GetInt("MaxDayValue");
        else
            GameManager.MaxMonths = 5;
    }

    public void Dayplus()
    {
        if(Jewel >= 20000)
        {
            Jewel -= 20000;
            GameManager.MaxMonths += MaxMonthPlus;
            PlayerPrefs.SetInt("MaxDayValue", GameManager.MaxMonths);
        }

    }

    public void BaseMoney()
    {
        if(Jewel >= 5000)
        {
            Jewel -= 5000;
            BaseGold += 100000;
            PlayerPrefs.SetInt("BaseMoney", BaseGold);
        }
    }
    




}
