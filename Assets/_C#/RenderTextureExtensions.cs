using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RenderTextureExtensions
{
    public static void Resize(this RenderTexture renderTexture, int width, int height)
    {
        renderTexture.Release();
        renderTexture.width = width;
        renderTexture.height = height;
    }
}
