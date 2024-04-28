using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class LevelRunner : MonoBehaviour
{
    [SerializeField] Match3Runner match3Runner;

    public ScriptableLevelData loadedLevel { get; private set; }

    public void GenerateAndRun(ScriptableLevelData level)
    {
        loadedLevel = level;

        match3Runner.GenerateAndRun(level);
    }

    void OnDestroy()
    {

    }
}
