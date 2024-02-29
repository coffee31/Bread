using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreateCustom : MonoBehaviour
{
    [SerializeField]
    float FWaitTime;
    [SerializeField]
    float MWaitTime;

    [SerializeField]
    float FPlayTime;
    [SerializeField]
    float MPlayTime;

    [SerializeField]
    bool FWait;
    [SerializeField]
    bool MainWait;

    [SerializeField]
    bool logcheck;
    [SerializeField]
    bool Fend;

    bool customCheck;
    bool[] deleteON = new bool[50];

    [SerializeField]
    public bool StandUp;


    [SerializeField]
    public GameObject[] customerPrefab; // 생성할 고객 프리팹
    public Transform spawnPoints; // 고객이 생성될 위치
    public GameObject newCustomer;

    public GameObject Info;


    [SerializeField]
    GameManager gameManager;

    public BreadUI breadScript;
    public float spawnInterval; // 고객 생성 간격
    private float nextSpawnTime = 0.0f;

    public float maxspawntime;
    bool GameStop;


    public GameObject panel;
    public GameObject startText;
    public GameObject EndText;
    public GameObject LastText;
    public Text RemainTime;


    /* Object Pool
    public int poolSize = 30; // 풀 크기
    private List<GameObject> objectPool = new List<GameObject>(); // 오브젝트 풀
    private GameObject newCustomer; // 생성된 고객 오브젝트

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = CustomCreate();
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }
    GameObject GetPooledObject()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (!objectPool[i].activeInHierarchy)
            {
                return objectPool[i]; //비활성화된 오브젝트를 가져옴
            }
        }
        GameObject obj = CustomCreate(); //모든 풀링이 활성화 된 경우 추가 생성
        objectPool.Add(obj);
        return obj;
    }
    GameObject CustomCreate()
    {
        return Instantiate(customerPrefab[Random.Range(0, customerPrefab.Length)]);
    }

    public void SpawnCustomer()
    {
        GameObject newCustomer = GetPooledObject();
        newCustomer.transform.position = spawnPoints.position;
        newCustomer.transform.rotation = spawnPoints.rotation;
        newCustomer.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Delete"))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            newCustomer.SetActive(false);
        }
    }


    */



    private void Awake()
    {
        GameStop = false;

        StandUp = true;
        FWait = true;
        logcheck = true;
        customCheck = true;
        Fend = false;

       
        MainWait = false;
        maxspawntime = 18;
        spawnInterval = Random.Range(1, maxspawntime);


        //시간 조절 [30 / 60 / 40 / 70]
        FWaitTime = 30.0f;
        FPlayTime = 60.0f;
        MWaitTime = 40.0f;
        MPlayTime = 70.0f;
    }

    private void Start()
    {
        for (int i = 0; i < deleteON.Length; i++)
        {
            deleteON[i] = false;
        }
        TextMsg();
    }

    private void Update()
    {
        if(!GameStop)
        {
            WaitTime();
        }
        checker();

    }

    private void SpawnCustomer()
    {
        if (spawnPoints == null)
        {
            Debug.LogError("스폰지점 없음 : 방어 코드");
            return;
        }
        else
        {
            newCustomer = Instantiate(customerPrefab[Random.Range(0, 3)], spawnPoints.position, spawnPoints.rotation);
        }

    }


    void WaitTime()
    {
        if(FWait)
        {
            FWaitTime -= Time.deltaTime;
            RemainTime.text = "남은 시간 : " + (int)FWaitTime;
        }
        
        if(FWaitTime <= 0)
        {
            FWait = false;
            FWaitTime = 0;
        }

        if (!FWait && !MainWait && !Fend)
        {
            for (int i = 0; i < deleteON.Length; i++)
            {
                deleteON[i] = true;
            }
            TextMsg();
            StandUp = false;
            GameManager.Instance.player.SetActive(false);
            FPlayTime -= Time.deltaTime;
            RemainTime.text = "남은 시간 : " + (int)FPlayTime;
            if (Time.time >= nextSpawnTime)
            {
                SpawnCustomer();
                // 다음 생성 간격을 랜덤하게 설정
                spawnInterval = Random.Range(3, maxspawntime);
                // 다음 고객 생성 시간 설정
                nextSpawnTime = Time.time + spawnInterval;
            }
        }
        if (FPlayTime <= 0)
        {
            MainWait = true;
            FPlayTime = 1;
            GameManager.Instance.MonthCount++;
        }

        if (MainWait && newCustomer == null)
        {
            deleteBread();
            if (Time.timeScale != 0)
            {
                Time.timeScale = 1;
            }

            Fend = true;
            customCheck = true;
            StandUp = true;
            TextMsg();
            logcheck = false;
            GameManager.Instance.player.SetActive(true);
            MWaitTime -= Time.deltaTime;
            RemainTime.text = "남은 시간 : " + (int)MWaitTime;
        }
        else if (MainWait && newCustomer != null)
        {
            if (Time.timeScale != 0)
            {
                Time.timeScale = 2;
            }
            
            TextMsg2();
        }


        if(MWaitTime <= 0)
        {
            MainWait = false;
            MWaitTime = 40.0f;
        }

        if (!MainWait && Fend)
        {
            for (int i = 0; i < deleteON.Length; i++)
            {
                deleteON[i] = true;
            }
            StandUp = false;
            TextMsg();
            GameManager.Instance.player.SetActive(false);
            MPlayTime -= Time.deltaTime;
            RemainTime.text = "남은 시간 : " + (int)MPlayTime;
            if (Time.time >= nextSpawnTime)
            {
                SpawnCustomer();
                spawnInterval = Random.Range(1, maxspawntime);
                nextSpawnTime = Time.time + spawnInterval;
            } 

        }

        if(MPlayTime <= 0)
        {
            MainWait = true;
            MPlayTime = 70.0f;
            GameManager.Instance.MonthCount++;
        }

    }


    void checker()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("day : " + GameManager.Instance.MonthCount);
            Debug.Log("maxday : " + GameManager.MaxMonths);
            Debug.Log("jewel : " + Cash.Jewel);
            Debug.Log("Total : " + GameManager.Instance.TotalSell);
            Debug.Log("spawnTime : " + maxspawntime);
        }
    }

    void TextMsg()
    {
        if(GameManager.Instance.MonthCount <= GameManager.MaxMonths)
        {
            if (logcheck && FWait || logcheck && MainWait)
            {
                logcheck = false;
                if (GameManager.AutoSave && GameManager.Instance.MonthCount > 1)
                {
                    GameManager.Instance.SaveList();
                    Debug.Log("세이브성공");
                }
                if (GameManager.Instance.MonthCount % 3 == 0 && maxspawntime >= 8)
                {
                    maxspawntime -= 2;
                }
                panel.SetActive(true);
                startText.SetActive(true);
                EndText.SetActive(false);
                LastText.SetActive(false);
                Invoke("TextOFF", 2.0f);

                if (GameManager.Instance.MonthCount > 1)
                {
                    Info.SetActive(true);
                    StartCoroutine(Disable(5.0f));
                }
            }
            if (!logcheck && !FWait && !MainWait)
            {
                logcheck = true;
                GameManager.Instance.CurrentSell = 0;
                GameManager.Instance.Revenue = 0;
                GameManager.Instance.Dispose = 0;
                GameManager.Instance.Expenditure = 0;

                panel.SetActive(true);
                startText.SetActive(false);
                EndText.SetActive(true);
                LastText.SetActive(false);
                Invoke("TextOFF", 2.0f);
            }
        }
        else
        {
            GameManager.Instance.GameEnd();
            GameStop = true;
        }
    }

    IEnumerator Disable(float time)
    {
        yield return new WaitForSeconds(time);
        if (Info.activeSelf)
            Info.SetActive(false);
    }

    public void InfoOpen()
    {
        if (!Info.activeSelf)
            Info.SetActive(true);
        else
            Info.SetActive(false);
    }


    void TextMsg2()
    {
        panel.SetActive(true);
        startText.SetActive(false);
        EndText.SetActive(false);
        LastText.SetActive(true);

        if (customCheck)
        {
            customCheck = false;
        }
            

    }

    public void TextOFF()
    {
        panel.SetActive(false);
        startText.SetActive(false);
        EndText.SetActive(false);
        LastText.SetActive(false);
    }

    void deleteBread()
    {
        for (int index = 0; index < breadScript.Sell.Length; index++)
        {
            if (deleteON[index] && breadScript.RecipeON[index])
            {
                deleteON[index] = false;
                int curentBread = gameManager.breadCounters[index];
                float deleteMax = curentBread * GameManager.Instance.MonthCount * 0.1f;
                if (GameManager.Instance.MonthCount > 10)
                {
                    deleteMax = curentBread;
                }
                int deleteBread = Random.Range(0, (int)deleteMax);
                gameManager.breadCounters[index] -= deleteBread;
                gameManager.UpdateBreadTexts();
                gameManager.charter.Gold -= deleteBread * GameManager.Instance.breadSinglePrices[index];
                gameManager.Dispose -= deleteBread;
                gameManager.Expenditure -= deleteBread * GameManager.Instance.breadSinglePrices[index];
            }
        }
    }

    public void SaveVariables()
    {
        PlayerPrefs.SetFloat("FWaitTimeKey", FWaitTime);
        PlayerPrefs.SetFloat("MWaitTimeKey", MWaitTime);
        PlayerPrefs.SetFloat("FPlayTimeKey", FPlayTime);
        PlayerPrefs.SetFloat("MPlayTimeKey", MPlayTime);
        PlayerPrefs.SetInt("FWaitKey", FWait ? 1 : 0); // bool 값을 int로 변환하여 저장
        PlayerPrefs.SetInt("MainWaitKey", MainWait ? 1 : 0);
        PlayerPrefs.SetInt("LogCheckKey", logcheck ? 1 : 0);
        PlayerPrefs.SetInt("FendKey", Fend ? 1 : 0);
        PlayerPrefs.SetInt("CustomCheckKey", customCheck ? 1 : 0);
        PlayerPrefs.Save(); // 변경사항 저장
    }
    public void LoadVariables()
    {
        FWaitTime = PlayerPrefs.GetFloat("FWaitTimeKey");
        if(FWaitTime < 5)
        {
            FWaitTime += 5;
        }

        MWaitTime = PlayerPrefs.GetFloat("MWaitTimeKey");
        if (MWaitTime < 5)
        {
            MWaitTime += 10;
        }


        FPlayTime = PlayerPrefs.GetFloat("FPlayTimeKey");
        MPlayTime = PlayerPrefs.GetFloat("MPlayTimeKey");
        FWait = PlayerPrefs.GetInt("FWaitKey") == 1 ? true : false;// PlayerPrefs에서 불러온 int 값을 bool로 변환
        MainWait = PlayerPrefs.GetInt("MainWaitKey") == 1 ? true : false;
        logcheck = PlayerPrefs.GetInt("LogCheckKey") == 1 ? true : false;
        Fend = PlayerPrefs.GetInt("FendKey") == 1 ? true : false;
        customCheck = PlayerPrefs.GetInt("CustomCheckKey") == 1 ? true : false;
    }

}
