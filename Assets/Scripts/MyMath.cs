using UnityEngine;

public static class MyMath
{

    public static float ClampedRemap(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        // Clamp the input value to the input range first
        float clampedValue = Mathf.Clamp(value, inputMin, inputMax);

        // Remap the clamped value to the output range
        float remappedValue = outputMin + (clampedValue - inputMin) * (outputMax - outputMin) / (inputMax - inputMin);

        // Return the remapped value
        return remappedValue;
    }


    public static float PerlinNoise3D(float x, float y, float z)
    {
        y += 1;
        z += 2;
        float xy = _perlin3DFixed(x, y);
        float xz = _perlin3DFixed(x, z);
        float yz = _perlin3DFixed(y, z);
        float yx = _perlin3DFixed(y, x);
        float zx = _perlin3DFixed(z, x);
        float zy = _perlin3DFixed(z, y);

        return xy * xz * yz * yx * zx * zy;
    }

    static float _perlin3DFixed(float a, float b)
    {
        return Mathf.Sin(Mathf.PI * Mathf.PerlinNoise(a, b));
    }
}
