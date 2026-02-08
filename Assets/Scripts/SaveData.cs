using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int unlockedLevel = 1;
    public List<LevelScore> levelScores = new();
}

[System.Serializable]
public class LevelScore
{
    public int levelNumber;
    public int bestScore;
}
