using System;
using System.Collections;
using System.Collections.Generic;
using GameplayEnums.Cells;
using UnityEditor;
using UnityEngine;

public static class ScriptableLevelDataEditorUtilities
{
    #region GUIContent getters

    static GridItemGraphicProvider _itemGraphicProvider;

    public static GUIContent GetGuiContentForType(TypeEnum type)
    {
        var texture = GetTextureForType(type);
        if (texture) return new GUIContent(texture);
        return new GUIContent(Enum.GetName(typeof(TypeEnum), type));
    }
    #endregion

    #region Texture getters
    public static Texture2D GetTextureForType(TypeEnum type)
    {
        string typeName = Enum.GetName(typeof(TypeEnum), type);
        var texture = (Texture2D)EditorGUIUtility.Load($"ScriptableLevelDataEditor/{typeName}.png");
        return texture;
    }
    #endregion

    public static Color GetColor(int colorIndex, int colorCount)
    {
        var color = Color.black;
        if (_itemGraphicProvider == null)
            _itemGraphicProvider = AssetFinder.FindAssetsByType<GridItemGraphicProvider>()[0];
        if (colorIndex >= 0 && colorIndex < colorCount)
        {
            DotGraphic graphic = null;
            if (colorIndex >= 0 && colorIndex < _itemGraphicProvider.DotGraphicsPrefabs.Length)
                graphic = _itemGraphicProvider.DotGraphicsPrefabs[colorIndex];
            if (graphic != null)
                return graphic.CorolatedColor;
            else
            {
                float h = Mathf.Lerp(0f, 1f, colorIndex * (1f / colorCount));
                color = Color.HSVToRGB(h, 1f, 1f);
            }
        }
        return color;
    }

    public static Texture2D ColorTexture(Texture2D texture, Color color)
    {
        var newTexture = new Texture2D(texture.width, texture.height);
        var pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; ++i)
        {
            if (pixels[i].a > 0f)
            {
                pixels[i] = color;
            }
        }
        newTexture.SetPixels(pixels);
        newTexture.Apply();
        return newTexture;
    }
}
