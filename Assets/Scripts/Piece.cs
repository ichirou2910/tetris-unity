using Enums;
using UnityEngine;
using Utils;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int position { get; private set; }
    public Vector3Int[] cellStates { get; private set; }
    private int rotationIndex { get; set; }

    [SerializeField] private float _stepTimeout = 1f;
    [SerializeField] private float _lockTimeout = .5f;
    [SerializeField] private float _tapTimeout;

    private float _stepTime;
    private float _lockTime;
    private Vector2 mousePos = Vector2.negativeInfinity;
    private float _buttonDownStart;

    public PieceStateEnum pieceState;

    public void Init(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;
        this._stepTime = Time.time + this._stepTimeout;
        this._lockTime = 0f;
        this.pieceState = PieceStateEnum.Init;

        if (cellStates == null)
            cellStates = new Vector3Int[data.cells.Length];

        for (int i = 0; i < data.cells.Length; i++)
        {
            cellStates[i] = (Vector3Int)data.cells[i];
        }
    }

    // Start is called before the first frame update
    // void Start()
    // {
    //     
    // }

    // Update is called once per frame
    void Update()
    {
        if (board.gameState != GameStateEnum.PlayerInput)
            return;

        board.ClearPiece(this);

        _lockTime += Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 currentMousePos = Input.mousePosition;
            if (mousePos.x < 0)
                mousePos = currentMousePos;
            _buttonDownStart = Time.time;
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 currentMousePos = Input.mousePosition;
            if (Time.time - _buttonDownStart > _tapTimeout)
            {
                float diffX = currentMousePos.x - mousePos.x;
                float diffY = currentMousePos.y - mousePos.y;

                // Horizontal swipe
                if (Mathf.Abs(diffX) >= Mathf.Abs(diffY))
                {
                    if (currentMousePos.x <= mousePos.x)
                    {
                        Rotate(-1);
                    }
                    else
                        Rotate(1);
                }
                // Vertical swip
                else
                {
                    if (currentMousePos.y <= mousePos.y)
                        HardDrop();
                    else
                    {
                        Swap();
                    }
                }
            }
            else
            {
                if (currentMousePos.y <= Screen.height / 2)
                {
                    if (currentMousePos.x <= Screen.width * 2 / 3)
                        Move(Vector2Int.down);
                }
                else
                {
                    if (currentMousePos.x <= Screen.width / 2)
                        Move(Vector2Int.left);
                    else
                        Move(Vector2Int.right);
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 currentMousePos = Input.mousePosition;
            mousePos = currentMousePos;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }
#endif

        // Automatically fall 1 step when not doing anything
        if (Time.time > _stepTime)
        {
            Step();
        }

        board.SetPiece(this);
    }

    private void Step()
    {
        Move(Vector2Int.down);
        _stepTime = Time.time + _stepTimeout;

        if (_lockTime >= _lockTimeout)
        {
            Lock();
        }
    }

    private void Lock()
    {
        board.SetPiece(this);
        board.ChangeState(GameStateEnum.ClearLine);
        // board.ClearLines();
        // board.SpawnPiece();
    }

    private bool Move(Vector2Int direction)
    {
        Vector3Int newPos = position;
        newPos.x += direction.x;
        newPos.y += direction.y;

        if (board.IsValidPosition(this, newPos))
        {
            position = newPos;
            _lockTime = 0f;
            return true;
        }

        return false;
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
        }

        Lock();
    }

    private void Rotate(int direction)
    {
        // Store the current rotation in case the rotation fails
        // and we need to revert
        int currentRotation = rotationIndex;

        // Rotate all of the cells using a rotation matrix
        rotationIndex = Helpers.WrapNumber(rotationIndex + direction, 0, 4);
        ApplyRotation(direction);

        // Revert the rotation if the wall kick tests fail
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = currentRotation;
            ApplyRotation(-direction);
        }
    }

    private void Swap()
    {
        board.SwapPiece();
    }

    private void ApplyRotation(int direction)
    {
        var matrix = TetrominoSRSData.RotationMatrix;

        // Rotate all of the cells using the rotation matrix
        for (int i = 0; i < cellStates.Length; i++)
        {
            Vector3 cell = cellStates[i];

            int finalX, finalY;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" and "O" are rotated from an offset center point
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    finalX = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    finalY = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    finalX = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    finalY = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            cellStates[i] = new Vector3Int(finalX, finalY, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        // even index is for incremental rotation index
        int wallKickIndex = rotationIndex * 2;

        // odd index is for decremental rotation index
        // therefore subtract by 1
        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Helpers.WrapNumber(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }
}