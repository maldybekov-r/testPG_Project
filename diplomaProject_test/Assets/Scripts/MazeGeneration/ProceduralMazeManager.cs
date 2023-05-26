using UnityEngine;

public class ProceduralMazeManager : MonoBehaviour
{
    public enum WallType
    {
        top = 0,
        right = 1,
        bottom = 2,
        left = 3
    }

    public enum CellType
    {
        Passed = 0,
        Free = 1
    }

    public enum MazeGenerationType
    {
        twoDim = 0,
        threeDim = 1
    }

    [Header("Generation Components:")]
    [SerializeField] private GameObject _wallPrefHor2D;
    [SerializeField] private GameObject _wallPrefVert2D;

    [SerializeField] private GameObject _wallPrefHor3D;
    [SerializeField] private GameObject _wallPrefVert3D;

    [SerializeField] private Transform _wallParentTr;

    [Header("Generation Parameters:")]
    [Range(11, 33)]
    [SerializeField] private int _mazeSize;
    [SerializeField] private float _scaleMultiplayer;
    [SerializeField] private MazeGenerationType _mazeGenerationType;
    [SerializeField] private bool _isAutoGenerated;

    public bool AutoGenerated { get => _isAutoGenerated; }

    private CustomMaze _maze = null;

    public void OnGenerateMaze()
    {

        _wallParentTr.localScale = Vector3.one;

        if (_maze == null)
        {
            if (_mazeGenerationType == MazeGenerationType.twoDim)
                _maze = new CustomMaze(_wallPrefHor2D, _wallPrefVert2D, _wallParentTr, _mazeSize, _mazeGenerationType);
            else if (_mazeGenerationType == MazeGenerationType.threeDim)
                _maze = new CustomMaze(_wallPrefHor3D, _wallPrefVert3D, _wallParentTr, _mazeSize, _mazeGenerationType);
        }

        if (_mazeGenerationType == MazeGenerationType.twoDim)
        {

            if (_maze.TryResetUpdateMazeSize(
            new CustomMaze(
                _wallPrefHor2D,
                _wallPrefVert2D,
                _wallParentTr,
                _mazeSize,
                _mazeGenerationType
            ),
            out CustomMaze otherMaze))
                _maze = otherMaze;
        }
        else if (_mazeGenerationType == MazeGenerationType.threeDim)
        {
            if (_maze.TryResetUpdateMazeSize(
            new CustomMaze(
                _wallPrefHor3D,
                _wallPrefVert3D,
                _wallParentTr,
                _mazeSize,
                _mazeGenerationType
            ),
            out CustomMaze otherMaze))
                _maze = otherMaze;
        }

        _maze.GenerateMaze();
        SetTheScaleOfParent();
    }

    public void OnClenMaze()
    {
        if (_maze == null)
            return;

        _maze.ResetMaze();
    }

    private void SetTheScaleOfParent()
    {

        if (_scaleMultiplayer == 0)
            _scaleMultiplayer = 1;

        _wallParentTr.localScale = Vector3.one * _scaleMultiplayer;
    }
}
