using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameplayEnums.Cells;
using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;
using Utilities = ScriptableLevelDataEditorUtilities;

[CustomEditor(typeof(ScriptableLevelData))]
public class ScriptableLevelDataEditor : NaughtyInspector
{
    ScriptableLevelData scriptableLevelData;

    //brush mode
    BrushModeEnum brushMode = BrushModeEnum.cell;

    //brush type
    TypeEnum[] types;
    Dictionary<TypeEnum, GUIContent> typesGuiContents;
    int brushTypeIndex;
    TypeEnum BrushType => types[brushTypeIndex];

    //brush color
    int brushColor = -1;
    Texture2D baseColorTexture;
    Dictionary<int, Dictionary<int, Texture2D>> colorTextures = new();
    GUIContent baseColorGuiContent;
    Dictionary<int, Dictionary<int, GUIContent>> colorGuiContents = new();


    protected override void OnEnable()
    {
        base.OnEnable();

        //initialize brush
        types = (TypeEnum[])Enum.GetValues(typeof(TypeEnum));
        typesGuiContents = new(types.Length);

        foreach (var type in types)
        {
            typesGuiContents.Add(type, Utilities.GetGuiContentForType(type));
        }

        baseColorTexture = Utilities.ColorTexture(new Texture2D(100, 100), Color.black);
        baseColorGuiContent = new GUIContent(baseColorTexture);

        scriptableLevelData = target as ScriptableLevelData;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);

        DrawLevelDataInspector();
    }

    void DrawLevelDataInspector()
    {
        DrawColorCountInspector();
        GUILayout.Space(20);
        DrawDimensionInspector();

        GUILayout.Space(40);
        DrawBrushSelectors();
        GUILayout.Space(20);
        DrawGridInspector();
    }

    void DrawDimensionInspector()
    {
        var grid = scriptableLevelData.LevelData.GridData;

        int width = grid.Width;
        int height = grid.Height;

        //draw width and height fields
        int newWidth = EditorGUILayout.DelayedIntField("Width", width);
        if (newWidth != width && newWidth > 0)
        {
            Undo.RecordObject(scriptableLevelData, "Set grid width");
            grid.Width = newWidth;
            EditorUtility.SetDirty(scriptableLevelData);
        }

        int newHeight = EditorGUILayout.DelayedIntField("Height", height);
        if (newHeight != height && newHeight > 0)
        {
            Undo.RecordObject(scriptableLevelData, "Set grid height");
            grid.Height = newHeight;
            EditorUtility.SetDirty(scriptableLevelData);
        }
    }

    void DrawColorCountInspector()
    {
        int colorCount = scriptableLevelData.LevelData.ColorCount;
        int newColorCount = EditorGUILayout.DelayedIntField("Color count", scriptableLevelData.LevelData.ColorCount);
        if (newColorCount != colorCount && newColorCount > 1)
        {
            Undo.RecordObject(scriptableLevelData, "Set color count");
            scriptableLevelData.LevelData.ColorCount = newColorCount;
            EditorUtility.SetDirty(scriptableLevelData);
        }
    }

    void DrawBrushSelectors()
    {
        brushTypeIndex = GUILayout.SelectionGrid(brushTypeIndex, typesGuiContents.Values.ToArray(), types.Length);

        if (TypeEnumUtilities.UsesColor(BrushType))
        {
            int colorCount = scriptableLevelData.LevelData.ColorCount;

            GUIContent[] guiContents = new GUIContent[colorCount + 1];
            for (int i = -1; i < colorCount; ++i)
            {
                guiContents[i + 1] = GetGuiContentForColor(i, colorCount);
            }

            brushColor = GUILayout.SelectionGrid(brushColor + 1, guiContents, colorCount + 1) - 1;
        }

        GUILayout.Space(20);

        brushMode = (BrushModeEnum)EditorGUILayout.EnumPopup("Brush mode", brushMode);
    }

    void DrawGridInspector()
    {
        var grid = scriptableLevelData.LevelData.GridData;
        int width = grid.Width;
        int height = grid.Height;
        int colorCount = scriptableLevelData.LevelData.ColorCount;


        float maxButtonWidth = EditorGUIUtility.currentViewWidth / width;

        GUILayoutOption[] cellButtonLayoutOptions = new[]
        {
            GUILayout.MinWidth(0),
            GUILayout.MaxWidth(maxButtonWidth),
            GUILayout.MinHeight(0),
            GUILayout.MaxHeight(maxButtonWidth)
        };

        GUILayout.BeginVertical();
        for (int j = 0; j < height; ++j)
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < width; ++i)
            {
                var cell = grid[i, j];
                var guiContent = GetGuiContentForCell(cell, colorCount);
                if (GUILayout.Button(guiContent, cellButtonLayoutOptions))
                {
                    Undo.RecordObject(scriptableLevelData, "Set cell data");

                    List<CellData> targetCells = new();

                    switch (brushMode)
                    {
                        case BrushModeEnum.cell:
                            targetCells.Add(cell);
                            break;
                        case BrushModeEnum.row:
                            for (int x = 0; x < width; ++x)
                            {
                                targetCells.Add(grid[x, j]);
                            }
                            break;
                        case BrushModeEnum.column:
                            for (int y = 0; y < height; ++y)
                            {
                                targetCells.Add(grid[i, y]);
                            }
                            break;
                        case BrushModeEnum.all:
                            for (int x = 0; x < width; ++x)
                            {
                                for (int y = 0; y < height; ++y)
                                {
                                    targetCells.Add(grid[x, y]);
                                }
                            }
                            break;
                    }

                    targetCells.ForEach(targetCell => targetCell.type = BrushType);

                    if (TypeEnumUtilities.UsesColor(BrushType))
                    {
                        targetCells.ForEach(targetCell => targetCell.color = brushColor);
                    }
                    EditorUtility.SetDirty(scriptableLevelData);
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    Texture2D GetTextureForColor(int colorIndex, int colorCount)
    {
        if (!colorTextures.ContainsKey(colorCount))
        {
            Dictionary<int, Texture2D> subDictionary = new Dictionary<int, Texture2D>();

            for (int i = 0; i < colorCount; ++i)
            {
                var color = Utilities.GetColor(i, colorCount);
                var texture = Utilities.ColorTexture(baseColorTexture, color);
                subDictionary.Add(i, texture);
            }

            colorTextures.Add(colorCount, subDictionary);
        }

        if (colorIndex < 0) return baseColorTexture;
        return colorTextures[colorCount][colorIndex];
    }

    GUIContent GetGuiContentForColor(int colorIndex, int colorCount)
    {
        if (!colorGuiContents.ContainsKey(colorCount))
        {
            Dictionary<int, GUIContent> subDictionary = new Dictionary<int, GUIContent>();

            for (int i = 0; i < colorCount; ++i)
            {
                subDictionary.Add(i, new GUIContent(GetTextureForColor(i, colorCount)));
            }

            colorGuiContents.Add(colorCount, subDictionary);
        }

        if (colorIndex < 0) return baseColorGuiContent;
        return colorGuiContents[colorCount][colorIndex];
    }

    GUIContent GetGuiContentForCell(CellData cell, int colorCount)
    {
        var cellType = cell.type;

        Texture2D texture = null;

        if (!texture) texture = Utilities.GetTextureForType(cellType);

        if (TypeEnumUtilities.UsesColor(cellType))
        {
            texture = Utilities.ColorTexture(texture, Utilities.GetColor(cell.color, colorCount));
        }

        return new GUIContent(texture);
    }

    public enum BrushModeEnum
    {
        cell,
        row,
        column,
        all
    }
}
