using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerationManager : MonoBehaviour
{
    public enum DrawMode
    {
        perlinNoiseTexture = 0,
        perlinColorTexture = 1,
        worleyColorTexture = 2,
        perlinMeshMap = 3,
        falloffMeshMap = 4,
        worleyMeshMap = 5,
        combinePF = 6,
        combinePW = 7,
        combineWPF = 8,
    }

    [Header("Generation Parameters:")]
    [SerializeField] private bool _isAutoGenerate;
    [SerializeField] private bool _isAutoSpawn;
    [SerializeField] private DrawMode _drawMode;

    [Header("Generation Components:")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;

    [SerializeField] private Transform _waterPlaneTr;

    [Header("")]
    [Header("Texture Generation Parameters:")]
    [Range(10, 100)]
    [SerializeField] private float _noiseScale = 10;
    [SerializeField] private Vector2 _offset;

    [Header("")]
    [Header("Mesh Generation Parameters:")]
    [Range(10, 50)]
    [SerializeField] private float _meshHeightMultiplayer = 10;
    [Range(0, 5)]
    [SerializeField] private int _levelOfDetails = 5;

    [Header("Map Generation Components:")]
    [Header("Tarrain Region Parameters:")]
    [SerializeField] private TerrainType[] _terrainRegions;

    [Header("Terrain Region Componenets(Recomended 1 LOD):")]
    [SerializeField] private TerrainObjectRegion[] _terrainObjectRegions;

    [Header("")]
    [Header("Improve Parameters:")]
    [Header("Perlin Noise Paramters:")]
    [Range(1, 20)]
    [SerializeField] private int _octaves = 1;
    [Range(0, 1)]
    [SerializeField] private float _persistance = 0;
    [Range(0, 5)]
    [SerializeField] private float _lacunarity = 0;

    [Header("")]
    [Header("Falloff Noise Parameters:")]
    [Range(1, 10)]
    [SerializeField] private float _sharpness = 1;
    [Range(1, 10)]
    [SerializeField] private float _extensibility = 1;

    [Header("")]
    [Header("Worley Noise Parameters:")]
    [Range(2, 50)]
    [SerializeField] private int _pointsAmount = 2;

    private PerlinNoise _perlinNoise = new PerlinNoise();
    private FalloffNoise _falloffNoise = new FalloffNoise();
    private WoronoiNoise _worleyNoise = new WoronoiNoise();

    private MapMeshGenerator _meshGenerator = null;
    private MapTextureGenerator _textureGenerator = null;

    private const int _MAP_SIZE = 241;

    private Queue<GameObject> _parentBox = new Queue<GameObject>();

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        if (_meshGenerator == null)
            _meshGenerator = new MapMeshGenerator(ref _meshFilter, ref _meshRenderer);

        if (_textureGenerator == null)
            _textureGenerator = new MapTextureGenerator(ref _renderer, _MAP_SIZE);

        switch (_drawMode)
        {
            case DrawMode.perlinNoiseTexture: OnPerlinNoiseDrawn(); break;
            case DrawMode.perlinColorTexture: OnColorMapDrawn(); break;
            case DrawMode.worleyColorTexture: OnWorleyColorMapDrawn(); break;
            case DrawMode.perlinMeshMap: OnPerlinMeshDrawn(); break;
            case DrawMode.falloffMeshMap: OnFalloffNoiseDrawn(); break;
            case DrawMode.combinePF: OnPerlinAndFalloffMapGenerated(); break;
            case DrawMode.worleyMeshMap: OnWorleyNoiseDrawn(); break;
            case DrawMode.combinePW: OnPerlinAndWolreyMapGenerated(); break;
            case DrawMode.combineWPF: OnPerlinAndWolreyAndFalloffMapGenerated(); break;
        }
    }

    public bool IsAutoGenerate()
        => _isAutoGenerate;

    private void OnPerlinNoiseDrawn()
    {
        _perlinNoise.SetMapSize(width: _MAP_SIZE, height: _MAP_SIZE);
        _perlinNoise.GenerateNoise(_noiseScale, _octaves, _persistance, _lacunarity, _offset);

        _textureGenerator.SaveChanges(_textureGenerator.TryDrawBasicNoise(_perlinNoise.PerlinNoiseMap));
    }

    private void OnColorMapDrawn()
    {
        _perlinNoise.SetMapSize(width: _MAP_SIZE, height: _MAP_SIZE);
        _perlinNoise.GenerateNoise(_noiseScale, _octaves, _persistance, _lacunarity, _offset);

        _textureGenerator.SaveChanges(_textureGenerator.TryDrawBasicNoise(_perlinNoise.PerlinNoiseMap));
        _textureGenerator.SaveChanges(_textureGenerator.GenerateColorsOnNoise(_terrainRegions, _perlinNoise.PerlinNoiseMap));
    }

    private void OnWorleyColorMapDrawn()
    {
        _textureGenerator.SaveChanges(_worleyNoise.GenerateWorleyColorMap(_MAP_SIZE, _terrainRegions));
    }

    private void OnPerlinMeshDrawn()
    {
        _perlinNoise.SetMapSize(width: _MAP_SIZE, height: _MAP_SIZE);
        _perlinNoise.GenerateNoise(_noiseScale, _octaves, _persistance, _lacunarity, _offset);

        _textureGenerator.SaveChanges(_textureGenerator.TryDrawBasicNoise(_perlinNoise.PerlinNoiseMap));
        _textureGenerator.SaveChanges(_textureGenerator.GenerateColorsOnNoise(_terrainRegions, _perlinNoise.PerlinNoiseMap));

        _meshGenerator.GenerateMesh(_perlinNoise.PerlinNoiseMap, _meshHeightMultiplayer, _levelOfDetails);
        _meshGenerator.UpdateMesh(_textureGenerator.GeneratedTexture);
        _meshGenerator.AddCollissions();

        DrawWater();
        SpawnObjects(_perlinNoise.PerlinNoiseMap, _meshFilter.transform.localScale.x);
    }

    private void OnFalloffNoiseDrawn()
    {
        _falloffNoise.GenerateFalloffNoise(_MAP_SIZE, _sharpness, _extensibility);
        _textureGenerator.SaveChanges(_textureGenerator.TryDrawBasicNoise(_falloffNoise.FalloffMap));
        _textureGenerator.SaveChanges(_textureGenerator.GenerateColorsOnNoise(_terrainRegions, _falloffNoise.FalloffMap));

        _meshGenerator.GenerateMesh(_falloffNoise.FalloffMap, _meshHeightMultiplayer, _levelOfDetails);
        _meshGenerator.UpdateMesh(_textureGenerator.GeneratedTexture);
        _meshGenerator.AddCollissions();

        DrawWater();
        SpawnObjects(_falloffNoise.FalloffMap, _meshFilter.transform.localScale.x);
    }

    private void OnPerlinAndFalloffMapGenerated()
    {
        _perlinNoise.SetMapSize(width: _MAP_SIZE, height: _MAP_SIZE);
        _perlinNoise.GenerateNoise(_noiseScale, _octaves, _persistance, _lacunarity, _offset);

        _falloffNoise.GenerateFalloffNoise(_MAP_SIZE, _sharpness, _extensibility);

        float[,] combinedNoise = _textureGenerator.DecreaseAdditionalTextureFromMainTexture(
             mainNoise: _perlinNoise.PerlinNoiseMap,
             additionalNoise: _falloffNoise.FalloffMap
         );

        _textureGenerator.SaveChanges(_textureGenerator.TryDrawBasicNoise(combinedNoise));
        _textureGenerator.SaveChanges(_textureGenerator.GenerateColorsOnNoise(_terrainRegions, combinedNoise));

        _meshGenerator.GenerateMesh(combinedNoise, _meshHeightMultiplayer, _levelOfDetails);
        _meshGenerator.UpdateMesh(_textureGenerator.GeneratedTexture);
        _meshGenerator.AddCollissions();

        DrawWater();
        SpawnObjects(combinedNoise, _meshFilter.transform.localScale.x);
    }

    private void OnWorleyNoiseDrawn()
    {
        _worleyNoise.GenerateWorleyNoise(_MAP_SIZE, _pointsAmount);
        _textureGenerator.SaveChanges(_textureGenerator.TryDrawBasicNoise(_worleyNoise.WorleyMap));
        _textureGenerator.SaveChanges(_textureGenerator.GenerateColorsOnNoise(_terrainRegions, _worleyNoise.WorleyMap));

        _meshGenerator.GenerateMesh(_worleyNoise.WorleyMap, _meshHeightMultiplayer, _levelOfDetails);
        _meshGenerator.UpdateMesh(_textureGenerator.GeneratedTexture);
        _meshGenerator.AddCollissions();

        DrawWater();
        SpawnObjects(_worleyNoise.WorleyMap, _meshFilter.transform.localScale.x);
    }

    private void OnPerlinAndWolreyMapGenerated()
    {
        _perlinNoise.SetMapSize(width: _MAP_SIZE, height: _MAP_SIZE);
        _perlinNoise.GenerateNoise(_noiseScale, _octaves, _persistance, _lacunarity, _offset);

        _falloffNoise.GenerateFalloffNoise(_MAP_SIZE, _sharpness, _extensibility);

        _worleyNoise.GenerateWorleyNoise(_MAP_SIZE, _pointsAmount);

        float[,] combinedNoise = _textureGenerator.IncreaseAdditionalTextureToMainTexture(
             mainNoise: _perlinNoise.PerlinNoiseMap,
             additionalNoise: _worleyNoise.WorleyMap
         );

        _textureGenerator.SaveChanges(_textureGenerator.TryDrawBasicNoise(combinedNoise));
        _textureGenerator.SaveChanges(_textureGenerator.GenerateColorsOnNoise(_terrainRegions, combinedNoise));

        _meshGenerator.GenerateMesh(combinedNoise, _meshHeightMultiplayer, _levelOfDetails);
        _meshGenerator.UpdateMesh(_textureGenerator.GeneratedTexture);
        _meshGenerator.AddCollissions();

        DrawWater();
        SpawnObjects(combinedNoise, _meshFilter.transform.localScale.x);
    }

    private void OnPerlinAndWolreyAndFalloffMapGenerated()
    {
        _perlinNoise.SetMapSize(width: _MAP_SIZE, height: _MAP_SIZE);
        _perlinNoise.GenerateNoise(_noiseScale, _octaves, _persistance, _lacunarity, _offset);

        _falloffNoise.GenerateFalloffNoise(_MAP_SIZE, _sharpness, _extensibility);

        _worleyNoise.GenerateWorleyNoise(_MAP_SIZE, _pointsAmount);

        float[,] combinedNoise = _textureGenerator.IncreaseAdditionalTextureToMainTexture(
             mainNoise: _perlinNoise.PerlinNoiseMap,
             additionalNoise: _falloffNoise.FalloffMap
         );

        combinedNoise = _textureGenerator.DecreaseAdditionalTextureFromMainTexture(
            mainNoise: combinedNoise,
            additionalNoise: _worleyNoise.WorleyMap
            );

        _textureGenerator.SaveChanges(_textureGenerator.TryDrawBasicNoise(combinedNoise));
        _textureGenerator.SaveChanges(_textureGenerator.GenerateColorsOnNoise(_terrainRegions, combinedNoise));

        _meshGenerator.GenerateMesh(combinedNoise, _meshHeightMultiplayer, _levelOfDetails);
        _meshGenerator.UpdateMesh(_textureGenerator.GeneratedTexture);
        _meshGenerator.AddCollissions();

        DrawWater();
        SpawnObjects(combinedNoise, _meshFilter.transform.localScale.x);
    }

    private void DrawWater()
    {
        if (_terrainRegions == null || _terrainRegions.Length == 0)
            return;

        if (_waterPlaneTr == null)
            return;

        float waterRegionPosition = 0.0f;
        float waterPos = 0.0f;

        waterRegionPosition = _terrainRegions[0].height;
        waterPos = waterRegionPosition * _meshHeightMultiplayer * _meshFilter.transform.localScale.x;

        float scaleMultiplayer = _meshFilter.transform.localScale.x;

        _waterPlaneTr.localScale = new Vector3(_MAP_SIZE / 10.0f * scaleMultiplayer, 1.0f, _MAP_SIZE / 10.0f * scaleMultiplayer);
        _waterPlaneTr.position = new Vector3(_waterPlaneTr.position.x, waterPos, _waterPlaneTr.position.z);
    }

    private void SpawnObjects(float[,] noiceMap, float positionMultilayer)
    {
        if (!_isAutoSpawn)
            return;

        if (_terrainObjectRegions == null || _terrainObjectRegions.Length == 0)
            return;

        if (positionMultilayer == 0)
            positionMultilayer = 1;

        DestroyPrevSpawnedObject();

        int regionId = 0;
        int regionAmount = _terrainObjectRegions.Length;

        TerrainObjectRegion region = _terrainObjectRegions[regionId];
        GameObject spawnObj = region.terraingObject;
        MeshData meshData = _meshGenerator.MeshData;
        GameObject currentSpawnedObj = null;

        for (; regionId < regionAmount;)
        {
            int objAmount = region.amountOfObj;

            GameObject currentParent = new GameObject(region.regionName);

            Vector3[] vertPos = meshData.vertices;
            int vertSize = meshData.vertices.Length;

            Vector3[] spawnPos =
                GetSpawnPosition(objAmount, region.minSpawnHeight, region.maxSpawnHeight, noiceMap, meshData);

            for (int i = 0; i < objAmount; ++i)
            {
                //currentSpawnedObj = Instantiate(spawnObj, spawnPos[i] * 10.0f, Quaternion.identity);
                currentSpawnedObj = Instantiate(spawnObj, spawnPos[i] * positionMultilayer, Quaternion.identity);
                currentSpawnedObj.transform.parent = currentParent.transform;
            }

            _parentBox.Enqueue(currentParent);

            if (regionId + 1 >= regionAmount)
                break;

            region = _terrainObjectRegions[++regionId];
            spawnObj = region.terraingObject;
        }
    }

    private void DestroyPrevSpawnedObject()
    {
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            foreach (var parent in _parentBox)
                DestroyImmediate(parent.gameObject);
        }
        else
        {
            foreach (var parent in _parentBox)
                Destroy(parent.gameObject);
        }

        _parentBox.Clear();
    }

    private Vector3[] GetSpawnPosition(int size, float minHeightZone, float maxHeightZone, float[,] noiceMap, MeshData data)
    {

        int lod = _levelOfDetails == 0 ? 1 : _levelOfDetails * 2;

        Vector3[] res = new Vector3[size];

        int meshWidth = (noiceMap.GetLength(0) - 1) / lod + 1;
        int meshHeight = (noiceMap.GetLength(1) - 1) / lod + 1;

        int noiceWidth = noiceMap.GetLength(0);
        int noiceHeight = noiceMap.GetLength(1);

        float currentHeight = 0;

        int maxTries = 10;
        int currentTriesNumber = 0;

        for (int i = 0; i < size; ++i)
        {
            int posX = Random.Range(0, noiceWidth);
            int posY = Random.Range(0, noiceHeight);

            currentHeight = noiceMap[posX, posY];
            while (currentHeight < minHeightZone || currentHeight > maxHeightZone)
            {
                posX = Random.Range(0, noiceWidth);
                posY = Random.Range(0, noiceHeight);

                currentHeight = noiceMap[posX, posY];
                currentTriesNumber++;

                if (currentTriesNumber >= maxTries)
                {
                    currentTriesNumber = 0;
                    break;
                }
            }

            if (currentTriesNumber == 0)
                continue;

            posX = posX / lod + 1;
            posY = posY / lod + 1;

            if (posY * meshWidth + posX < data.vertices.Length)
                res[i] = data.vertices[posY * meshWidth + posX];
        }

        return res;
    }
}
