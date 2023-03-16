using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int position { get; private set; }
    public Vector3Int[] cellStates { get; private set; }
    
    public void Init(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;

        if (cellStates == null)
            cellStates = new Vector3Int[data.cells.Length];

        for (int i = 0; i < data.cells.Length; i++)
        {
            cellStates[i] = (Vector3Int)data.cells[i];
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
