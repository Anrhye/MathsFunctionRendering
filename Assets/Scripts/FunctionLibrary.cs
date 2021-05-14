using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public static class FunctionLibrary
{
    public struct FunctionData
    { 
        public Function function { get; private set; }
        public FunctionName functionName { get; private set; }

        public FunctionData(Function fct, FunctionName fctName)
        {
            this.function = fct;
            this.functionName = fctName;
        }
    }

    public delegate Vector3 Function(float u, float v, float t);

    private static List<Function> functions = new List<Function> {
        Wave2D, Wave3D,
        MultiWave2D, MultiWave3D,
        WaterRipple2D, WaterRipple3D,
        Sphere, ScalingSphere, RotatingSphere,
        Torus, RotatingTorus
    };

    public enum FunctionName { 
        Wave2D, Wave3D,
        MultiWave2D, MultiWave3D,
        WaterRipple2D, WaterRipple3D,
        Sphere, ScalingSphere, RotatingSphere,
        Torus, RotatingTorus
    };

    public static FunctionData GetFunction(FunctionName name)
    {
        if ((int)name > functions.Count - 1)
            name = FunctionName.Wave2D;

        return new FunctionData(functions[(int)name], name);
    }

    public static FunctionData GetNextFunction(FunctionName name)
        =>  GetFunction(++name);

    public static Vector3 MorphGraphs(float u, float v, float t,
        Function start, Function end, float progress)
    {
        return Vector3.LerpUnclamped(start(u, v, t), end(u, v, t), SmoothStep(0f, 1f, progress));
    }

    public static Vector3 Wave2D(float u, float v, float t)
        => new Vector3(u, Sin(PI * (u + t)), 0);

    public static Vector3 Wave3D(float u, float v, float t)
    {
        Vector3 p = new Vector3();

        p.x = u;
        p.y = Sin(PI * (u + v + t));
        p.z = v;

        return p;
    }

    public static Vector3 MultiWave2D(float u, float v, float t)
    {
        float y = Sin(PI * (u + .5f * t));
        y += 0.5f * Sin(2f * PI * (u + t));

        return new Vector3(u, y * (2f / 3f), 0);
    }
    public static Vector3 MultiWave3D(float u, float v, float t)
    {
        Vector3 p = new Vector3();

        p.x = u;

        p.y = Sin(PI * (u + .5f * t));
        p.y += 0.5f * Sin(2f * PI * (v + t));
        p.y += Sin(PI * (u + v + .25f * t));
        p.y *= 1f / 2.5f;

        p.z = v;

        return p;
    }

    public static Vector3 WaterRipple2D(float u, float v, float t)
    {
        float d = Abs(u);
        float y = Sin(PI * (4f * d - t));

        return new Vector3(u, y / (1f + 10f * d), 0);
    }
    public static Vector3 WaterRipple3D(float u, float v, float t)
    {
        Vector3 p = new Vector3();

        float d = Sqrt(u * u + v * v);

        p.x = u;

        p.y = Sin(PI * (4f * d - t));
        p.y /= 1f + 10f * d;

        p.z = v;

        return p;
    }


    public static Vector3 Sphere(float u, float v, float t)
    {
        Vector3 p = new Vector3();

        float radius = Cos(.5f * PI * v);

        p.x = Sin(PI * u) * radius;
        p.y = Sin(PI * 0.5f * v);
        p.z = Cos(PI * u) * radius;

        return p;
    }

    public static Vector3 ScalingSphere(float u, float v, float t)
    {
        Vector3 p = new Vector3();

        float radius = 0.5f + 0.5f * Sin(PI * t);
        float s = radius * Cos(0.5f * PI * v);

        p.x = s * Sin(PI * u);
        p.y = radius * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);

        return p;
    }

    public static Vector3 RotatingSphere(float u, float v, float t)
    {
        Vector3 p = new Vector3();

        float radius = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));
        float s = radius * Cos(0.5f * PI * v);

        p.x = s * Sin(PI * u);
        p.y = radius * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);

        return p;
    }

    public static Vector3 Torus(float u, float v, float t)
    {
        Vector3 p = new Vector3();

        float r1 = 1f;
        float r2 = 0.5f;
        float s = r1 + r2 * Cos(PI * v);

        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);

        return p;
    }

    public static Vector3 RotatingTorus(float u, float v, float t)
    {
        Vector3 p = new Vector3(); 

        float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));
        float s = r1 + r2 * Cos(PI * v);

        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);

        return p;
    }
}
