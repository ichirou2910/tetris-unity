using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece ghostingPiece;
    
    public Tilemap tilemap { get; private set; }
    public Vector3Int position { get; private set; }
    public Vector3Int[] cellStates { get; private set; }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        cellStates = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        Clear();
        Clone();
        Drop();
        Set();
    }

    private void Clear()
    {
        foreach (var cell in cellStates)
        {
            Vector3Int tilePos = cell + position;
            tilemap.SetTile(tilePos, null);
        }
    }

    private void Clone()
    {
        Array.Copy(ghostingPiece.cellStates, cellStates, ghostingPiece.cellStates.Length);
    }

    private void Drop()
    {
        Vector3Int pos = ghostingPiece.position;

        int currentRow = pos.y;
        int bottomRow = -10;
        
        board.ClearPiece(ghostingPiece);

        for (int row = currentRow; row >= bottomRow; row--)
        {
            pos.y = row;
            if (!board.IsValidPosition(ghostingPiece, pos))
                break;
            this.position = pos;
        }
        
        board.SetPiece(ghostingPiece);
    }

    private void Set()
    {
        foreach (var cell in cellStates)
        {
            Vector3Int tilePos = cell + position;
            tilemap.SetTile(tilePos, tile);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
