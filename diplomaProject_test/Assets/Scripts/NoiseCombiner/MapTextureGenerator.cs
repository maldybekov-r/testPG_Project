public class MapTextureGenerator
{
    public UnityEngine.Texture2D GeneratedTexture { get => _texture; }
    private UnityEngine.Texture2D _texture;

    private UnityEngine.Renderer _renderer;
    private int _mapSize;

    public MapTextureGenerator(ref UnityEngine.Renderer renderer, int size)
    {
        _renderer = renderer;
        _mapSize = size;
    }

    public UnityEngine.Color[] TryDrawBasicNoise(float[,] noiseMap)
    {
        UnityEngine.Color[] pixelNoiseValue = new UnityEngine.Color[_mapSize * _mapSize];

        if (noiseMap == null || _mapSize <= 0)
            return null;

        for (int y = 0; y < _mapSize; ++y)
            for (int x = 0; x < _mapSize; ++x)
                pixelNoiseValue[y * _mapSize + x] = UnityEngine.Color.Lerp(UnityEngine.Color.black, UnityEngine.Color.white, noiseMap[x, y]);

        return pixelNoiseValue;
    }


    public UnityEngine.Color[] GenerateColorsOnNoise(TerrainType[] regions, float[,] noiseMap)
    {

        UnityEngine.Color[] pixelColors = new UnityEngine.Color[_mapSize * _mapSize];

        if (noiseMap == null || _mapSize <= 0 || regions == null)
            return null;

        int regionsAmount = regions.Length;

        for (int y = 0; y < _mapSize; ++y)
            for (int x = 0; x < _mapSize; ++x)
            {
                float currentHeight = noiseMap[x, y];

                for (int regionId = 0; regionId < regionsAmount; ++regionId)
                    if (currentHeight <= regions[regionId].height)
                    {
                        pixelColors[y * _mapSize + x] = regions[regionId].color;
                        break;
                    }
            }

        return pixelColors;
    }

    public float[,] DecreaseAdditionalTextureFromMainTexture(float[,] mainNoise, float[,] additionalNoise)
    {
        float[,] res = new float[_mapSize, _mapSize];

        for (int r = 0; r < _mapSize; ++r)
            for (int c = 0; c < _mapSize; ++c)
                res[r, c] = UnityEngine.Mathf.Clamp01(mainNoise[r, c] - additionalNoise[r, c]);

        return res;
    }

    public float[,] IncreaseAdditionalTextureToMainTexture(float[,] mainNoise, float[,] additionalNoise)
    {
        float[,] res = new float[_mapSize, _mapSize];

        for (int r = 0; r < _mapSize; ++r)
            for (int c = 0; c < _mapSize; ++c)
                res[r, c] = UnityEngine.Mathf.Clamp01(mainNoise[r, c] + additionalNoise[r, c]);

        return res;
    }

    public void SaveChanges(UnityEngine.Color[] pixelColors)
    {
        if (pixelColors == null)
            return;

        _texture = new UnityEngine.Texture2D(_mapSize, _mapSize);
        _texture.SetPixels(pixelColors);
        _texture.Apply();

        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            _renderer.sharedMaterial.mainTexture = _texture;
        else
            _renderer.material.mainTexture = _texture;
    }
}
