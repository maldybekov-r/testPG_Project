public class MeshData
{
    public UnityEngine.Vector3[] vertices;
    public int[] triangles;
    public UnityEngine.Vector2[] uv;

    private int _triangleId;
    private UnityEngine.Mesh _mesh;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new UnityEngine.Vector3[meshWidth * meshHeight];
        uv = new UnityEngine.Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[_triangleId] = a;
        triangles[_triangleId + 1] = b;
        triangles[_triangleId + 2] = c;

        _triangleId += 3;
    }

    public UnityEngine.Mesh GenerateMesh()
    {
        _mesh = new UnityEngine.Mesh();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uv;

        _mesh.RecalculateNormals();
        return _mesh;
    }
}