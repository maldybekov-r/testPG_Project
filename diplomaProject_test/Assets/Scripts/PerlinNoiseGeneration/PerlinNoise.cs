public class PerlinNoise
{
    public float[,] PerlinNoiseMap { get => _noiseMap; }
    private float[,] _noiseMap = null;
    
    private int _width = 0;
    private int _height = 0;

    public PerlinNoise()
    {
    }

    public void SetMapSize(int width, int height)
    {
        _width = width;
        _height = height;

        _noiseMap = new float[width, height];
    }

    public void GenerateNoise(float scale, int octaves, float persistance, float lacunarity, UnityEngine.Vector2 offset)
    {
        if (scale <= 0)
            scale = 0.0001f;

        float _halfWidth = _width * 0.5f;
        float _halfHeight = _height * 0.5f;

        float minNoiseHeight = float.MaxValue;
        float maxNoiseHeight = float.MinValue;

        for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
            {
                float heightAmplitude = 1;
                float widthFrequency = 1;
                float noiseHeight = 0;

                for (int octaveId = 0; octaveId < octaves; ++octaveId)
                {

                    float xVal = (x - _halfWidth) / scale * widthFrequency + offset.x;
                    float yVal = (y - _halfHeight) / scale * widthFrequency + offset.y;
                    float pixelPerlinValue = UnityEngine.Mathf.PerlinNoise(xVal, yVal) * 2.0f - 1.0f;
                    noiseHeight += pixelPerlinValue * heightAmplitude;

                    heightAmplitude *= persistance;
                    widthFrequency *= lacunarity;
                }

                if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;

                _noiseMap[x, y] = noiseHeight;
            }

        for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
                _noiseMap[x, y] = UnityEngine.Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, _noiseMap[x, y]);
    }
}