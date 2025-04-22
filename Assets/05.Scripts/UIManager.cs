using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<UIManager>();
            
            return instance;
        }
    }

    

    public GameObject[] UI;
    public Slider healthSlider; //체력을 표시할 UI 슬라이더
    [SerializeField] private GameObject Player;
    [SerializeField] private Image[] Quests;
    [SerializeField] private Image[] ItemIcons;
    [SerializeField] private GameObject[] explainTexts;
    [SerializeField] private GameObject explainImg;

    bool findRequestor = false;
    bool haveKey = false;
    int batteryCount = 0;
    int questClearCount = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this) Destroy(gameObject);
        foreach (var item in ItemIcons)
        {
            item.gameObject.SetActive(false);
        }
        batteryCount = 0;
        
        DisableInfoTxt();
        explainImg.SetActive(true);
        explainTexts[0].SetActive(true);
    }

    private void DisableInfoTxt()
    {
        foreach (var explainTxt in explainTexts)
        {
            explainTxt.SetActive(false);
        }
        explainImg.SetActive(false);
    }

    public void ToggleHelpUI(int idx, bool isShow)
    {
        UI[idx].SetActive(isShow);
    }

    public void ResetPlayerHpBar()
    {
        healthSlider.maxValue = Player.GetComponent<PlayerHealth>().startingHealth;

        healthSlider.value = Player.GetComponent<PlayerHealth>().health;

    }

    public void UpdateItemIcons(ItemData item)
    {
        int itemID = int.Parse(item.itemID.ToString());
        ItemIcons[itemID].gameObject.SetActive(true);
    }

    public void UpdateQuest(ItemData item)
    {
        int itemID = int.Parse(item.itemID.ToString());
        if (itemID == 0)
        {
            haveKey = true;
            Quests[2].gameObject.SetActive(false);
            questClearCount++;
        }
        else if(itemID >= 1 && itemID <= 4)
        {
            Quests[1].GetComponentInChildren<TextMeshProUGUI>().text = $"Find spaceship batteries ({++batteryCount}/4)";
            if (batteryCount == 4)
            {
                Quests[1].gameObject.SetActive(false);
                questClearCount++;
            }
        }
        else if(itemID == 5)
        {
            if(findRequestor == true)
            {
                if (haveKey == false)
                {
                    DisableInfoTxt();
                    explainImg.SetActive(true);
                    explainTexts[2].SetActive(true);
                }
                else
                {
                    if (batteryCount < 4)
                    {
                        DisableInfoTxt();
                        explainImg.SetActive(true);
                        explainTexts[3].SetActive(true);
                    }
                    else //구조요청자 찾았고, 연료, 키 모두 찾았으면 게임 클리어 실행
                    {
                        Quests[3].gameObject.SetActive(false);
                        questClearCount++;
                        foreach(var ui in UI)
                        {
                            ui.SetActive(false);
                        }
                        GameObject.Find("Drake").SetActive(false);
                        GameObject.Find("AlienSolider").SetActive(false);
                        GameManager.Instance.GameClear = true;
                    }
                }
            }
            else
            {
                DisableInfoTxt();
                explainImg.SetActive(true);
                explainTexts[4].SetActive(true);
            }
        }
        else if(itemID == 6)
        {
            Quests[0].gameObject.SetActive(false);
            DisableInfoTxt();
            explainImg.SetActive(true);
            explainTexts[1].SetActive(true);
            findRequestor = true;
            questClearCount++;
        }
    }

    public void ShowGameOverUI()
    {
        ToggleHelpUI(5, true);
    }
}
