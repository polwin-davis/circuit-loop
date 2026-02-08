using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int currentScore { get; private set; }

    private float decayRate;          // score lost per second
    private float decayAccumulator;   // accumulates fractional decay
    private bool isRunning;

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    private void Awake()
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

        // 🔒 Hide score during gameplay
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
            int pointsToReduce = Mathf.FloorToInt(decayAccumulator);
            decayAccumulator -= pointsToReduce;

            currentScore -= pointsToReduce;
            currentScore = Mathf.Max(currentScore, 0);
        }
    }

    public void StopScore()
    {
        isRunning = false;
    }

    // ✅ Called on level completion
    public void ShowScore()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
            scoreText.gameObject.SetActive(true);
        }
    }
}
