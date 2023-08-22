﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {


    public static float EPSILON = 0.01f;
    public static bool ApproximatelyEqual(float a, float b)
    {
        return (Mathf.Abs(a - b) < EPSILON);
    }

    public static float Clamp(float val, float min, float max)
    {
        if (val < min)
            val = min;
        if (val > max)
            val = max;
        return val;
    }

    public static float AngleDiffPosNeg(float a, float b)
    {
        float diff = a - b;
        if (diff > 180)
            return diff - 360;
        if (diff < -180)
            return diff + 360;
        return diff;
    }

    public static float Degrees360(float angleDegrees)
    {
        while (angleDegrees >= 360)
            angleDegrees -= 360;
        while (angleDegrees < 0)
            angleDegrees += 360;
        return angleDegrees;

    }

    public static float VectorToHeadingDegrees(Vector3 v)
    {
        return Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
    }

    public static void CPA(Entity381 e1, Entity381 e2)
    {


    }

    public static float BinaryToDecimal(int[] bin)
    {
        float f = 0f;

        for(int i = 0; i < bin.Length; i++)
        {
            f += Mathf.Pow(2, i) * bin[i];
        }

        return f;
    }

    public static float GrayToDecimal(int[] bin)
    {
        float f = 0f;
        bool flip = false;

        for (int i = 0; i < bin.Length; i++)
        {
            if(bin[i] == 1)
            {
                f += (Mathf.Pow(2, i+1) - 1) * (flip ? -1 : 1);
                flip = !flip;
            }
        }

        return f;
    }
}
