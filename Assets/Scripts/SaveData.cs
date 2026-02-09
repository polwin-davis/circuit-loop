using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int unlockedLevel = 1;
    public List<LevelScore> levelScores = new();
    public bool finalLevelCompleted = false;
    public int appVersion = 1;
}

[System.Serializable]
public class LevelScore
{
    public int levelNumber;
    public int bestScore;
}
