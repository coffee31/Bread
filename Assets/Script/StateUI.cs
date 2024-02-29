using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StateUI : MonoBehaviour
{

    public Text Gold;
    public Text Jewel;

    public TMP_Text Month;
    public TMP_Text SellCount;
    public TMP_Text Revenue;
    public TMP_Text Dispose;
    public TMP_Text Expenditure;
    public TMP_Text Total;



    [SerializeField]
    Charter managerGold;

    public GameObject stateUI;


    // Start is called before the first frame update
    void Start()
    {
        managerGold = GameManager.Instance.player.GetComponent<Charter>();
    }

    // Update is called once per frame
    void Update()
    {
        GoldUpdate();
        TextUpdate();
    }
    public void stateON()
    {
        if(stateUI != null)
        {
            if(stateUI.activeSelf)
            {
                stateUI.SetActive(false);
            }
            else 
            { 
                stateUI.SetActive(true); 
            }
        }    

    }

    public void GoldUpdate()
    {
        Gold.text = "Gold : " + managerGold.Gold;
        Jewel.text = "Jewel : " + Cash.Jewel;
    }

    public void TextUpdate()
    {
        Month.text = "Month : " + GameManager.Instance.MonthCount;
        SellCount.text = "Bread Sell Count : " + GameManager.Instance.CurrentSell;
        Revenue.text = "Revenue : " + GameManager.Instance.Revenue;
        Dispose.text = "Dispose : " + GameManager.Instance.Dispose;
        Expenditure.text = "Expenditure : " + GameManager.Instance.Expenditure;
        Total.text = "Total : " + (GameManager.Instance.Revenue + GameManager.Instance.Expenditure);
    }


}
