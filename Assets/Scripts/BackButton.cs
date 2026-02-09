using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    [Tooltip("Scene name for the main menu / level select")]
    public string mainSceneName = "Main";

    public void GoBackToMain()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}
