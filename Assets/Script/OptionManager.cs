using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public GameObject Option;
    public AudioSource audioSource;
    public AudioSource SfxSource;

    private void Start()
    {
        audioSource.volume = 0.4f;
        SfxSource.volume = 0.4f;
    }

    void Update()
    {
        OptionUI();
    }

    public void Continue()
    {
        Option.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        SfxSource.volume = volume;
    }


    public void QuitGame()
    {
        if(GameManager.Instance.MonthCount > GameManager.MaxMonths)
        {
            SceneManager.LoadScene("MainMenu");
            Option.SetActive(false);
            Time.timeScale = 1.0f;
        }
        else
        {
            GameManager.Instance.SaveList();
            SceneManager.LoadScene("MainMenu");
            Option.SetActive(false);
            Time.timeScale = 1.0f;
        }

    }


    public void AutoSaveON(bool check)
    {
        if(check)
        {
            GameManager.AutoSave = true;
            Debug.Log("자동 세이브ON");
        }
        else
        {
            GameManager.AutoSave = false;
            Debug.Log("자동 세이브OFF");
        }
    }


    public void OptionUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Option != null)
            {
                if (Option.activeSelf)
                {
                    Option.SetActive(false);
                    Time.timeScale = 1.0f;

                }
                else
                {
                    Option.SetActive(true);
                    Time.timeScale = 0;
                }
            }
        }
    }

}
