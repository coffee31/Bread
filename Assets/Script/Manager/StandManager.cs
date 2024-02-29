using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class StandManager : MonoBehaviour
{

    public GameObject prefab;
    Vector3 spacing = new Vector3(-9.5f, 0, 0);
    Vector3 upspacing = new Vector3(0, 6.5f, 0);
    GameObject stand;

    CreateCustom CCustom;

    int maxStand = 44;
    int standPrice = 200000;


    [SerializeField]
    int Row = 0;
    [SerializeField]
    int column = 0;
    


    // Start is called before the first frame update
    void Start()
    {
        CCustom = GameObject.Find("Customspawn").GetComponent<CreateCustom>();
        stand = Instantiate(prefab, transform.position, Quaternion.identity);
        Row++;
        column = 0;

    }
    public void StandPlus()
    {
        if (CCustom.StandUp && maxStand > 0 && GameManager.Instance.charter.Gold >= standPrice)
        {
            if (Row <= 8)
            {
                stand = Instantiate(prefab, transform.position, Quaternion.identity);
                stand.transform.position = stand.transform.position + (spacing * Row) + (upspacing * column);
                GameManager.Instance.StandCount++;
                Row++;
                
            }
            else if (Row > 8)
            {
                Row = 0;
                column++;
                stand = Instantiate(prefab, transform.position, Quaternion.identity);
                stand.transform.position = stand.transform.position + spacing * Row + upspacing * column;
                GameManager.Instance.StandCount++;
                Row++;
            }
        }
        if(maxStand > 0 && GameManager.Instance.charter.Gold >= standPrice)
        {
            GameManager.Instance.charter.Gold -= standPrice;
            maxStand--;
        }
    }
}
