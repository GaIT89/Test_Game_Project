using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour
{
    public int playerScore;
    public int boardWidth ;
    public int boardHeight ;
    public float gemMoveSpeed;

    public List<Gem> listGemCanBlowUp = new List<Gem>();

    public enum GameState { wait, play }
    public GameState currentState = GameState.play;

    public GameObject bgBlockPrefab;
    public UIManager UI;
    public Gem[] gems;
    public Gem[,] allGemsOnBoard;

    public MatchFinder matchFind;

    private void Awake()
    {
        matchFind = FindObjectOfType<MatchFinder>();
        UI = FindObjectOfType<UIManager>();
    }
    void Start()
    {
        allGemsOnBoard = new Gem[boardWidth, boardHeight];

        SetupBoard();
    }
    void Update()
    {
        matchFind.FindAllMatches();
        FindAllGemCanMatch();
        Debug.Log("Gem Can BlowUp " + listGemCanBlowUp.Count);
    }

    private void SetupBoard()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                // Setup Block on Board
                Vector2 pos = new Vector2(x, y);
                GameObject bgBlock = Instantiate(bgBlockPrefab, pos, Quaternion.identity);

                // clean Hierachy 
                bgBlock.transform.parent = transform;
                bgBlock.name = "block : " + x + "- " + y;

                // Setup Gems on Board Randomly

                int gemToUse = Random.Range(0, gems.Length);

                int bounderLoop = 0;
                // Not Matched Gem when Spwan
                while (AlreadyMatch(new Vector2Int(x, y), gems[gemToUse]) && bounderLoop < 20)
                {
                    gemToUse = Random.Range(0, gems.Length);
                    bounderLoop++;
                }
                SpawnGem(new Vector2Int(x, y), gems[gemToUse]);
            }
        }
    }

    private void SpawnGem(Vector2Int posToSpawn, Gem gemToSpawn)
    {
        Gem gem = Instantiate(gemToSpawn, new Vector3(posToSpawn.x, posToSpawn.y + boardHeight, 0f), Quaternion.identity);
        // clean Hierachy 
        gem.transform.parent = transform;
        gem.name = "Gem : " + posToSpawn.x + "- " + posToSpawn.y;

        // store GemSpawned to gems collection
        allGemsOnBoard[posToSpawn.x, posToSpawn.y] = gem;
        gem.SetupGems(posToSpawn, this);
    }

    // Check for Already Matched Gem when Spwan
    private bool AlreadyMatch(Vector2Int posToCheck, Gem gemToCheck)
    {
        if (posToCheck.x > 1)
        {
            if (allGemsOnBoard[posToCheck.x - 1, posToCheck.y].Type == gemToCheck.Type &&
                allGemsOnBoard[posToCheck.x - 2, posToCheck.y].Type == gemToCheck.Type)
            {
                return true;
            }
        }

        if (posToCheck.y > 1)
        {
            if (allGemsOnBoard[posToCheck.x, posToCheck.y - 1].Type == gemToCheck.Type &&
                allGemsOnBoard[posToCheck.x, posToCheck.y - 2].Type == gemToCheck.Type)
            {
                return true;
            }
        }
        return false;
    }

    // Remove Match Gem 
    private void RemoveMatchGem(Vector2Int pos)
    {
        if (allGemsOnBoard[pos.x, pos.y] != null)
        {
            if (allGemsOnBoard[pos.x, pos.y].isMatched)
            {
                Destroy(allGemsOnBoard[pos.x, pos.y].gameObject);
                allGemsOnBoard[pos.x, pos.y] = null;
                playerScore++;
            }
        }
    }
    public void DestroyAllMatchInList()
    {
        for (int i = 0; i < matchFind.listGemMatched.Count; i++)
        {
            if (matchFind.listGemMatched[i] != null)
            {
                RemoveMatchGem(matchFind.listGemMatched[i].posIndex);
                SoundManager.instance.PlayWhenMatchGem();
            }
        }
        StartCoroutine(GemFallDown());
    }
    private IEnumerator GemFallDown()
    {
        yield return new WaitForSeconds(0.3f);

        int nullBlockCount = 0;

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (allGemsOnBoard[x, y] == null)
                {
                    nullBlockCount++;
                }
                else if (nullBlockCount > 0)
                {
                    allGemsOnBoard[x, y].posIndex.y -= nullBlockCount;
                    allGemsOnBoard[x, y - nullBlockCount] = allGemsOnBoard[x, y];
                    allGemsOnBoard[x, y] = null;
                }
            }
            nullBlockCount = 0;
        }
        StartCoroutine(FillBoard());
    }
    private IEnumerator FillBoard()
    {
        matchFind.FindAllMatches();

        if (matchFind.listGemMatched.Count > 0)
        {
            yield return new WaitForSeconds(0.6f);
            DestroyAllMatchInList();
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            ReFillTheBoard();
            matchFind.FindAllMatches();

            if (matchFind.listGemMatched.Count > 0)
            {
                yield return new WaitForSeconds(0.6f);
                DestroyAllMatchInList();
            }
        }
        yield return new WaitForSeconds(0.5f);

        FindAllGemCanMatch();

        currentState = Board.GameState.play;

        yield return new WaitForSeconds(2);
        if (IsNoMove())
        {
            UI.LoseGame();
            Debug.Log(" There's No More Gem To BlowUp");
        }


    }
    private void ReFillTheBoard()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (allGemsOnBoard[x, y] == null)
                {
                    int gemToUse = Random.Range(0, gems.Length);
                    SpawnGem(new Vector2Int(x, y), gems[gemToUse]);
                }
            }
        }
    }


    public void SwitchGemToCheckMatch(int x, int y, Vector2 direction)
    {
        var holder = allGemsOnBoard[x + (int)direction.x, y + (int)direction.y];
        allGemsOnBoard[x + (int)direction.x, y + (int)direction.y] = allGemsOnBoard[x, y];
        allGemsOnBoard[x, y] = holder;
    }

    private bool CheckForMatch()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                Gem currentGem = allGemsOnBoard[x, y];
                if (currentGem != null)
                {
                    if (x < boardWidth - 3)
                    {
                        if (allGemsOnBoard[x + 1, y] != null && allGemsOnBoard[x + 2, y] != null)
                        {
                            if (allGemsOnBoard[x + 1, y].Type == currentGem.Type && allGemsOnBoard[x + 2, y].Type == currentGem.Type)
                            {
                                return true;
                            }
                        }
                    }
                    if (y < boardHeight - 3)
                    {
                        if (allGemsOnBoard[x, y + 1] != null && allGemsOnBoard[x, y + 2] != null)
                        {
                            if (allGemsOnBoard[x, y + 1].Type == currentGem.Type && allGemsOnBoard[x, y + 2].Type == currentGem.Type)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck( int x, int y, Vector2 direction)
    {
        SwitchGemToCheckMatch(x, y, direction);
        if (CheckForMatch())
        {
            SwitchGemToCheckMatch(x, y, direction);
            return true;
        }
        SwitchGemToCheckMatch(x,y, direction);
        return false;
    }

    private bool IsNoMove()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (allGemsOnBoard[x, y] != null)
                {
                    if (x < boardWidth - 1)
                    {
                        if (SwitchAndCheck(x, y, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (y < boardHeight - 1)
                    {
                        if (SwitchAndCheck(x, y, Vector2.up))
                        {
                            return false;
                        }
                    }

                }
            }
        }
        return true;
    }
    public  void FindAllGemCanMatch()
    {
        listGemCanBlowUp.Clear();

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (allGemsOnBoard[x, y] != null)
                {
                    if (x < boardWidth - 1)
                    {
                        if (SwitchAndCheck(x, y, Vector2.right))
                        {
                            listGemCanBlowUp.Add(allGemsOnBoard[x, y]);
                        }
                    }
                    if (y < boardHeight - 1)
                    {
                        if (SwitchAndCheck(x, y, Vector2.up))
                        {
                            listGemCanBlowUp.Add(allGemsOnBoard[x, y]);
                        }
                    }
                    if (listGemCanBlowUp.Count > 0)
                    {
                        listGemCanBlowUp =  listGemCanBlowUp.Distinct().ToList();
                    }
                }
            }
        }
    }
}


