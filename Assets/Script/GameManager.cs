using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

/* Json
public class SaveData
{
    public int CharterGold;
    public int CharterJewel;
    public int Days;
    public int StandValue;
    public int Totalsell;
    public float spawnTime;
    public int BreadOpen;
    public int RecipeCheck;
    public int[] InteractableValue;
    public int[] Recipe;
    public int BreadMaxCount;
    public int CreateMax;
    public int SellCount;
    public int[] CurrentSellBread;
    public int[] BreadPrice;
    public int[] BreadCounter;
    public int[] BreadCreate;
}

*/

public class GameManager : MonoBehaviour
{
    /* 진열대 값*/
    public int standvalue = 0;

    // 싱글톤 인스턴스를 저장할 정적 변수
    private static GameManager instance;

    //세이브 관련 변수
    public static bool AutoSave = true;
    public static int LoadCheck = 0;

    //날짜 관련 변수
    public static int MaxMonths = 5; //12부터 시작 //Cash에 MonthDays도 12로 변경
    public int MonthCount = 1;

    //오브젝트 참조
    public GameObject CreateUI;
    public GameObject StandUI;

    
    //Infomation 관련 변수
    public int CurrentSell = 0;
    public int Revenue = 0;
    public int Dispose = 0;
    public int Expenditure = 0;

    //총 판매량
    public int TotalSell = 0;

    //진열대 및 레시피 관련 변수
    public int StandCount = 0;
    public int BreadSlotOpenCount;
    public int checkRange = 1;

    public int sellcount; //빵 가격 상승에 필요한 빵 판매 개수
    public int CreateMax = 200; // 최대 생산 가능한 빵 개수

    public int[] breadCounters = new int[50]; // 버튼 인덱스에 해당하는 Bread 개수 배열
    private int[] breadPrices = { 7200, 12300, 16800, 20000, 24600, 17890, 12100, 35000, };
    public int[] breadSinglePrices = { 720, 1230, 1680, 2000, 2460, 2240, 2420, 3500, };

    int[] breadCreate = new int[50];

    public Text[] breadText;

    [SerializeField]
    public Charter charter;
    public GameObject playerPrefab;
    public GameObject player;

    public BreadUI breadScript;
    public StateUI stateUI;
    public CreateCustom createCustom;

    public GameObject Success;
    public GameObject Fail;
    public GameObject NoLoadText;
    public GameObject EndMsg;

    // 다른 스크립트에서 접근할 수 있는 싱글톤 인스턴스 속성
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 씬에서 싱글톤 오브젝트를 찾습니다.
                instance = FindObjectOfType<GameManager>();

                // 씬에 싱글톤 오브젝트가 없으면 생성합니다.
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(GameManager).Name);
                    instance = singletonObject.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        CharCreate();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        standvalue = 0;
        MonthCount = 1;
        TotalSell = 0;
        sellcount = 100;
        BreadSlotOpenCount = 1;

        for (int i = 0; i < breadCreate.Length; i++)
        {
            breadCreate[i] = 12;
        }

        UpdateBreadTexts();

        if (LoadCheck == 1)
        {
            LoadList();
        }
    }


    public void UpdateBreadTexts()
    {
        string[] breadNames = { "Bread ", "Twist", "Muffin", "Donut", "SandWitch", "Baguette", "croissant", "Cake", };
        for (int i = 0; i < breadText.Length; i++)
        {
            breadText[i].text = breadNames[i] + ": " + breadCounters[i].ToString();
        }
    }

    public void Recipe(int buttonIndex)
    {
        // 여기서는 판매가격 증가 코드 및 생성개수 증가 코드
        if (breadScript.Sell[buttonIndex] >= sellcount)
        {
            Debug.Log(breadScript.Sell[buttonIndex]);
            breadScript.Sell[buttonIndex] -= sellcount;
            breadScript.Slots[buttonIndex].GetComponent<Button>().image.color = Color.white;
            sellcount *= 2;
            breadCreate[buttonIndex] *= 2;
            breadSinglePrices[buttonIndex] += breadSinglePrices[buttonIndex] / 2;
            Debug.Log("yes");
        }
        else { Debug.Log("no"); }
        Debug.Log(breadScript.Sell[buttonIndex]);
    }

    public void Buy(int buttonIndex)
    {
        int price = breadPrices[buttonIndex];
        if (charter.Gold >= price && breadScript.RecipeON[buttonIndex])
        {
            // 해당 Bread 개수 증가
            if (breadCounters[buttonIndex] < CreateMax)
            {
                charter.Gold -= price;
                breadCounters[buttonIndex] += breadCreate[buttonIndex];
            }
            if (breadCounters[buttonIndex] > CreateMax)
            {
                breadCounters[buttonIndex] = CreateMax;
            }
            UpdateBreadTexts();
        }
    }


    void CharCreate()
    {
        // 골드를 설정합니다.
        player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        player.name = "Player";
        charter = player.GetComponent<Charter>();
        charter.Gold = 3000000 + Cash.BaseGold; //300000
    }

    public void GameEnd()
    {
        //게임 종료
        Cash.Jewel += TotalSell;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("CharterJewel", Cash.Jewel); // Jewel
        PlayerPrefs.SetInt("MaxDayValue", MaxMonths);
        PlayerPrefs.SetInt("BaseMoney", Cash.BaseGold);
        PlayerPrefs.Save();

        EndMsg.SetActive(true);
        Invoke("MainMove", 10.0f); // 10초뒤 메인화면 이동
    }
    void MainMove()
    {
        EndMsg.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    #region Json Save
    /*
    public void SaveList()
    {
        if (createCustom.StandUp && DayCount <= MaxDays)
        {
            SaveData data = new SaveData();
            data.CharterGold = charter.Gold;
            data.CharterJewel = Cash.Jewel;
            data.Days = DayCount;
            data.StandValue = StandCount * 200000;
            data.Totalsell = TotalSell;
            data.spawnTime = createCustom.maxspawntime;
            data.BreadOpen = BreadSlotOpenCount;
            data.RecipeCheck = checkRange;
            data.InteractableValue = new int[breadScript.Slots.Length];
            data.Recipe = new int[breadScript.Slots.Length];
            data.BreadMaxCount = breadScript.BreadMaxPrice;
            data.CreateMax = CreateMax;
            data.SellCount = sellcount;
            data.CurrentSellBread = new int[breadScript.Sell.Length];
            data.BreadPrice = new int[breadSinglePrices.Length];
            data.BreadCounter = new int[breadCounters.Length];
            data.BreadCreate = new int[breadCreate.Length];

            for (int i = 0; i < breadScript.Slots.Length; i++)
            {
                data.InteractableValue[i] = breadScript.Slots[i].GetComponent<Button>().interactable ? 1 : 0;
                data.Recipe[i] = breadScript.RecipeON[i] ? 1 : 0;
            }

            for (int i = 0; i < breadScript.Sell.Length; i++)
            {
                data.CurrentSellBread[i] = breadScript.Sell[i];
            }

            for (int i = 0; i < breadSinglePrices.Length; i++)
            {
                data.BreadPrice[i] = breadSinglePrices[i];
            }

            for (int i = 0; i < breadCounters.Length; i++)
            {
                data.BreadCounter[i] = breadCounters[i];
            }

            for (int i = 0; i < breadCreate.Length; i++)
            {
                data.BreadCreate[i] = breadCreate[i];
            }

            string jsonData = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString("SaveData", jsonData);
            PlayerPrefs.Save();
            Success.SetActive(true);
            StartCoroutine(imageFalse(Success));
        }
        else
        {
            Fail.SetActive(true);
            StartCoroutine(imageFalse(Fail));
        }
    }


    public void LoadList()
    {
        if (PlayerPrefs.HasKey("SaveData"))
        {
            string jsonData = PlayerPrefs.GetString("SaveData");
            SaveData loadedData = JsonConvert.DeserializeObject<SaveData>(jsonData);

            // 로드한 데이터를 사용하여 게임 상태를 업데이트합니다.
            charter.Gold = loadedData.CharterGold;
            Cash.Jewel = loadedData.CharterJewel;
            DayCount = loadedData.Days;
            StandCount = loadedData.StandValue / 200000;
            TotalSell = loadedData.Totalsell;
            createCustom.maxspawntime = loadedData.spawnTime;
            BreadSlotOpenCount = loadedData.BreadOpen;
            checkRange = loadedData.RecipeCheck;

            for (int i = 0; i < breadScript.Slots.Length; i++)
            {
                breadScript.Slots[i].GetComponent<Button>().interactable = loadedData.InteractableValue[i] == 1 ? true : false;
                breadScript.RecipeON[i] = loadedData.Recipe[i] == 1 ? true : false;
            }

            breadScript.BreadMaxPrice = loadedData.BreadMaxCount;
            CreateMax = loadedData.CreateMax;
            sellcount = loadedData.SellCount;

            for (int i = 0; i < breadScript.Sell.Length; i++)
            {
                breadScript.Sell[i] = loadedData.CurrentSellBread[i];
            }

            for (int i = 0; i < breadSinglePrices.Length; i++)
            {
                breadSinglePrices[i] = loadedData.BreadPrice[i];
            }

            for (int i = 0; i < breadCounters.Length; i++)
            {
                breadCounters[i] = loadedData.BreadCounter[i];
            }

            for (int i = 0; i < breadCreate.Length; i++)
            {
                breadCreate[i] = loadedData.BreadCreate[i];
            }
        }
        else
        {
            Debug.Log("저장된 데이터가 없습니다.");
        }
    }

    */
    #endregion

    public void SaveList()
    {
        if(createCustom.StandUp && MonthCount <= MaxMonths)
        {
            standvalue = (StandCount * 200000);
            //캐릭터 골드, 주얼 및 날짜
            PlayerPrefs.SetInt("CharterGold", charter.Gold); //Charter Gold 
            PlayerPrefs.SetInt("CharterJewel", Cash.Jewel); // Jewel
            PlayerPrefs.SetInt("Days", MonthCount); //DayCount : 현재 날짜
            PlayerPrefs.SetInt("StandValue", standvalue);
            Bread.instance.SaveSlotCount();

            //빵 판매 개수
            PlayerPrefs.SetInt("Totalsell", TotalSell); //TotalSell : 총 판매한 빵 개수


            //손님 및 남은 시간 관련
            PlayerPrefs.SetFloat("spawnTime",  createCustom.maxspawntime);
            createCustom.SaveVariables();



            //레시피 관련 세이브 목록
            PlayerPrefs.SetInt("BreadOpen", BreadSlotOpenCount);// BreadSlotOpenCount: 레시피 오픈 가격
            PlayerPrefs.SetInt("RecipeCheck", checkRange);

            for (int i = 0; i < breadScript.Slots.Length; i++)
            {
                PlayerPrefs.SetInt("InteractableValue" + i, breadScript.Slots[i].GetComponent<Button>().interactable ? 1 : 0);
                PlayerPrefs.SetInt("Recipe" + i, breadScript.RecipeON[i] ? 1: 0 ); // 열린 레시피
            }

            //빵 제한 수량 해제 관련 목록
            PlayerPrefs.SetInt("BreadMaxCount", breadScript.BreadMaxPrice); //빵 제한수량 해제 금액
            PlayerPrefs.SetInt("CreateMax", CreateMax); //최대 빵 생성 제한


            //빵 가격 상승 관련 목록
            PlayerPrefs.SetInt("SellCount", sellcount); //sellcount : 빵 가격 상승에 필요한 빵 판매 개수

            for (int i = 0; i < breadScript.Sell.Length; i++)
            {
                PlayerPrefs.SetInt("CurrentSellBread" + i, breadScript.Sell[i]); // 현재 판매한 빵 개수
            }

            for (int i = 0; i < breadSinglePrices.Length; i++)
            {
                PlayerPrefs.SetInt("BreadPrice" + i, breadSinglePrices[i]); //breadSinglePrices 가격
            }

            // 현재 빵 개수 및 생성되는 빵 개수
            for (int i = 0; i < breadCounters.Length; i++)
            {
                PlayerPrefs.SetInt("BreadCounter" + i, breadCounters[i]); //breadCounters : 남은 빵 개수
            }

            for (int i = 0; i < breadCreate.Length; i++)
            {
                PlayerPrefs.SetInt("BreadCreate" + i, breadCreate[i]); //빵 생성 개수
            }

            PlayerPrefs.Save();
            Success.SetActive(true);
            StartCoroutine(imageFalse(Success));
        }
        else
        {
            Fail.SetActive(true);
            StartCoroutine(imageFalse(Fail));
        }
    }



    public void LoadList()
    {
        if (PlayerPrefs.HasKey("CharterGold"))
        {
            if (createCustom.StandUp && MonthCount < MaxMonths)
            {
                //캐릭터 골드, 주얼, 날짜 불러오기
                charter.Gold = PlayerPrefs.GetInt("CharterGold");
                charter.Gold += PlayerPrefs.GetInt("StandValue");
                Cash.Jewel = PlayerPrefs.GetInt("CharterJewel");
                MonthCount = PlayerPrefs.GetInt("Days");
                checkRange = PlayerPrefs.GetInt("RecipeCheck");

                Bread.instance.LoadSlotCount();

                //빵 판매 개수 불러오기
                TotalSell = PlayerPrefs.GetInt("Totalsell");

                // 손님 및 남은시간 관련
                createCustom.maxspawntime = PlayerPrefs.GetFloat("spawnTime");
                createCustom.LoadVariables();

                // 레시피 관련 세이브 목록 불러오기 
                BreadSlotOpenCount = PlayerPrefs.GetInt("BreadOpen");

                // 빵 제한 수량 해제 관련 목록 불러오기 
                breadScript.BreadMaxPrice = PlayerPrefs.GetInt("BreadMaxCount");
                CreateMax = PlayerPrefs.GetInt("CreateMax");

                // 빵 가격 상승 관련 목록 불러오기
                sellcount = PlayerPrefs.GetInt("SellCount");

                for (int i = 0; i < breadScript.Sell.Length; i++)
                {
                    breadScript.Sell[i] = PlayerPrefs.GetInt("CurrentSellBread" + i);
                }

                for (int i = 0; i < breadSinglePrices.Length; i++)
                {
                    breadSinglePrices[i] = PlayerPrefs.GetInt("BreadPrice" + i);
                }

                // 현재 빵 개수 및 생성되는 빵 개수 불러오기
                for (int i = 0; i < breadCounters.Length; i++)
                {
                    breadCounters[i] = PlayerPrefs.GetInt("BreadCounter" + i);
                }

                for (int i = 0; i < breadCreate.Length; i++)
                {
                    breadCreate[i] = PlayerPrefs.GetInt("BreadCreate" + i);
                }

                // Recipe 배열 로드
                for (int i = 0; i < breadScript.Slots.Length; i++)
                {
                    bool recipeValue = PlayerPrefs.GetInt("Recipe" + i, 0) == 1 ? true : false;
                    breadScript.RecipeON[i] = recipeValue;

                    bool interactableValue = PlayerPrefs.GetInt("InteractableValue" + i, 0) == 1 ? true : false;
                    breadScript.Slots[i].GetComponent<Button>().interactable = interactableValue;
                }


                UpdateBreadTexts();
                Success.SetActive(true);
                StartCoroutine(imageFalse(Success));
            }
            else
            {
                Fail.SetActive(true);
                StartCoroutine(imageFalse(Fail));
            }
        }
        else
        {
            NoLoadText.SetActive(true);
            Fail.SetActive(true);
            StartCoroutine(imageFalse(Fail));
            StartCoroutine(imageFalse(NoLoadText));
        }
    }


    IEnumerator imageFalse(GameObject image)
    {
        yield return new WaitForSeconds(1.0f);
        if (image.activeSelf)
            image.SetActive(false);
    }

    public void MoblieUION()
    {
        if (charter.CreateON)
        {
            charter.Create.SetActive(true);
        }
        else if (charter.StandON)
        {
            charter.Stand.SetActive(true);
        }
    }
}
