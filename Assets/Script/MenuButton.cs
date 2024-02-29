using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public GameObject Image;
    public GameObject CashShop;

    public TMP_Text text1;
    public TMP_Text text2;
    public TMP_Text text3;

    private void Update()
    {
        text1.text = "MonthDays : " + GameManager.MaxMonths;
        text2.text = "BaseGold : " + Cash.BaseGold;
        text3.text = "Jewel : " + Cash.Jewel;
    }
    public void SceneChange()
    {
        SceneManager.LoadScene("GameScene");
        GameManager.LoadCheck = 0;
    }

    public void SaveDelete()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadScene()
    {
        if (PlayerPrefs.HasKey("CharterGold"))
        {
            SceneManager.LoadScene("GameScene");
            GameManager.LoadCheck = 1;
        }
        else
        {
            Image.SetActive(true);
            if(Image.activeSelf)
            {
                Invoke("FalseImage", 1.0f);
            }
        }
    }

    public void ShopON()
    {
        if (CashShop.activeSelf)
            CashShop.SetActive(false);
        else
            CashShop.SetActive(true);
    }

    void FalseImage()
    {
        Image.SetActive(false);
    }
}
