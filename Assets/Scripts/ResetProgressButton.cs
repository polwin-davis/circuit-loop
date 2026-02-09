using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetProgressButton : MonoBehaviour
{
    public string bootstrapSceneName = "Main";

    public void ResetProgress()
    {
        SaveSystem.ResetProgress();
        SceneManager.LoadScene(bootstrapSceneName);
    }
}
