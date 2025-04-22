using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }
    private bool isGameOver = false;
    public bool IsGameover
    {
        get { return isGameOver; }
        set
        {
            isGameOver = value;
            if (isGameOver)
                UIManager.Instance.ShowGameOverUI();
        }
    }

    private bool gameClear = false;
    public bool GameClear
    {
        get {
            return gameClear;
        }
        set
        {
            gameClear = value;
            if (gameClear)
            {
                Debug.Log("게임 클리어!");
                SetQuestComplete?.Invoke();
            }
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public UnityEvent SetQuestComplete;
}
