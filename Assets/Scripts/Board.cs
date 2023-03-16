using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public TetrominoData[] tetrominoes;

    [SerializeField] private Vector3Int spawnPosition;

    private Piece activePiece { get; set; }
    private int _tetrominoTypeFlags = 127;

    private Vector2Int _boardSize = new Vector2Int(10, 20);

    private RectInt _bounds
    {
        get
        {
            Vector2Int pos = new Vector2Int(-_boardSize.x / 2, -_boardSize.y / 2);
            return new RectInt(pos, _boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        foreach (var tetro in tetrominoes)
        {
            tetro.Init();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnPiece();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void SpawnPiece()
    {
        // 7-bag
        int index;
        do
        {
            index = Random.Range(0, 7);
            // Index bit is 0 means the tetromino type there is spawned already
            if ((_tetrominoTypeFlags & (1 << index)) == 0)
            {
                index = -1;
                continue;
            }

            // Set index bit to 0
            _tetrominoTypeFlags &= ~(1 << index);
            // If empty bag, reset
            if (_tetrominoTypeFlags == 0)
                _tetrominoTypeFlags = 127;
        } while (index < 0);

        TetrominoData data = tetrominoes[index];
        activePiece.Init(this, spawnPosition, data);
        SetPiece(activePiece);
    }

    public void SetPiece(Piece piece)
    {
        foreach (var cell in piece.cellStates)
        {
            Vector3Int tilePos = cell + piece.position;
            tilemap.SetTile(tilePos, piece.data.tile);
        }
    }

    public void ClearPiece(Piece piece)
    {
        foreach (var cell in piece.cellStates)
        {
            Vector3Int tilePos = cell + piece.position;
            tilemap.SetTile(tilePos, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        foreach (var cell in piece.cellStates)
        {
            Vector3Int testPos = cell + position;

            if (!_bounds.Contains((Vector2Int)testPos))
            {
                return false;
            }

            if (tilemap.HasTile(testPos))
            {
                return false;
            }
        }

        return true;
    }
}