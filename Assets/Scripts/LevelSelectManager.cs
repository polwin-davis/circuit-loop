using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public Button[] levelButtons;
    public LevelData[] levels;

    private SaveData saveData;

    void Start()
    {
        saveData = SaveSystem.Load();
        SetupButtons();
    }

    void SetupButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;

            levelButtons[i].interactable =
                levelIndex <= saveData.unlockedLevel;

            int index = i;
            levelButtons[i].onClick.AddListener(() =>
            {
                LoadLevel(levels[index]);
            });
        }
    }

    void LoadLevel(LevelData level)
    {
        SceneManager.LoadScene(level.sceneName);
    }
}
