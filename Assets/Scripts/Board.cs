using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Events;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new(10, 20);

    [SerializeField] private int previewCount;
    [SerializeField] private Vector3Int spawnPosition;
    [SerializeField] private Vector3Int swapPosition;
    [SerializeField] private Vector3Int previewPosition;

    private Queue<TetrominoData> tetrisQueue { get; set; }
    private Piece activePiece { get; set; }
    private Piece swapPiece { get; set; }
    private Piece[] previewPieces { get; set; }
    private int _tetrominoTypeFlags = 127;

    public GameStateEnum gameState;

    private RectInt Bounds
    {
        get
        {
            Vector2Int pos = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(pos, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        swapPiece = gameObject.AddComponent<Piece>();
        swapPiece.enabled = false;

        previewPieces = new Piece[previewCount];

        tetrisQueue = new Queue<TetrominoData>();
        for (int i = 0; i < previewCount; i++)
        {
            tetrisQueue.Enqueue(tetrominoes[GetNextPieceIndex()]);
            previewPieces[i] = gameObject.AddComponent<Piece>();
            previewPieces[i].enabled = false;
        }

        foreach (var tetro in tetrominoes)
        {
            tetro.Init();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(GameStateEnum.SpawningPiece);
    }

    public void ChangeState(GameStateEnum newState)
    {
        gameState = newState;

        switch (newState)
        {
            case GameStateEnum.InitBoard:
                break;
            case GameStateEnum.SpawningPiece:
                SpawnPiece();
                break;
            case GameStateEnum.ClearLine:
                StartCoroutine(ClearLines());
                break;
        }
    }

    public void SpawnPiece()
    {
        TetrominoData data = tetrisQueue.Dequeue();
        tetrisQueue.Enqueue(tetrominoes[GetNextPieceIndex()]);
        activePiece.Init(this, spawnPosition, data);
        
        UpdatePreview();

        if (IsValidPosition(activePiece, spawnPosition))
            SetPiece(activePiece);

        ChangeState(GameStateEnum.PlayerInput);
    }

    public void SetPiece(Piece piece)
    {
        foreach (var cell in piece.cellStates)
        {
            Vector3Int tilePos = cell + piece.position;
            tilemap.SetTile(tilePos, piece.data.tile);
        }
    }

    private void UpdatePreview()
    {
        var previewData = tetrisQueue.ToArray();
        Vector3Int pos = previewPosition;
        for (int i = 0; i < previewCount; i++)
        {
            pos.y -= 3;
            if (previewPieces[i].data != null)
                ClearPiece(previewPieces[i]);
            previewPieces[i].Init(this, pos, previewData[i]);
            SetPiece(previewPieces[i]);
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

    public void SwapPiece()
    {
        // if not holding anything
        if (swapPiece.data == null)
        {
            // set current piece as held
            swapPiece.Init(this, swapPosition, activePiece.data);
            SetPiece(swapPiece);

            // spawn new piece
            ChangeState(GameStateEnum.SpawningPiece);
        }
        else
        {
            // Swap cells
            // Temporarily store the current saved data so we can swap
            TetrominoData savedData = swapPiece.data;

            // Clear the existing saved piece from the board
            if (savedData.cells != null)
            {
                ClearPiece(swapPiece);
            }

            // Store the next piece as the new saved piece
            // Draw this piece at the "hold" position on the board
            swapPiece.Init(this, swapPosition, activePiece.data);
            SetPiece(swapPiece);

            // Swap the saved piece to be the next piece
            if (savedData.cells != null)
            {
                // Clear the existing next piece before swapping
                ClearPiece(activePiece);

                // Re-initialize the next piece with the saved data
                // Draw this piece at the "preview" position on the board
                activePiece.Init(this, activePiece.position, savedData);
                SetPiece(activePiece);
            }
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        foreach (var cell in piece.cellStates)
        {
            Vector3Int testPos = cell + position;

            if (!Bounds.Contains((Vector2Int)testPos))
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

    public IEnumerator ClearLines()
    {
        List<int> clearedRows = new();
        // Check if row clearable
        for (int row = Bounds.yMin; row < Bounds.yMax; row++)
        {
            if (IsRowFull(row))
            {
                clearedRows.Add(row);
            }
        }

        if (clearedRows.Any())
        {
            // Clear rows
            foreach (int row in clearedRows)
            {
                for (int col = Bounds.xMin; col < Bounds.xMax; col++)
                {
                    tilemap.SetTile(new Vector3Int(col, row, 0), null);
                }
            }

            yield return new WaitForSeconds(0.1f);

            // Push down above rows
            foreach (int row in clearedRows)
            {
                int curRow = row;
                while (curRow < Bounds.yMax)
                {
                    for (int col = Bounds.xMin; col < Bounds.xMax; col++)
                    {
                        var rowAbove = tilemap.GetTile(new Vector3Int(col, curRow + 1, 0));
                        tilemap.SetTile(new Vector3Int(col, curRow, 0), rowAbove);
                    }

                    curRow++;
                }
            }

            int score = 1000 * clearedRows.Count;
            this.PublishEvent(EventID.OnPlayerScore, score);
        }

        ChangeState(GameStateEnum.SpawningPiece);
    }

    private int GetNextPieceIndex()
    {
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

        return index;
    }

    private bool IsRowFull(int row)
    {
        for (int col = Bounds.xMin; col < Bounds.xMax; col++)
        {
            if (!tilemap.HasTile(new Vector3Int(col, row, 0)))
            {
                return false;
            }
        }

        return true;
    }
}