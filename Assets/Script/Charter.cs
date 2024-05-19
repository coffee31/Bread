using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class Charter : MonoBehaviour
{
    //�ӵ� �� ���� ���� ����
    float moveSpeed = 15f; // �̵� �ӵ� ���� ����
    Rigidbody2D rigid;


    // ī�޶� ����
    Camera mainCamera;
    float minX, maxX, minY, maxY;

     //UI ���� ����
    public bool CreateON = false;
    public bool StandON = false;

    //UI ȭ�� ����
    public GameObject Create;
    public GameObject Stand;
    
    private int gold;
    public int Gold
    {
        get { return gold; }
        set { gold = value; }
    }


    private Vector2 mobileInput = Vector2.zero; // ���̽�ƽ �Է� ����



    void Start()
    {
        GameObject canvasObject = GameObject.Find("Canvas");
        Create = canvasObject.transform.Find("CreateUI").gameObject;
        Stand = canvasObject.transform.Find("StandUI").gameObject;
        mainCamera = Camera.main;
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UION();
    }
    void FixedUpdate()
    {
        Moving();
        CamSet();
    }

    void Moving()
    {
        // Ű �Է��� �����Ͽ� �̵� ���͸� �����մϴ�.
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector2 moveDirection = mobileInput != Vector2.zero ? mobileInput : new Vector2(moveX, moveY);
        rigid.velocity = moveDirection * moveSpeed;

    }
    void CamSet()
    {
        // ��ġ�� ī�޶� ����Ʈ ��� ���� ����
        minX = mainCamera.ViewportToWorldPoint(new Vector2(0, 0)).x;
        maxX = mainCamera.ViewportToWorldPoint(new Vector2(1, 0)).x;
        minY = mainCamera.ViewportToWorldPoint(new Vector2(0, 0)).y;
        maxY = mainCamera.ViewportToWorldPoint(new Vector2(0, 1)).y;

        Vector2 currentPosition = transform.position;


        currentPosition.x = Mathf.Clamp(currentPosition.x, minX, maxX);
        currentPosition.y = Mathf.Clamp(currentPosition.y, minY, maxY);

        transform.position = currentPosition;
    }


    // UIâ ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Create"))
        {
            CreateON = true;

        }
        else if (collision.gameObject.CompareTag("BreadCreate"))
        {
            StandON = true;
        }
     }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject != null)
        {
            if (collision.gameObject.CompareTag("Create"))
            {
                if (Create != null)
                {
                    Create.SetActive(false);
                    CreateON = false;
                }
            }
            else if (collision.gameObject.CompareTag("BreadCreate"))
            {
                if (Stand != null)
                {
                    Stand.SetActive(false);
                    StandON = false;
                }
            }
        }
    }
    void UION()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CreateON)
            {
                Create.SetActive(true);
            }
            else if (StandON)
            {
                Stand.SetActive(true);
            }
        }
    }




    public void MobileMove(Vector2 input)
    {
        mobileInput = input;
    }


}

