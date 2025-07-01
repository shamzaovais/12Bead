using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class gameAi : MonoBehaviour
{
    // Game Board Configuration
    private const int BOARD_SIZE = 5;
    private int[,] board = new int[BOARD_SIZE, BOARD_SIZE]; // 0 = empty, 1 = player, -1 = computer
    private Vector2Int selectedPiece = new Vector2Int(-1, -1);

    // Game State
    private bool isPlayerTurn = true;
    private bool gameOver = false;

    void Start()
    {
        InitializeBoard();
    }

    void InitializeBoard()
    {
        // Setup initial positions (player at top, computer at bottom)
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                if (i < 2) board[i, j] = 1;     // Player pieces
                else if (i > 2) board[i, j] = -1; // Computer pieces
                else board[i, j] = 0;           // Middle row empty
            }
        }
    }

    void Update()
    {
        if (!gameOver && isPlayerTurn)
        {
            HandlePlayerInput();
        }
    }

    void HandlePlayerInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2Int gridPos = ConvertToGrid(hit.point);
                
                if (selectedPiece.x == -1)
                {
                    // Select piece
                    if (board[gridPos.x, gridPos.y] == 1)
                    {
                        selectedPiece = gridPos;
                        HighlightValidMoves(gridPos);
                    }
                }
                else
                {
                    // Try to move
                    if (IsValidMove(selectedPiece, gridPos))
                    {
                        ExecuteMove(selectedPiece, gridPos);
                        CheckGameOver();
                        StartComputerTurn();
                    }
                    selectedPiece = new Vector2Int(-1, -1);
                }
            }
        }
    }

    void StartComputerTurn()
    {
        isPlayerTurn = false;
        // Simple AI: Random valid move
        List<Move> computerMoves = GetAllValidMoves(-1);
        if (computerMoves.Count > 0)
        {
            Move computerMove = computerMoves[Random.Range(0, computerMoves.Count)];
            StartCoroutine(ComputerMove(computerMove));
        }
        else
        {
            Debug.Log("Computer has no valid moves!");
            gameOver = true;
        }
    }

    IEnumerator ComputerMove(Move move)
    {
        yield return new WaitForSeconds(1f);
        
        ExecuteMove(move.start, move.end);
        CheckGameOver();
        isPlayerTurn = true;
    }

    bool IsValidMove(Vector2Int start, Vector2Int end)
    {
        // Check basic move rules
        if (board[start.x, start.y] == 0) return false;
        if (board[end.x, end.y] != 0) return false;
        
        // Orthogonal and diagonal movement check
        int dx = Mathf.Abs(end.x - start.x);
        int dy = Mathf.Abs(end.y - start.y);
        
        // Regular move (1 space)
        if (dx <= 1 && dy <= 1)
        {
            return true;
        }
        
        // Jump move (2 spaces)
        if (dx == 2 || dy == 2)
        {
            Vector2Int midpoint = new Vector2Int(
                (start.x + end.x) / 2,
                (start.y + end.y) / 2
            );
            return board[midpoint.x, midpoint.y] == -board[start.x, start.y];
        }
        
        return false;
    }

    void ExecuteMove(Vector2Int start, Vector2Int end)
    {
        // Handle jumps
        if (Mathf.Abs(end.x - start.x) > 1 || Mathf.Abs(end.y - start.y) > 1)
        {
            Vector2Int midpoint = new Vector2Int(
                (start.x + end.x) / 2,
                (start.y + end.y) / 2
            );
            board[midpoint.x, midpoint.y] = 0;
        }

        board[end.x, end.y] = board[start.x, start.y];
        board[start.x, start.y] = 0;
        
        UpdateVisuals(start, end);
    }

    List<Move> GetAllValidMoves(int player)
    {
        List<Move> validMoves = new List<Move>();
        
        for (int x = 0; x < BOARD_SIZE; x++)
        {
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                if (board[x, y] == player)
                {
                    // Check all possible moves
                    for (int dx = -2; dx <= 2; dx++)
                    {
                        for (int dy = -2; dy <= 2; dy++)
                        {
                            Vector2Int end = new Vector2Int(x + dx, y + dy);
                            if (IsValidMove(new Vector2Int(x, y), end))
                            {
                                validMoves.Add(new Move(new Vector2Int(x, y), end));
                            }
                        }
                    }
                }
            }
        }
        return validMoves;
    }

    void CheckGameOver()
    {
        int playerCount = 0;
        int computerCount = 0;
        
        foreach (int piece in board)
        {
            if (piece == 1) playerCount++;
            if (piece == -1) computerCount++;
        }

        if (playerCount == 0 || computerCount == 0)
        {
            gameOver = true;
            Debug.Log(playerCount == 0 ? "Computer Wins!" : "Player Wins!");
        }
    }

    // Helper methods
    Vector2Int ConvertToGrid(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x + 2);
        int y = Mathf.RoundToInt(position.z + 2);
        return new Vector2Int(
            Mathf.Clamp(x, 0, BOARD_SIZE-1),
            Mathf.Clamp(y, 0, BOARD_SIZE-1)
        );
    }

    void UpdateVisuals(Vector2Int start, Vector2Int end)
    {
        // Update your game objects here
        Debug.Log($"Moved from {start} to {end}");
    }

    void HighlightValidMoves(Vector2Int position)
    {
        // Implement visual highlighting of valid moves
    }

    struct Move
    {
        public Vector2Int start;
        public Vector2Int end;

        public Move(Vector2Int s, Vector2Int e)
        {
            start = s;
            end = e;
        }
    }
}
