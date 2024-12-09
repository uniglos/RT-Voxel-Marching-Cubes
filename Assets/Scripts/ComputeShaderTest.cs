using UnityEngine;



public class ComputeShaderWithNoise : MonoBehaviour
{
    public ComputeShader computeShader;
    public Texture3D noiseTexture;  // The 3D noise texture

    private ComputeBuffer positionsBuffer;
    private int kernelIndex;

    void Start()
    {
        noiseTexture = Generate3DTexture(256,20f);

        // Check if the computeShader is assigned
        if (computeShader == null)
        {
            Debug.LogError("ComputeShader is not assigned in the Inspector.");
            return;
        }

        // Set the kernel index for the compute shader
        kernelIndex = computeShader.FindKernel("CSMain");

        // Check if kernelIndex is valid
        if (kernelIndex == -1)
        {
            Debug.LogError("Kernel 'CSMain' not found in the compute shader.");
            return;
        }

        // Prepare the buffer to store positions (assuming float3 for each position)
        int dataSize = 256; // Example size (e.g., 256 elements)
        positionsBuffer = new ComputeBuffer(dataSize, sizeof(float) * 3); // Allocating space for float3

        // Set the texture and buffer in the compute shader
        computeShader.SetTexture(kernelIndex, "NoiseTexture", noiseTexture);
        computeShader.SetBuffer(kernelIndex, "_Positions", positionsBuffer);

        // Dispatch the shader
        int threadGroups = Mathf.CeilToInt(dataSize / 8f);  // Adjust to fit your data
        computeShader.Dispatch(kernelIndex, threadGroups, 1, 1);

        // Retrieve results from the buffer
        float[] positions = new float[dataSize * 3]; // Assuming each position is a float3
        positionsBuffer.GetData(positions);

        // Log or use the positions
        for (int i = 0; i < dataSize; i++)
        {
            float x = positions[i * 3];
            float y = positions[i * 3 + 1];
            float z = positions[i * 3 + 2];
            Debug.Log($"Position {i}: ({x}, {y}, {z})");
        }
    }

    void OnDestroy()
    {
        // Release the buffer to avoid memory leaks
        if (positionsBuffer != null)
        {
            positionsBuffer.Release();
        }
    }




public static Texture3D Generate3DTexture(int size, float scale)
    {
        Texture3D texture = new Texture3D(size, size, size, TextureFormat.RFloat, false);
        float[] noiseData = new float[size * size * size];
        int index = 0;

        for (int z = 0; z < size; z++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nx = x / (float)size * scale;
                    float ny = y / (float)size * scale;
                    float nz = z / (float)size * scale;

                    // Calculate Perlin noise value
                    float noiseValue = MyMath.PerlinNoise3D(nx, ny, nz);
                    noiseData[index++] = noiseValue;
                }
            }
        }

        // Convert float array to Color array for Texture3D
        Color[] colorData = new Color[noiseData.Length];
        for (int i = 0; i < noiseData.Length; i++)
        {
            float value = noiseData[i];
            colorData[i] = new Color(value, value, value, 1.0f); // Grayscale
        }

        texture.SetPixels(colorData);
        texture.Apply();
        return texture;
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
