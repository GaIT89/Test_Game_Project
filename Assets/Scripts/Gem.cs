using System.Collections;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public Vector2Int posIndex;
    public Board gameBoard;
    public bool isMatched;
    public enum GemType { Air, Water, Ice, Fire, Forest, Flash, Earth }
    public GemType Type;

    private Vector2Int originalPosition;
    private Vector2 firstClickPosition;
    private Vector2 finalClickPosition;

    private bool isMouseClick;
    private Gem otherGem;
    private float moveGemAngle = 0f;

    void Update()
    {
        if (Vector2.Distance(transform.position, posIndex) > 0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, posIndex, gameBoard.gemMoveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(posIndex.x, posIndex.y, 0f);
            gameBoard.allGemsOnBoard[posIndex.x, posIndex.y] = this;
        }

        if (isMouseClick && Input.GetMouseButtonUp(0))
        {
            if (gameBoard.currentState == Board.GameState.play)
            {
                isMouseClick = false;
                finalClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CaculateMoveAngle();
            }
        }
    }

    public void SetupGems(Vector2Int pos, Board board)
    {
        posIndex = pos;
        gameBoard = board;
    }

    private void OnMouseDown()
    {
        if (gameBoard.currentState == Board.GameState.play)
        {
            firstClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isMouseClick = true;
        }
    }

    private void CaculateMoveAngle()
    {
        moveGemAngle = Mathf.Atan2(finalClickPosition.y - firstClickPosition.y, finalClickPosition.x - firstClickPosition.x);
        moveGemAngle = moveGemAngle * 180 / Mathf.PI;
        if (Vector3.Distance(firstClickPosition, finalClickPosition) > 0.5f)
        {
            CaculateWhereToMoveGem();
        }
    }

    private void CaculateWhereToMoveGem()
    {
        originalPosition = posIndex;

        if (moveGemAngle < 45 && moveGemAngle > -45 && posIndex.x < gameBoard.boardWidth - 1)
        {
            otherGem = gameBoard.allGemsOnBoard[posIndex.x + 1, posIndex.y];
            otherGem.posIndex.x--;
            posIndex.x++;

        }
        else if (moveGemAngle > 45 && moveGemAngle <= 135 && posIndex.y < gameBoard.boardHeight - 1)
        {
            otherGem = gameBoard.allGemsOnBoard[posIndex.x, posIndex.y + 1];
            otherGem.posIndex.y--;
            posIndex.y++;

        }
        else if (moveGemAngle < -45 && moveGemAngle >= -135 && posIndex.y > 0)
        {
            otherGem = gameBoard.allGemsOnBoard[posIndex.x, posIndex.y - 1];
            otherGem.posIndex.y++;
            posIndex.y--;
        }
        else if (posIndex.x > 0 && moveGemAngle > 135 || moveGemAngle < -135)
        {

            otherGem = gameBoard.allGemsOnBoard[posIndex.x - 1, posIndex.y];
            otherGem.posIndex.x++;
            posIndex.x--;
        }

        if (otherGem != null)
        {
            gameBoard.allGemsOnBoard[posIndex.x, posIndex.y] = this;
            gameBoard.allGemsOnBoard[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;
        }
        StartCoroutine(CheckWrongMove());
    }

    public IEnumerator CheckWrongMove()
    {
        gameBoard.currentState = Board.GameState.wait;

        gameBoard.matchFind.FindAllMatches();

        yield return new WaitForSeconds(0.5f);
        if (otherGem != null)
        {
            if (!isMatched && !otherGem.isMatched)
            {
                otherGem.posIndex = posIndex;
                posIndex = originalPosition;

                yield return new WaitForSeconds(0.5f);
                gameBoard.currentState = Board.GameState.play;
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
                gameBoard.DestroyAllMatchInList();
            }
        }
    }
}

