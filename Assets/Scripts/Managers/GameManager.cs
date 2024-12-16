using System.Collections;
using DTT.Singletons;
using Gilzoide.FlexUi;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBehaviour<GameManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnStart()
    {
        NetworkChecker.Instance.OnStart();
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