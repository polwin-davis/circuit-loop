using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public LevelData levelData;
    public List<NodeTile> tiles = new();

    private bool levelCompleted;
    public bool IsLevelCompleted => levelCompleted;

    [Header("Effects")]
    public ParticleSystem completionParticles;
    public AudioSource completionAudio;

    [Header("UI")]
    public GameObject levelUnlockPanel;
    public TMPro.TextMeshProUGUI levelText;
    public GameObject lockImage;

    [Header("Audio")]
    public AudioSource tileRotateAudio;

    [Header("Delays")]
    public float celebrationDelay = 1f;
    public float unlockDelay = 1f;
    public float loadNextLevelDelay = 1f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (ScoreManager.Instance && levelData)
            ScoreManager.Instance.Init(levelData);
    }

    // Called whenever a tile is rotated
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

    // Handles completion logic
    void OnLevelCompleted()
    {
        ScoreManager.Instance?.StopScore();
        ScoreManager.Instance?.ShowScore();

        SaveBestScore();

        SaveData data = SaveSystem.Load();
        bool hasNext = HasNextLevel(out LevelData nextLevel);

        completionParticles?.Play();
        completionAudio?.Play();

        if (hasNext)
        {
            if (nextLevel.levelNumber > data.unlockedLevel)
            {
                data.unlockedLevel = nextLevel.levelNumber;
                SaveSystem.Save(data);
                ShowLevelUnlockUI(nextLevel.levelNumber);
            }
        }
        else
        {
            if (!data.finalLevelCompleted)
            {
                data.finalLevelCompleted = true;
                SaveSystem.Save(data);
                ShowComingSoonUI();
            }
        }

        Invoke(nameof(RemoveLockIcon), celebrationDelay);
        Invoke(nameof(HideUnlockUI), celebrationDelay + unlockDelay);
        Invoke(nameof(LoadNextLevel),
            celebrationDelay + unlockDelay + loadNextLevelDelay);
    }

    // Determines if a next level exists
    bool HasNextLevel(out LevelData nextLevel)
    {
        nextLevel = null;
        int nextLevelNumber = levelData.levelNumber + 1;

        foreach (LevelData level in Resources.LoadAll<LevelData>(""))
        {
            if (level.levelNumber == nextLevelNumber)
            {
                nextLevel = level;
                return true;
            }
        }
        return false;
    }

    void LoadNextLevel()
    {
        if (HasNextLevel(out LevelData nextLevel))
            SceneManager.LoadScene(nextLevel.sceneName);
        else
            SceneManager.LoadScene("Main");
    }

    // Saves the best score for the current level
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

    void ResetTileColors()
    {
        foreach (var t in tiles)
            t.SetInvalidColor();
    }

    void SetAllTilesValid()
    {
        foreach (var t in tiles)
            t.SetValidColor();
    }

    void ShowLevelUnlockUI(int level)
    {
        levelUnlockPanel.SetActive(true);
        levelText.text = $"LEVEL {level}";
        lockImage.SetActive(true);
    }

    void ShowComingSoonUI()
    {
        levelUnlockPanel.SetActive(true);
        levelText.text = "COMING SOON";
        lockImage.SetActive(false);
    }

    void RemoveLockIcon() => lockImage.SetActive(false);
    void HideUnlockUI() => levelUnlockPanel.SetActive(false);

    Direction GetOppositeDirection(Direction d)
    {
        return d switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => Direction.Up
        };
    }
}
