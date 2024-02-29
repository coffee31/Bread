using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterManager : MonoBehaviour
{
    private static CounterManager instance;
    private bool isChecked = false;

    
    BreadUI breadScript;
    GameManager gameManager;
    SoundManager soundManager;
    CreateCustom CCustom;
    CustomerManager customerManager;


    private void Start()
    {
        breadScript = GameObject.Find("Canvas").GetComponent<BreadUI>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        soundManager = GameObject.Find("GameManager").GetComponent<SoundManager>();
        CCustom = GameObject.Find("Customspawn").GetComponent<CreateCustom>();
    }

    public void CheckCounter(int buttonIndex)
    {
        isChecked = true;

        // 인덱스에 해당하는 breadCounters 값을 줄임
        if (buttonIndex >= 0 && buttonIndex < breadScript.Sell.Length)
        {
            customerManager = CCustom.newCustomer.GetComponent<CustomerManager>();
            int decreaseAmount = customerManager.decreaseAmount;
            if (gameManager.breadCounters[buttonIndex] >= 0)
            {
                if (gameManager.breadCounters[buttonIndex] >= decreaseAmount)
                {
                    if (decreaseAmount != 0 && gameManager.breadCounters[buttonIndex] != 0)
                    {
                        soundManager.PlaySoundEffect();
                    }
                    gameManager.breadCounters[buttonIndex] -= decreaseAmount;
                    breadScript.Sell[buttonIndex] += decreaseAmount;
                    GameManager.Instance.TotalSell += decreaseAmount;
                    GameManager.Instance.CurrentSell += decreaseAmount;
                    customerManager.CustomerTypeAmount();
                }
                else
                {
                    if (decreaseAmount != 0 && gameManager.breadCounters[buttonIndex] != 0)
                    {
                        soundManager.PlaySoundEffect();
                    }
                    decreaseAmount = gameManager.breadCounters[buttonIndex];
                    gameManager.breadCounters[buttonIndex] = 0;
                    breadScript.Sell[buttonIndex] += decreaseAmount;
                    GameManager.Instance.TotalSell += decreaseAmount;
                }

                int goldIncrease = decreaseAmount * gameManager.breadSinglePrices[buttonIndex];
                gameManager.charter.Gold += goldIncrease;
                gameManager.Revenue += goldIncrease;
                gameManager.UpdateBreadTexts();
            }
        }
    }


    public bool IsChecked()
    {
        return isChecked;
    }

}
