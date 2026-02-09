using UnityEngine;

[CreateAssetMenu(menuName = "LoopGame/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public string sceneName;

    [Header("Scoring")]
    public int maxScore = 100;
    public float scoreDecayPerSecond = 1f;
}
