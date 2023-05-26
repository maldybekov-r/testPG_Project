using System;
using Unity.VisualScripting;

public class MazeCell
{
    public enum CellType
    {
        Passed = 0,
        Free = 1
    }

    public enum WallType
    {
        top = 0,
        right = 1,
        bottom = 2,
        left = 3
    }


    private const int _NEIGHBOUR_AMOUNT = 4;

    private MazeCell[] _neighbours
        = new MazeCell[_NEIGHBOUR_AMOUNT];

    private System.Collections.Generic.List<UnityEngine.GameObject> _walls
        = new System.Collections.Generic.List<UnityEngine.GameObject>();

    private System.Collections.Generic.List<System.Tuple<MazeCell, int>> _analyzingCells
        = new System.Collections.Generic.List<System.Tuple<MazeCell, int>>();

    private System.Random rand = new System.Random();
    private MazeCell _parentCell;


    private UnityEngine.Vector2 _topPos2D;
    private UnityEngine.Vector2 _rightPos2D;
    private UnityEngine.Vector2 _bottomPos2D;
    private UnityEngine.Vector2 _leftPos2D;

    private UnityEngine.Vector3 _topPos3D;
    private UnityEngine.Vector3 _rightPos3D;
    private UnityEngine.Vector3 _bottomPos3D;
    private UnityEngine.Vector3 _leftPos3D;

    private UnityEngine.GameObject _top;
    private UnityEngine.GameObject _right;
    private UnityEngine.GameObject _bottom;
    private UnityEngine.GameObject _left;

    private float _wallSize;

    public CellType Type { get => _type; }
    private CellType _type;

    public int PosX { get => _posX; }
    private int _posX;

    public int PosY { get => _posY; }
    private int _posY;

    public MazeCell(int x, int y)
    {
        _posX = x;
        _posY = y;
    }

    ~MazeCell()
    {
        _neighbours = null;
        _walls.Clear();
    }

    public void AddNeighrbours(MazeCell neighbour, int index)
        => _neighbours[index] = neighbour;

    public void Spawn2DimensionWallsWithType(
        UnityEngine.GameObject wallPrefHor,
        UnityEngine.GameObject wallPrefVert,
        UnityEngine.Vector2 originPos,
        UnityEngine.Transform parentTr,
        float wallSize
        )
    {
        _wallSize = wallSize + 0.25f;
        originPos += new UnityEngine.Vector2(_posX * _wallSize, _posY * _wallSize);

        _topPos2D = new UnityEngine.Vector2(_posX, _wallSize + _posY);
        _rightPos2D = new UnityEngine.Vector2(_wallSize + _posX, _posY);
        _bottomPos2D = new UnityEngine.Vector2(_posX, -_wallSize + _posY);
        _leftPos2D = new UnityEngine.Vector2(-_wallSize + _posX, _posY);

        _top = UnityEngine.GameObject.Instantiate(wallPrefHor, _topPos2D + originPos, UnityEngine.Quaternion.identity);
        _right = UnityEngine.GameObject.Instantiate(wallPrefVert, _rightPos2D + originPos, UnityEngine.Quaternion.identity);
        _bottom = UnityEngine.GameObject.Instantiate(wallPrefHor, _bottomPos2D + originPos, UnityEngine.Quaternion.identity);
        _left = UnityEngine.GameObject.Instantiate(wallPrefVert, _leftPos2D + originPos, UnityEngine.Quaternion.identity);

        _top.transform.parent = parentTr;
        _right.transform.parent = parentTr;
        _bottom.transform.parent = parentTr;
        _left.transform.parent = parentTr;

        _walls.Add(_top);
        _walls.Add(_right);
        _walls.Add(_bottom);
        _walls.Add(_left);
    }

    public void Spawn3DimensionWallsWithType(
        UnityEngine.GameObject wallPrefHor,
        UnityEngine.GameObject wallPrefVert,
        UnityEngine.Vector3 originPos,
        UnityEngine.Transform parentTr,
        float wallSize
        )
    {
        _wallSize = wallSize + 0.25f;
        originPos += new UnityEngine.Vector3(_posX * _wallSize, 0.0f, _posY * _wallSize);

        _topPos3D = new UnityEngine.Vector3(_posX, 0.0f, _wallSize + _posY);
        _rightPos3D = new UnityEngine.Vector3(_wallSize + _posX, 0.0f, _posY);
        _bottomPos3D = new UnityEngine.Vector3(_posX, 0.0f, -_wallSize + _posY);
        _leftPos3D = new UnityEngine.Vector3(-_wallSize + _posX, 0.0f, _posY);

        _top = UnityEngine.GameObject.Instantiate(wallPrefHor, _topPos3D + originPos, UnityEngine.Quaternion.identity);
        _right = UnityEngine.GameObject.Instantiate(wallPrefVert, _rightPos3D + originPos, UnityEngine.Quaternion.identity);
        _bottom = UnityEngine.GameObject.Instantiate(wallPrefHor, _bottomPos3D + originPos, UnityEngine.Quaternion.identity);
        _left = UnityEngine.GameObject.Instantiate(wallPrefVert, _leftPos3D + originPos, UnityEngine.Quaternion.identity);

        _top.transform.parent = parentTr;
        _right.transform.parent = parentTr;
        _bottom.transform.parent = parentTr;
        _left.transform.parent = parentTr;

        _walls.Add(_top);
        _walls.Add(_right);
        _walls.Add(_bottom);
        _walls.Add(_left);
    }

    public void DestroyWalls()
    {
        if (_walls == null || _walls.Count == 0)
            return;

        foreach (var wall in _walls)
            UnityEngine.GameObject.Destroy(wall);
    }

    public void DestroyWallsImmediately()
    {
        if (_walls == null || _walls.Count == 0)
            return;

        foreach (var wall in _walls)
            UnityEngine.GameObject.DestroyImmediate(wall);
    }

    public void DestroyCertainWallByType(WallType type)
    {
        if (_walls.Count != 4)
            return;

        switch (type)
        {
            case WallType.top: UnityEngine.GameObject.DestroyImmediate(_walls[0]); break;
            case WallType.right: UnityEngine.GameObject.DestroyImmediate(_walls[1]); break;
            case WallType.bottom: UnityEngine.GameObject.DestroyImmediate(_walls[2]); break;
            case WallType.left: UnityEngine.GameObject.DestroyImmediate(_walls[3]); break;
        }
    }

    public void DestroyWallsRound()
    {
        DestroyWalls();

        _neighbours[0].DestroyCertainWallByType(WallType.bottom);
        _neighbours[1].DestroyCertainWallByType(WallType.left);
        _neighbours[2].DestroyCertainWallByType(WallType.top);
        _neighbours[3].DestroyCertainWallByType(WallType.right);
    }

    public bool TryGetRandomNeighbour(out MazeCell res)
    {
        res = default;
        _analyzingCells.Clear();

        for (int i = 0; i < _neighbours.Length; ++i)
            if (_neighbours[i].IsCellPickable())
                _analyzingCells.Add(new System.Tuple<MazeCell, int>(_neighbours[i], i));

        if (_analyzingCells.Count == 0)
        {
            res = _parentCell;
            _type = CellType.Passed;

            return _parentCell != null;
        }

        int indexOfCell = rand.Next(0, _analyzingCells.Count);
        _type = CellType.Passed;

        res = _analyzingCells[indexOfCell].Item1;

        res.SetParent(this);
        RemovePairWall(res, _analyzingCells[indexOfCell].Item2);

        return true;
    }

    private void RemovePairWall(MazeCell neighbourCell, int id)
    {
        switch (id)
        {
            case 0:
                DestroyCertainWallByType(WallType.top);
                neighbourCell.DestroyCertainWallByType(WallType.bottom);
                break;
            case 1:
                DestroyCertainWallByType(WallType.right);
                neighbourCell.DestroyCertainWallByType(WallType.left);
                break;
            case 2:
                neighbourCell.DestroyCertainWallByType(WallType.top);
                DestroyCertainWallByType(WallType.bottom);
                break;
            case 3:
                DestroyCertainWallByType(WallType.left);
                neighbourCell.DestroyCertainWallByType(WallType.right);
                break;
        }
    }

    public bool IsCellPickable()
    {
        if (_type == CellType.Passed)
            return false;
        else if (_posX == -1 && _posY == -1)
            return false;

        return true;
    }

    public void SetType(CellType type)
        => _type = type;

    public void SetParent(MazeCell parent)
        => _parentCell = parent;

    public MazeCell GetParent()
        => _parentCell;

    public override string ToString()
        => $"[{_posY}, {_posX}]";

    public void PrintNeighbours()
    {
        string res = default;

        for (int i = 0; i < _neighbours.Length; ++i)
            if (_neighbours != null)
                res += _neighbours[i].ToString();

        UnityEngine.Debug.Log($"[{_posX}, {_posY}] => ({res})");
    }
}