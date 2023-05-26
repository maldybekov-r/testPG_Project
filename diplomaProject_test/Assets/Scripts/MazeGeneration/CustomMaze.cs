public class CustomMaze
{
    public UnityEngine.Transform MazeParentTr { get => _mazeParentTr; }
    public UnityEngine.Transform WallParentTr { get => _wallParentTr; }
    public UnityEngine.GameObject WallPrefHor { get => _wallPrefHor; }
    public UnityEngine.GameObject WallPrefVer { get => _wallPrefVer; }
    public int MazeSize { get => _mazeSize; }
    public ProceduralMazeManager.MazeGenerationType GenerationType { get => _generationType; }


    private const int _MAZE_DEFAULT_SIZE = 11;
    private readonly UnityEngine.Vector2 _MAZE_DEFAULT_SCALE_2D = new UnityEngine.Vector2(1.0f, 1.0f);
    private readonly UnityEngine.Vector3 _MAZE_DEFAULT_SCALE_3D = new UnityEngine.Vector3(1.0f, 1.0f, 1.0f);

    private MazeCell[,] _mazeCell = null;

    private UnityEngine.Transform _mazeParentTr = default;
    private UnityEngine.Transform _wallParentTr = default;

    private UnityEngine.GameObject _wallPrefHor = default;
    private UnityEngine.GameObject _wallPrefVer = default;

    private int _mazeSize = 0;
    private ProceduralMazeManager.MazeGenerationType _generationType;

    public CustomMaze(
        UnityEngine.GameObject wallPrefHor,
        UnityEngine.GameObject wallPrefVert,
        UnityEngine.Transform parent,
        int size = _MAZE_DEFAULT_SIZE,
        ProceduralMazeManager.MazeGenerationType generationType = ProceduralMazeManager.MazeGenerationType.twoDim
    )
    {
        _mazeParentTr = parent;
        _wallParentTr = parent;

        _wallPrefHor = wallPrefHor;
        _wallPrefVer = wallPrefVert;

        _mazeSize = size;
        _mazeCell = new MazeCell[size, size];
        _generationType = generationType;
    }

    ~CustomMaze()
    {
        _mazeCell = null;
    }

    private void Init2DimensionDataWithWalls()
    {
        _mazeCell = new MazeCell[_mazeSize, _mazeSize];
        _wallParentTr.localScale = _MAZE_DEFAULT_SCALE_2D;

        UnityEngine.Vector2 originPoint = UnityEngine.Vector2.zero - UnityEngine.Vector2.one * _mazeSize * 0.5f;
        float wallSize = UnityEngine.Mathf.Max(_wallPrefVer.transform.localScale.y, _wallPrefVer.transform.localScale.x);

        for (int y = 0; y < _mazeSize; ++y)
            for (int x = 0; x < _mazeSize; ++x)
            {
                _mazeCell[y, x] = new MazeCell(x, y);
                _mazeCell[y, x].SetType(MazeCell.CellType.Free);
                _mazeCell[y, x].Spawn2DimensionWallsWithType(_wallPrefHor, _wallPrefVer, originPoint, _wallParentTr, wallSize);
            }

        for (int y = 0; y < _mazeSize; ++y)
            for (int x = 0; x < _mazeSize; ++x)
            {
                _mazeCell[y, x].AddNeighrbours((y + 1 >= _mazeSize) ? new MazeCell(-1, -1) : _mazeCell[y + 1, x], 0);
                _mazeCell[y, x].AddNeighrbours((x + 1 >= _mazeSize) ? new MazeCell(-1, -1) : _mazeCell[y, x + 1], 1);
                _mazeCell[y, x].AddNeighrbours((y - 1 < 0) ? new MazeCell(-1, -1) : _mazeCell[y - 1, x], 2);
                _mazeCell[y, x].AddNeighrbours((x - 1 < 0) ? new MazeCell(-1, -1) : _mazeCell[y, x - 1], 3);
            }
    }

    private void Init3DimensionDataWithWalls()
    {
        _mazeCell = new MazeCell[_mazeSize, _mazeSize];
        _wallParentTr.localScale = _MAZE_DEFAULT_SCALE_3D;

        UnityEngine.Vector3 originPoint = new UnityEngine.Vector3(-_mazeSize * 0.5f, 0.0f, -_mazeSize * 0.5f);
        float wallSize = UnityEngine.Mathf.Max(_wallPrefVer.transform.localScale.y, _wallPrefVer.transform.localScale.x);

        for (int y = 0; y < _mazeSize; ++y)
            for (int x = 0; x < _mazeSize; ++x)
            {
                _mazeCell[y, x] = new MazeCell(x, y);
                _mazeCell[y, x].SetType(MazeCell.CellType.Free);
                _mazeCell[y, x].Spawn3DimensionWallsWithType(_wallPrefHor, _wallPrefVer, originPoint, _wallParentTr, wallSize);
            }

        for (int y = 0; y < _mazeSize; ++y)
            for (int x = 0; x < _mazeSize; ++x)
            {
                _mazeCell[y, x].AddNeighrbours((y + 1 >= _mazeSize) ? new MazeCell(-1, -1) : _mazeCell[y + 1, x], 0);
                _mazeCell[y, x].AddNeighrbours((x + 1 >= _mazeSize) ? new MazeCell(-1, -1) : _mazeCell[y, x + 1], 1);
                _mazeCell[y, x].AddNeighrbours((y - 1 < 0) ? new MazeCell(-1, -1) : _mazeCell[y - 1, x], 2);
                _mazeCell[y, x].AddNeighrbours((x - 1 < 0) ? new MazeCell(-1, -1) : _mazeCell[y, x - 1], 3);
            }
    }


    private void SetRandomPath()
    {
        int centerPoint = (int)(_mazeSize * 0.5f);
        StartWalking(_mazeCell[centerPoint, centerPoint]);
    }

    private void StartWalking(MazeCell startPoint)
    {
        if (startPoint == null)
            return;

        if (startPoint.TryGetRandomNeighbour(out MazeCell res))
            StartWalking(res);
    }

    private void ClearMaze()
    {
        if (_mazeCell == null || _mazeCell.GetLength(0) == 0)
            return;

        for (int r = 0; r < _mazeSize; ++r)
            for (int c = 0; c < _mazeSize; ++c)
                _mazeCell[r, c]?.DestroyWalls();
        _mazeCell = null;
    }

    private void DestroyMaze()
    {
        if (_mazeCell == null || _mazeCell.GetLength(0) == 0)
            return;

        for (int r = 0; r < _mazeSize; ++r)
            for (int c = 0; c < _mazeSize; ++c)
                _mazeCell[r, c]?.DestroyWallsImmediately();
        _mazeCell = null;
    }

    public void GenerateMaze()
    {
        ResetMaze();

        if (_generationType == ProceduralMazeManager.MazeGenerationType.twoDim)
            Init2DimensionDataWithWalls();
        else if (_generationType == ProceduralMazeManager.MazeGenerationType.threeDim)
            Init3DimensionDataWithWalls();

        SetRandomPath();

        if (_generationType == ProceduralMazeManager.MazeGenerationType.twoDim)
            _mazeParentTr.localScale = _MAZE_DEFAULT_SCALE_2D;
        else if (_generationType == ProceduralMazeManager.MazeGenerationType.threeDim)
            _mazeParentTr.localScale = _MAZE_DEFAULT_SCALE_3D;
    }

    public void ResetMaze()
    {
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            ClearMaze();
        else
            DestroyMaze();
    }

    public override string ToString()
    {
        string res = default;
        for (int r = 0; r < _mazeSize; ++r)
        {
            for (int c = 0; c < _mazeSize; ++c)
                res += _mazeCell[r, c] + " ";
            res += '\n';
        }

        return res;
    }

    public bool TryResetUpdateMazeSize(
        CustomMaze otherMaze,
        out CustomMaze maze
        )
    {
        maze = otherMaze;
        if (_mazeSize != otherMaze.MazeSize ||
            _generationType != otherMaze.GenerationType ||
            _wallParentTr != otherMaze.WallParentTr ||
            _wallPrefHor.GetInstanceID() != otherMaze.WallPrefHor.GetInstanceID() ||
            _wallPrefVer.GetInstanceID() != otherMaze.WallPrefVer.GetInstanceID()
            )
        {

            ResetMaze();
            return true;
        }

        return false;
    }
}
