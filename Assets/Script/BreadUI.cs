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
        //Slots �迭���� Slot�̶�� ������Ʈ�� �ִ� ��ü���� ���ʷ� ����
        //<���⼭�� SlotLook�̶�� �θ�� �� �ڽ� ������Ʈ�� �� Slot�� ���� ��ü���� ������>
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
                    RecipeON[i] = true; // �ش� ��ư�� �ش��ϴ� RecipeON ���� true�� ����
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
            // �Ʒ��� ��ư�� ������ �����ϴ� �ڵ�
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
