using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GamePlay/ScriptableLevelData")]
public class ScriptableLevelData : ScriptableObject
{
    [SerializeField] LevelData levelData;

    public LevelData LevelData => levelData;
}
