public class MapMeshGenerator
{
    public MeshData MeshData { get => _meshData; }
    private MeshData _meshData;

    private UnityEngine.MeshFilter _meshFilter;
    private UnityEngine.MeshRenderer _meshRenderer;

    private UnityEngine.MeshCollider _currentMeshCollider;

    public MapMeshGenerator(ref UnityEngine.MeshFilter filter, ref UnityEngine.MeshRenderer meshRenderer)
    {
        _meshFilter = filter;
        _meshRenderer = meshRenderer;
    }

    public void GenerateMesh(float[,] heightMap, float heightMultiplayer, int levelOfDetail)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        int meshSimplificationIncrememt = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrememt + 1;

        float topLeftX = (width - 1) * -0.5f;
        float topLeftZ = (height - 1) * 0.5f;

        _meshData = new MeshData(meshWidth: verticesPerLine, meshHeight: verticesPerLine);
        int vertexId = 0;

        for (int y = 0; y < height; y += meshSimplificationIncrememt)
            for (int x = 0; x < width; x += meshSimplificationIncrememt)
            {
                _meshData.vertices[vertexId] =
                    new UnityEngine.Vector3(topLeftX + x, heightMap[x, y] * heightMultiplayer, topLeftZ - y);

                _meshData.uv[vertexId] =
                    new UnityEngine.Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    _meshData.AddTriangle(vertexId, vertexId + verticesPerLine + 1, vertexId + verticesPerLine);
                    _meshData.AddTriangle(vertexId + verticesPerLine + 1, vertexId, vertexId + 1);
                }

                vertexId++;
            }
    }

    public void AddCollissions()
    {
        if (_meshData == null)
            return;

        _currentMeshCollider = _meshFilter.GetComponent<UnityEngine.MeshCollider>();

        if (_currentMeshCollider == null)
            _currentMeshCollider = _meshFilter.gameObject.AddComponent<UnityEngine.MeshCollider>();

        _currentMeshCollider.sharedMesh = _meshData.GenerateMesh();
    }

    public void UpdateMesh(UnityEngine.Texture2D texture)
    {
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            _meshFilter.sharedMesh = _meshData.GenerateMesh();
            _meshRenderer.sharedMaterial.mainTexture = texture;
        }
        else
        {
            _meshFilter.mesh = _meshData.GenerateMesh();
            _meshRenderer.material.mainTexture = texture;
        }
    }
}
