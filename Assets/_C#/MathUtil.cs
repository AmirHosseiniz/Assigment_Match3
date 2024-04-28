using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtil
{
    public static float Map(float a, float b, float c, float d, float t)
    {
        return (((t - a) / (b - a)) * (d - c)) + c;
    }

    public static Vector2 Rotate(this Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    public static string GetAbbreviatedString(int number)
    {
        float abbreviatedNumber;
        string postFix = "";

        if (number >= 1000000)
        {
            number /= 100000;
            number *= 100000;
            abbreviatedNumber = (float)number / 1000000;
            postFix = "M";
        }
        else if (number >= 1000)
        {
            number /= 100;
            number *= 100;
            abbreviatedNumber = (float)number / 1000;
            postFix = "K";
        }
        else abbreviatedNumber = number;
        return abbreviatedNumber.ToString("0.#") + postFix;
    }

    public static float NextFloat(this System.Random random, float min, float max)
    {
        return MathUtil.Map(0f, 1f, min, max, (float)random.NextDouble());
    }

    public static float NextFloat(this System.Random random)
    {
        return (float)random.NextDouble();
    }

    public static int Digits(int n)
    {
        int digits = n < 0 ? 2 : 1;
        while ((n /= 10) != 0) ++digits;
        return digits;
    }
}
