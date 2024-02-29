using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public float moveSpeed;
    private bool hasReachedCounter = false;
    public float avoidDistance = 0.5f;
    private bool isDeleted = false;
    public int decreaseAmount;
    

    private Transform targetCounter;
    public Transform deletetrans;
    private bool hasCheckedAllCounters = false;

    Animator animator;

    public enum CustomerType
    {
        Man,
        Girl,
        Kid,
    }
    public CustomerType customerType;


    private bool[] checkedCounters; // �� �������� üũ ���θ� ������ �迭

    private void Start()
    {
        animator = GetComponent<Animator>();
        CustomerTypeAmount();

        FindNextCounter();
        deletetrans = GameObject.Find("delete").GetComponent<Transform>();

        // ��� �����뿡 ���� üũ ���� �迭 �ʱ�ȭ
        CounterManager[] counters = FindObjectsOfType<CounterManager>();
        checkedCounters = new bool[counters.Length];
    }

    private void Update()
    {
        if (!hasReachedCounter && targetCounter != null && !isDeleted)
        {
            Vector3 direction = (targetCounter.position - transform.position).normalized;
            Vector3 movement = direction * moveSpeed * Time.deltaTime;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, avoidDistance, LayerMask.GetMask("Stand"));
            if (hit.collider != null)
            {
                Vector3 avoidDirection = Vector3.up;
                movement = avoidDirection * moveSpeed * Time.deltaTime;
            }
            transform.Translate(movement);
            if (direction.y >= 0)
            {
                animator.SetFloat("Move", 1.0f);
            }
            else if (direction.y < 0)
            {
                animator.SetFloat("Move", -1.0f);
            }

            if (Vector3.Distance(transform.position, targetCounter.position) < 4.0f)
            {
                hasReachedCounter = true;
                
                for(int i = 0; i < GameManager.Instance.checkRange; i++)
                {
                    targetCounter.GetComponent<CounterManager>().CheckCounter(i);
                }
                // ���� �������� �ε����� ã�Ƽ� üũ ���� �迭�� ����
                int currentIndex = Array.IndexOf(checkedCounters, false);
                if (currentIndex >= 0)
                {
                    checkedCounters[currentIndex] = true;
                }
                // ��� �����밡 üũ�Ǿ����� Ȯ���ϰ� ��� üũ�Ǿ��ٸ� �մ� ����
                if (!AllCountersChecked())
                {
                    StartCoroutine(MoveToNextCounterWithDelay(2.0f));
                }
                else
                {
                    hasCheckedAllCounters = true;
                    StartCoroutine(MoveToDestination(deletetrans.position, 2.0f));
                    animator.SetFloat("Move", -1.0f);
                }
            }
        }
    }

    private bool AllCountersChecked()
    {
        foreach (bool isChecked in checkedCounters)
        {
            if (!isChecked)
            {
                return false;
            }
        }
        return true;
    }
    private void FindNextCounter()
    {
        CounterManager[] counters = FindObjectsOfType<CounterManager>();

        if (targetCounter != null)
        {
            int currentIndex = -1;
            for (int i = 0; i < counters.Length; i++)
            {
                if (counters[i].transform == targetCounter)
                {
                    currentIndex = i;
                    break;
                }
            }
            if (currentIndex == -1)
            {
                return;
            }
            int nextIndex = (currentIndex + 1) % counters.Length;
            targetCounter = counters[nextIndex].transform;
        }
        else
        {
            targetCounter = counters[0].transform;
        }
        hasReachedCounter = false;
    }
    private IEnumerator MoveToDestination(Vector3 destination, float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 direction = (destination - transform.position).normalized;
        float distanceToDestination = Vector3.Distance(transform.position, destination);

        while (distanceToDestination > 2f && !isDeleted)
        {
            Vector3 movement = direction * moveSpeed * Time.deltaTime;
            transform.Translate(movement);
            distanceToDestination = Vector3.Distance(transform.position, destination);
            yield return null;
        }
        // �������� ������ �Ŀ� isDeleted�� true�� �����մϴ�.
        isDeleted = true;
        yield break;
    }

    private IEnumerator MoveToNextCounterWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        FindNextCounter();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Delete"))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            Destroy(gameObject, 3.0f);
        }
    }

    public void CustomerTypeAmount()
    {
        switch (customerType)
        {
            case CustomerType.Man:
                moveSpeed = 40f;
                decreaseAmount = UnityEngine.Random.Range(0, 14);
                break;
            case CustomerType.Girl:
                moveSpeed = 30f;
                decreaseAmount = UnityEngine.Random.Range(0, 11);
                break;
            case CustomerType.Kid:
                moveSpeed = 20f;
                decreaseAmount = UnityEngine.Random.Range(0, 9);
                break;
        }
    }

}
