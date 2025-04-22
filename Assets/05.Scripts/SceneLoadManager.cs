using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] Animator spaceShipAni;
    readonly int hashIsLobby = Animator.StringToHash("IsLobby");
    readonly int hashStart = Animator.StringToHash("Start");
    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name != "MainScene")
        spaceShipAni.SetBool(hashIsLobby, true);
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name=="StartScene" && Input.GetKey(KeyCode.Space))
        {
            spaceShipAni.SetTrigger(hashStart);
            Invoke("LoadMainScene", 0.35f);
        }
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            spaceShipAni.SetBool(hashIsLobby, false);
        }
    }

    private void LoadMainScene()
    {
        spaceShipAni.SetBool(hashIsLobby, false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ShowLobby()
    {
        SceneManager.LoadScene("StartScene");
    }
}
