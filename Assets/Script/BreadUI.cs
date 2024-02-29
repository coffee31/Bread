using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreadUI : MonoBehaviour
{
    public Slot[] Slots;
    public Transform SlotLook;

    Bread bread;
    [SerializeField]
    GameManager gameManager;

    private int[] sell = new int[50];
    public int[] Sell
    {
        get { return sell; }
        set { sell = value; }
    }

    public bool[] RecipeON = new bool[50];
    public int BreadMaxPrice = 500000;

    private void Awake()
    {
        for (int i = 1; i < RecipeON.Length; i++)
        {
            RecipeON[i] = false;
        }
        RecipeON[0] = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        bread = Bread.instance;
        gameManager = GameManager.Instance;
        //Slots 배열에는 Slot이라는 컴포넌트가 있는 객체들을 차례로 넣음
        //<여기서는 SlotLook이라는 부모와 그 자식 오브젝트들 중 Slot을 가진 객체들을 가져옴>
        Slots = SlotLook.GetComponentsInChildren<Slot>();
        bread.onSlotCountChange += SlotChange;
    }

    private void SlotChange(int val)
    {
        if (bread.SlotCnt > 1 && gameManager.charter.Gold >= 50000 * gameManager.BreadSlotOpenCount)
        {
            gameManager.charter.Gold -= 50000 * gameManager.BreadSlotOpenCount;
            gameManager.BreadSlotOpenCount *= 2;
            GameManager.Instance.checkRange++;
            for (int i = 0; i < Slots.Length; i++)
            {
                if (i < bread.SlotCnt)
                {
                    Slots[i].GetComponent<Button>().interactable = true;
                    RecipeON[i] = true; // 해당 버튼에 해당하는 RecipeON 값을 true로 변경
                }
                else
                {
                    Slots[i].GetComponent<Button>().interactable = false;
                    RecipeON[i] = false;
                }
            }
        }
        else if(bread.SlotCnt == 1)
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                if (i < bread.SlotCnt)
                {
                    Slots[i].GetComponent<Button>().interactable = true;
                    RecipeON[i] = true;
                }

                else 
                {
                    Slots[i].GetComponent<Button>().interactable = false;
                    RecipeON[i] = false;
                }
            }
        }
    }

    public void AddSlot()
    {
        if(bread.SlotCnt < 8)
        {
            bread.SlotCnt++;
        }
            
    }
    private void Update()
    {
        for (int buttonIndex = 0; buttonIndex < sell.Length; buttonIndex++)
        {
            selling(buttonIndex);
        }
    }
    void selling(int buttonindex)
    {
        if (sell[buttonindex] >= GameManager.Instance.sellcount)
        {
            // 아래는 버튼의 색상을 변경하는 코드
            Slots[buttonindex].GetComponent<Button>().image.color = Color.yellow;
        }
    }


    public void AddMax()
    {
        if(gameManager.charter.Gold >= BreadMaxPrice)
        {
            gameManager.charter.Gold -= BreadMaxPrice;
            BreadMaxPrice *= 2;

            GameManager.Instance.CreateMax += 200;
        }

    }



}
