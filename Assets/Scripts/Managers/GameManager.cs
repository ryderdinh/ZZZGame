using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void StartConnectToDeathMode()
    {
        StartCoroutine(LoadConnectToDeathScene());
    }

    private IEnumerator LoadConnectToDeathScene()
    {
        var asyncLoad = SceneManager.LoadSceneAsync("ConnectToDeathScene");

        while (!asyncLoad.isDone) yield return null;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("HomeScene");
    }
}