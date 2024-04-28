using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelLoader : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] ScriptableLevelData editorLevel;
#endif

    [SerializeField] ScriptableLevelData[] levels;
    [SerializeField] UnityEvent<ScriptableLevelData> onLevelLoaded;

    public static int LevelIndex
    {
        get => PlayerPrefs.GetInt("levelIndex", 0);
        private set => PlayerPrefs.SetInt("levelIndex", value);
    }

    public ScriptableLevelData LoadedLevel { get; private set; }

    private void Awake()
    {
        ScriptableLevelData levelData = null;
        levelData = LoadLevel(levelData);

        LoadedLevel = levelData;

        onLevelLoaded.Invoke(levelData);
    }

    private ScriptableLevelData LoadLevel(ScriptableLevelData levelData)
    {
#if UNITY_EDITOR
        if (editorLevel) levelData = editorLevel;
#endif
        if (!levelData)
        {
            int levelIndex = LevelIndex;
            levelIndex %= levels.Length;
            levelData = levels[levelIndex];
        }
        return levelData;
    }

    private void OnDestroy()
    {

    }
}
