public class FalloffNoise
{
    public float[,] FalloffMap { get => _falloffMap; }
    private float[,] _falloffMap;

    public FalloffNoise()
    {
    }

    public void GenerateFalloffNoise(int size, float sharpness, float extensibility)
    {
        _falloffMap = new float[size, size];

        for (int r = 0; r < size; ++r)
            for (int c = 0; c < size; ++c)
            {
                float x = c / (float)size * 2 - 1;
                float y = r / (float)size * 2 - 1;

                float value =
                    UnityEngine.Mathf.Max(UnityEngine.Mathf.Abs(x), UnityEngine.Mathf.Abs(y));

                _falloffMap[r, c] = Evaluate(value, sharpness, extensibility);
            }
    }

    public float Evaluate(float value, float a, float b)
        => UnityEngine.Mathf.Pow(value, a) / (UnityEngine.Mathf.Pow(value, a) + UnityEngine.Mathf.Pow(b - b * value, a));
}