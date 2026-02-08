using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public LevelData levelData;

    [Header("Tiles")]
    public List<NodeTile> tiles = new List<NodeTile>();

    private bool levelCompleted;

    [Header("Celebration")]
    public ParticleSystem completionParticles;
    public AudioSource completionAudio;

    [Header("UI")]
    public GameObject levelUnlockPanel;
    public TMPro.TextMeshProUGUI levelText;
    public GameObject lockImage;

    [Header("Timing")]
    public float celebrationDelay = 1f;
    public float unlockDelay = 1f;
    public float loadNextLevelDelay = 1f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (ScoreManager.Instance != null && levelData != null)
            ScoreManager.Instance.Init(levelData);
    }

    public void CheckLevel()
    {
        if (levelCompleted)
            return;

        foreach (NodeTile tile in tiles)
        {
            if (!IsConnectionValid(tile))
            {
                ResetTileColors();
                return;
            }
        }

        if (!IsSingleClosedLoop())
        {
            ResetTileColors();
            return;
        }

        levelCompleted = true;
        SetAllTilesValid();
        OnLevelCompleted();
    }

    void OnLevelCompleted()
    {
        // 🛑 Stop score + show final score
        ScoreManager.Instance?.StopScore();
        ScoreManager.Instance?.ShowScore();

        SaveBestScore();

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int currentLevelNumber = currentIndex;
        int nextLevelNumber = currentLevelNumber + 1;

        SaveData data = SaveSystem.Load();
        if (nextLevelNumber > data.unlockedLevel)
        {
            data.unlockedLevel = nextLevelNumber;
            SaveSystem.Save(data);
        }

        completionParticles?.Play();
        completionAudio?.Play();

        ShowLevelUnlockUI(nextLevelNumber);

        Invoke(nameof(RemoveLockIcon), celebrationDelay);
        Invoke(nameof(HideUnlockUI), celebrationDelay + unlockDelay);
        Invoke(nameof(LoadNextLevel),
            celebrationDelay + unlockDelay + loadNextLevelDelay);
    }

    void SaveBestScore()
    {
        SaveData data = SaveSystem.Load();
        int score = ScoreManager.Instance.currentScore;

        LevelScore entry =
            data.levelScores.Find(x => x.levelNumber == levelData.levelNumber);

        if (entry == null)
        {
            data.levelScores.Add(new LevelScore
            {
                levelNumber = levelData.levelNumber,
                bestScore = score
            });
        }
        else if (score > entry.bestScore)
        {
            entry.bestScore = score;
        }

        SaveSystem.Save(data);
    }

    void LoadNextLevel()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            SceneManager.LoadScene("Main");
    }

    bool IsConnectionValid(NodeTile tile)
    {
        if (!HasCorrectConnectionCount(tile))
            return false;

        foreach (Direction dir in tile.connections)
        {
            NodeTile n = tile.GetNeighbor(dir);
            if (n == null || !n.connections.Contains(GetOppositeDirection(dir)))
                return false;
        }
        return true;
    }

    bool HasCorrectConnectionCount(NodeTile tile)
    {
        return tile.tileType switch
        {
            TileType.End => tile.connections.Count == 1,
            TileType.Straight or TileType.Corner => tile.connections.Count == 2,
            TileType.Cross => tile.connections.Count == 4,
            _ => false
        };
    }

    bool IsSingleClosedLoop()
    {
        HashSet<NodeTile> visited = new();
        Stack<NodeTile> stack = new();
        stack.Push(tiles[0]);

        while (stack.Count > 0)
        {
            NodeTile t = stack.Pop();
            if (!visited.Add(t))
                continue;

            foreach (Direction d in t.connections)
            {
                NodeTile n = t.GetNeighbor(d);
                if (n != null && !visited.Contains(n))
                    stack.Push(n);
            }
        }

        return visited.Count == tiles.Count;
    }

    void ResetTileColors() { foreach (var t in tiles) t.SetInvalidColor(); }
    void SetAllTilesValid() { foreach (var t in tiles) t.SetValidColor(); }

    Direction GetOppositeDirection(Direction d) =>
        d switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => Direction.Up
        };

    void ShowLevelUnlockUI(int level)
    {
        levelUnlockPanel.SetActive(true);
        levelText.text = $"LEVEL {level}";
        lockImage.SetActive(true);
    }

    void RemoveLockIcon() => lockImage.SetActive(false);
    void HideUnlockUI() => levelUnlockPanel.SetActive(false);
}
