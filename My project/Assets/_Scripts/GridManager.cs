using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    public int cols = 6;
    public int rows = 5;
    public float cellSize = 1f;
    public GameObject blockPrefab; // Block prefab
    public Color[] colorSprites; // assign color sprites in inspector


    Block[,] grid;
    Vector2 origin = Vector2.zero; // bottom-left world position of grid


    public GameManager gameManager; // assign in inspector


    void Start()
    {
        grid = new Block[cols, rows];
        // choose origin so grid is centered around this GameObject
        origin = (Vector2)transform.position - new Vector2(cols - 1, rows - 1) * 0.5f * cellSize;
        FillInitialGrid();
    }


    void FillInitialGrid()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                SpawnBlockAt(x, y, RandomColorId());
            }
        }
    }


    Color RandomColorId()
    {
        return colorSprites[ Random.Range(0, colorSprites.Length)];
    }


    void SpawnBlockAt(int x, int y, Color colorId)
    {
        Vector2 pos = GridToWorld(x, y);
        GameObject go = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
        Block b = go.GetComponent<Block>();
        b.Initialize(colorId, x, y, this);
        grid[x, y] = b;
    }


    Vector2 GridToWorld(int x, int y)
    {
        return origin + new Vector2(x * cellSize, y * cellSize);
    }


    public void OnBlockClicked(int x, int y)
    {
        // stop the player from being able to click block is the game has ended
        if (gameManager == null || gameManager.IsGameOver) return; 

        Block clicked = grid[x, y];
        if (clicked == null) return;

        // Find the adjacent blocks
        List<Block> connected = GetConnectedBlocks(x, y);
        if (connected == null || connected.Count == 0)
        {
            Debug.Log("No connected blocks found.");
            return;
        }

        // Log the amount of block removed for debugging purposes
        Debug.Log($"Removing {connected.Count} blocks at {x},{y}");

        // Remove them from the grid and destroy the GameObjects
        foreach (Block b in connected)
        {
            // clear the grid reference first
            grid[b.gridX, b.gridY] = null;
            Destroy(b.gameObject);
        }

        // Update score and moves
        gameManager.AddScore(connected.Count);
        gameManager.UseMove();

        // make the blocks drop
        StartCoroutine(ApplyGravityAndRefill());
    }

    // gather the adjacents
    List<Block> GetConnectedBlocks(int startX, int startY)
    {
        Block start = grid[startX, startY];
        if (start == null) return null;

        Color targetColor = start.colorId;
        bool[,] visited = new bool[cols, rows];
        List<Block> found = new List<Block>(); // store the visited blocks of the same color
        Queue<(int x, int y)> queue = new Queue<(int x, int y)>(); 

        queue.Enqueue((startX, startY));
        visited[startX, startY] = true;

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        while (queue.Count > 0)
        {
            var (cx, cy) = queue.Dequeue();
            Block current = grid[cx, cy];
            if (current == null || current.colorId != targetColor)
                continue;

            found.Add(current);

            for (int i = 0; i < 4; i++)
            {
                int nx = cx + dx[i];
                int ny = cy + dy[i];

                // Check grid bounds
                if (nx < 0 || nx >= cols || ny < 0 || ny >= rows)
                    continue;

                if (visited[nx, ny])
                    continue;

                Block neighbor = grid[nx, ny];
                if (neighbor == null || neighbor.colorId != targetColor)
                    continue;

                visited[nx, ny] = true;
                queue.Enqueue((nx, ny));
            }
        }

        return found;
    }

    IEnumerator ApplyGravityAndRefill()
    {
        yield return new WaitForSeconds(0.08f);

        // drop blocks
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (grid[x, y] == null)
                {
                    // find the first block above this block
                    for (int aboveY = y + 1; aboveY < rows; aboveY++)
                    {
                        if (grid[x, aboveY] != null)
                        {
                            Block falling = grid[x, aboveY];
                            grid[x, y] = falling;
                            grid[x, aboveY] = null;

                            falling.gridX = x;
                            falling.gridY = y;

                            Vector2 targetPos = GridToWorld(x, y);
                            StartCoroutine(MoveBlockSmooth(falling, targetPos));
                            break;
                        }
                    }
                }
            }
        }

        // wait for the fall animations
        yield return new WaitForSeconds(0.18f);

        // create new blocks to fill the board
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (grid[x, y] == null)
                {
                    Color colorId = RandomColorId();
                    
                    SpawnBlockAt(x, y, colorId);

                    Block spawned = grid[x, y];
                    if (spawned != null)
                    {
                        Vector2 startPos = (Vector2)GridToWorld(x, y) + Vector2.up * rows * cellSize * 0.5f;
                        spawned.transform.position = startPos;
                        StartCoroutine(MoveBlockSmooth(spawned, GridToWorld(x, y)));
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.08f);
    }

    IEnumerator MoveBlockSmooth(Block block, Vector2 target)
    {
        float duration = 0.12f;
        Vector2 start = block.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            block.transform.position = Vector2.Lerp(start, target, t);
            yield return null;
        }

        block.transform.position = target;
    }

}