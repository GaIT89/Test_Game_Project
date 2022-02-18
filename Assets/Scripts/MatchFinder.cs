using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MatchFinder : MonoBehaviour
{
    public List<Gem> listGemMatched = new List<Gem>();

    private Board board;
    private void Awake()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        listGemMatched.Clear();
        for (int x = 0; x < board.boardWidth; x++)
        {
            for (int y = 0; y < board.boardHeight; y++)
            {
                Gem currentGem = board.allGemsOnBoard[x, y];
                if (currentGem != null)
                {
                    if (x > 0 && x < board.boardWidth - 1)
                    {
                        Gem leftGem = board.allGemsOnBoard[x - 1, y];
                        Gem rightGem = board.allGemsOnBoard[x + 1, y];
                        if (leftGem != null & rightGem != null)
                        {
                            if (leftGem.Type == currentGem.Type && rightGem.Type == currentGem.Type)
                            {
                                currentGem.isMatched = true;
                                leftGem.isMatched = true;
                                rightGem.isMatched = true;

                                listGemMatched.Add(rightGem);
                                listGemMatched.Add(leftGem);
                                listGemMatched.Add(currentGem);
                            }
                        }
                    }
                    if (y > 0 && y < board.boardHeight - 1)
                    {
                        Gem topGem = board.allGemsOnBoard[x, y + 1];
                        Gem botGem = board.allGemsOnBoard[x, y - 1];
                        if (topGem != null & botGem != null)
                        {
                            if (topGem.Type == currentGem.Type && botGem.Type == currentGem.Type)
                            {
                                currentGem.isMatched = true;
                                topGem.isMatched = true;
                                botGem.isMatched = true;

                                listGemMatched.Add(botGem);
                                listGemMatched.Add(currentGem);
                                listGemMatched.Add(topGem);
                            }
                        }
                    }
                }
            }
            if (listGemMatched.Count > 0)
            {
                listGemMatched = listGemMatched.Distinct().ToList();
            }
        }
    }
}

