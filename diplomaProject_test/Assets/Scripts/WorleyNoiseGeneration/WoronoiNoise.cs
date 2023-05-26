using System.Drawing;

public class WoronoiNoise
{

    public float[,] WorleyMap { get => _worleyMap; }
    private float[,] _worleyMap;

    public WoronoiNoise()
    {
    }

    public UnityEngine.Color[] GenerateWorleyColorMap(int size, TerrainType[] regions)
    {

        int regionsLen = regions.Length;

        UnityEngine.Vector2[] pointsPos = new UnityEngine.Vector2[regionsLen];
        UnityEngine.Color[] pixelColors = new UnityEngine.Color[size * size];
        UnityEngine.Color[] regionsColor = new UnityEngine.Color[regionsLen];

        for (int i = 0; i < regionsLen; ++i)
        {
            int posX = UnityEngine.Random.Range(0, size);
            int posY = UnityEngine.Random.Range(0, size);

            pointsPos[i] = new UnityEngine.Vector2(posX, posY);
        }

        for (int i = 0; i < regionsLen; i++)
            regionsColor[i] = regions[i].color;

        for (int r = 0; r < size; ++r)
            for (int c = 0; c < size; ++c)
            {
                int regionTypeId = 0;
                float distance = float.MaxValue;

                for (int i = 0; i < regionsLen; ++i)
                {

                    float closestDistanceToPoint = UnityEngine.Vector2.Distance(new UnityEngine.Vector2(c, r), pointsPos[i]);

                    if (closestDistanceToPoint < distance)
                    {
                        distance = closestDistanceToPoint;
                        regionTypeId = i;
                    }
                }

                pixelColors[r * size + c] = regionsColor[regionTypeId];
            }

        return pixelColors;
    }

    public void GenerateWorleyNoise(int size, int pointsAmount)
    {
        _worleyMap = new float[size, size];

        UnityEngine.Vector2[] pointsPos = new UnityEngine.Vector2[pointsAmount];
        float[] regionColors = new float[pointsAmount];

        for (int i = 0; i < pointsAmount; ++i)
        {
            int posX = UnityEngine.Random.Range(0, size);
            int posY = UnityEngine.Random.Range(0, size);

            pointsPos[i] = new UnityEngine.Vector2(posX, posY);
        }

        for (int i = 0; i < pointsAmount; i++)
        {
            float xVal = UnityEngine.Random.Range(0, 1.0f);
            regionColors[i] = xVal;
        }

        for (int r = 0; r < size; ++r)
            for (int c = 0; c < size; ++c)
            {
                int regionTypeId = 0;
                float distance = float.MaxValue;

                for (int i = 0; i < pointsAmount; ++i)
                {

                    float closestDistanceToPoint = UnityEngine.Vector2.Distance(new UnityEngine.Vector2(c, r), pointsPos[i]);

                    if (closestDistanceToPoint < distance)
                    {
                        distance = closestDistanceToPoint;
                        regionTypeId = i;
                    }
                }

                _worleyMap[r, c] = regionColors[regionTypeId];
            }
    }

    private float calculateDistance(int x1, int y1, int x2, int y2)
        => UnityEngine.Mathf.Sqrt(UnityEngine.Mathf.Pow(x2 - x1, 2) + UnityEngine.Mathf.Pow(y2 - y1, 2));
}
