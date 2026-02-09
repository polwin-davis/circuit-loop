using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int currentScore { get; private set; }

    private float decayRate;
    private float decayAccumulator;
    private bool isRunning;

    public TextMeshProUGUI scoreText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Init(LevelData levelData)
    {
        currentScore = levelData.maxScore;
        decayRate = levelData.scoreDecayPerSecond;
        decayAccumulator = 0f;
        isRunning = true;

        if (scoreText != null)
            scoreText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isRunning || currentScore <= 0)
            return;

        decayAccumulator += decayRate * Time.deltaTime;

        if (decayAccumulator >= 1f)
        {
            int reduce = Mathf.FloorToInt(decayAccumulator);
            decayAccumulator -= reduce;
            currentScore = Mathf.Max(currentScore - reduce, 0);
        }
    }

    public void StopScore()
    {
        isRunning = false;
    }

    public void ShowScore()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
            scoreText.gameObject.SetActive(true);
        }
    }
}
