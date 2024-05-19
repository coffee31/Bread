using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileManager : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    RectTransform rect;
    Vector2 Drag = Vector2.zero;
    public RectTransform value;
    public Charter player;
    GameManager gameManager;


    private void Start()
    {
        rect = GetComponent<RectTransform>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.FindWithTag("Player").GetComponent<Charter>();
    }


    public void OnDrag(PointerEventData eventData)
    {
        // 조이스틱 넓이 및 이동 범위
        Vector2 Drag = (eventData.position - rect.anchoredPosition) / (rect.sizeDelta.x * 0.5f);
        if(Drag.magnitude > 1)
        {
            Drag = Drag.normalized;
        }
        value.anchoredPosition = Drag * (rect.sizeDelta.x * 0.5f);
        player.MobileMove(Drag);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        OnDrag(eventData);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        value.anchoredPosition = Vector2.zero;
        player.MobileMove(Vector2.zero);
    }
}
