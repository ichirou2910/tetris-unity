using System.Collections.Generic;
using UnityEngine;

public static class StaticParams
{
    private static readonly float cos = Mathf.Cos(Mathf.PI / 2f);
    private static readonly float sin = Mathf.Sin(Mathf.PI / 2f);
    public static readonly float[] RotationMatrix = { 0f, 1f, -1f, 0f };

    public static readonly Dictionary<Tetromino, Vector2Int[]> Cells = new()
    {
        { Tetromino.I, new [] { new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 2, 1) } },
        { Tetromino.O, new [] { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Tetromino.T, new [] { new Vector2Int( 0, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Tetromino.J, new [] { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Tetromino.L, new [] { new Vector2Int( 1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Tetromino.S, new [] { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0) } },
        { Tetromino.Z, new[] { new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
    };

    private static readonly Vector2Int[,] WallKicksI = {
        { new(0, 0), new(-2, 0), new ( 1, 0), new (-2,-1), new ( 1, 2) },
        { new (0, 0), new ( 2, 0), new (-1, 0), new ( 2, 1), new (-1,-2) },
        { new (0, 0), new (-1, 0), new ( 2, 0), new (-1, 2), new ( 2,-1) },
        { new (0, 0), new ( 1, 0), new (-2, 0), new ( 1,-2), new (-2, 1) },
        { new (0, 0), new ( 2, 0), new (-1, 0), new ( 2, 1), new (-1,-2) },
        { new (0, 0), new (-2, 0), new ( 1, 0), new (-2,-1), new ( 1, 2) },
        { new (0, 0), new ( 1, 0), new (-2, 0), new ( 1,-2), new (-2, 1) },
        { new (0, 0), new (-1, 0), new ( 2, 0), new (-1, 2), new ( 2,-1) },
    };

    private static readonly Vector2Int[,] WallKicksOTJLSZ = {
        { new (0, 0), new (-1, 0), new (-1, 1), new (0,-2), new (-1,-2) },
        { new (0, 0), new ( 1, 0), new ( 1,-1), new (0, 2), new ( 1, 2) },
        { new (0, 0), new ( 1, 0), new ( 1,-1), new (0, 2), new ( 1, 2) },
        { new (0, 0), new (-1, 0), new (-1, 1), new (0,-2), new (-1,-2) },
        { new (0, 0), new ( 1, 0), new ( 1, 1), new (0,-2), new ( 1,-2) },
        { new (0, 0), new (-1, 0), new (-1,-1), new (0, 2), new (-1, 2) },
        { new (0, 0), new (-1, 0), new (-1,-1), new (0, 2), new (-1, 2) },
        { new(0, 0), new ( 1, 0), new ( 1, 1), new (0,-2), new ( 1,-2) },
    };

    public static readonly Dictionary<Tetromino, Vector2Int[,]> WallKicks = new()
    {
        { Tetromino.I, WallKicksI },
        { Tetromino.O, WallKicksOTJLSZ },
        { Tetromino.T, WallKicksOTJLSZ },
        { Tetromino.J, WallKicksOTJLSZ },
        { Tetromino.L, WallKicksOTJLSZ },
        { Tetromino.S, WallKicksOTJLSZ },
        { Tetromino.Z, WallKicksOTJLSZ },
    };
}
