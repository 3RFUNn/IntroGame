using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController1 : MonoBehaviour
{
    [Header("References")]
    public GameObject asteroidPrefab;  
    public GameObject gameArea;        

    [Header("Cellular Automata Settings")]
    [Range(1, 100)]
    public int iterations = 50;
    [Range(0f, 1f)]
    public float fillProbability = 0.45f;
    [Range(1, 8)]
    public int birthLimit = 4;
    [Range(1, 8)]
    public int deathLimit = 4;

    [Header("Path Generation Settings")]
    [Range(1, 7)]
    public int pathWidth = 3;        
    [Range(0f, 1f)]
    public float upwardBias = 0.6f;  

    [Header("Spawn Settings")]
    public float spawnZOffset = 15f;     
    public float playAreaEntryZ = 10f;   
    public float spawnCooldown = 2f;     

    // Private grid properties
    private int[,] mGrid;
    private int gridWidth;
    private int gridHeight;
    private float asteroidSize;
    private float gameAreaWidth;
    private float gameAreaHeight;
    
    // Spawn control
    private GameObject lastSpawnedAsteroid;
    private bool canSpawnNewRoom = true;
    
    // World space boundaries
    private Vector3 topRightCorner;
    private Vector3 bottomLeftCorner;

    // Path continuity tracking
    private int lastPathEndX;  // Track where the previous path ended
    private bool isFirstRoom = true;
    
    // Direction enum for random walk
    private enum Direction { Up, Left, Right }

    void Start()
    {
        InitializeGridSize();
        InitializeBounds();
        SpawnNewRoom(); // Spawn first room
    }

    void InitializeGridSize()
    {
        // Get sizes
        asteroidSize = asteroidPrefab.GetComponent<Renderer>().bounds.size.x;
        gameAreaWidth = gameArea.GetComponent<Renderer>().bounds.size.x;
        gameAreaHeight = gameArea.GetComponent<Renderer>().bounds.size.z;
        
        // Calculate grid dimensions
        gridWidth = Mathf.FloorToInt(gameAreaWidth / asteroidSize);
        gridHeight = Mathf.FloorToInt(gameAreaHeight / asteroidSize);
        
        // Initialize grid
        mGrid = new int[gridWidth, gridHeight];
        
        Debug.Log($"Grid initialized: {gridWidth}x{gridHeight}");
    }

    void InitializeBounds()
    {
        Bounds areaBounds = gameArea.GetComponent<Renderer>().bounds;
        
        topRightCorner = new Vector3(
            areaBounds.center.x + areaBounds.extents.x,
            0f,
            areaBounds.center.z + areaBounds.extents.z
        );
        
        bottomLeftCorner = new Vector3(
            areaBounds.center.x - areaBounds.extents.x,
            0f,
            areaBounds.center.z - areaBounds.extents.z
        );
        
        Debug.Log($"Bounds set - TopRight: {topRightCorner}, BottomLeft: {bottomLeftCorner}");
    }

    void CreateContinuousPath()
    {
        int startX;
        
        if (isFirstRoom)
        {
            // For the first room, start from a random position
            startX = Random.Range(pathWidth, gridWidth - pathWidth);
            isFirstRoom = false;
        }
        else
        {
            // For subsequent rooms, start from where the last path ended
            startX = lastPathEndX;
        }

        int currentX = startX;
        int currentY = 0;
        
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        int safetyCounter = 0;
        int maxIterations = gridWidth * gridHeight;
        
        // Smoother path generation with momentum
        int previousDirection = 0; // 0: neutral, -1: left, 1: right
        float directionChangeChance = 0.3f; // Chance to change direction
        
        while (currentY < gridHeight - 1 && safetyCounter < maxIterations)
        {
            // Clear path at current position
            for (int i = -pathWidth/2; i <= pathWidth/2; i++)
            {
                int clearX = currentX + i;
                if (clearX >= 0 && clearX < gridWidth)
                {
                    mGrid[clearX, currentY] = 0;
                }
            }
            
            visited.Add(new Vector2Int(currentX, currentY));
            
            // Determine next move with momentum
            if (Random.value < upwardBias)
            {
                currentY++;
            }
            else
            {
                if (Random.value < directionChangeChance)
                {
                    // Consider changing direction
                    List<int> possibleDirections = new List<int>();
                    
                    if (currentX > pathWidth) possibleDirections.Add(-1);
                    if (currentX < gridWidth - pathWidth - 1) possibleDirections.Add(1);
                    
                    if (possibleDirections.Count > 0)
                    {
                        previousDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];
                    }
                }
                
                // Apply current direction with bounds checking
                int newX = currentX + previousDirection;
                if (newX >= pathWidth && newX < gridWidth - pathWidth)
                {
                    currentX = newX;
                }
                else
                {
                    currentY++; // Move up if we can't move horizontally
                }
            }
            
            safetyCounter++;
        }
        
        // Clear final position and store it for next room
        for (int i = -pathWidth/2; i <= pathWidth/2; i++)
        {
            int clearX = currentX + i;
            if (clearX >= 0 && clearX < gridWidth)
            {
                mGrid[clearX, currentY] = 0;
            }
        }
        
        // Store the end position for the next room
        lastPathEndX = currentX;
        
        // Add some randomized wider areas occasionally for variety
        AddPathVariations();
    }

    void AddPathVariations()
    {
        int variationCount = Random.Range(2, 5);
        
        for (int i = 0; i < variationCount; i++)
        {
            int x = Random.Range(pathWidth, gridWidth - pathWidth);
            int y = Random.Range(0, gridHeight);
            int size = Random.Range(pathWidth + 1, pathWidth + 3);
            
            // Create a wider area
            for (int dx = -size; dx <= size; dx++)
            {
                for (int dy = -size; dy <= size; dy++)
                {
                    int newX = x + dx;
                    int newY = y + dy;
                    
                    if (newX >= 0 && newX < gridWidth && newY >= 0 && newY < gridHeight)
                    {
                        mGrid[newX, newY] = 0;
                    }
                }
            }
        }
    }

    void SpawnNewRoom()
    {
        // Generate pattern
        GenerateCAPattern();
        
        // Create continuous path instead of random path
        CreateContinuousPath();
        
        // Calculate cell sizes
        float cellWidth = gameAreaWidth / gridWidth;
        float cellHeight = gameAreaHeight / gridHeight;
        
        lastSpawnedAsteroid = null;
        
        // Spawn asteroids
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (mGrid[x,y] == 1)
                {
                    Vector3 basePosition = new Vector3(
                        topRightCorner.x - (x + 0.5f) * cellWidth,
                        0f,
                        bottomLeftCorner.z + (y + 0.5f) * cellHeight
                    );
                    
                    Vector3 spawnPosition = basePosition + new Vector3(0f, 0f, spawnZOffset);
                    
                    GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
                    lastSpawnedAsteroid = asteroid;
                }
            }
        }
        
        Debug.Log($"New room spawned with continuous path! Path ends at x: {lastPathEndX}");
    }

    void Update()
{
    CheckAndSpawnNewRoom();
}

void CheckAndSpawnNewRoom()
{
    if (lastSpawnedAsteroid != null && canSpawnNewRoom)
    {
        if (lastSpawnedAsteroid.transform.position.z <= playAreaEntryZ)
        {
            SpawnNewRoom();
            canSpawnNewRoom = false;
            StartCoroutine(ResetSpawnCooldown());
        }
    }
}

IEnumerator ResetSpawnCooldown()
{
    yield return new WaitForSeconds(spawnCooldown);
    canSpawnNewRoom = true;
}

void GenerateCAPattern()
{
    // Initialize random grid
    for (int x = 0; x < gridWidth; x++)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            mGrid[x,y] = Random.value < fillProbability ? 1 : 0;
        }
    }

    // Run CA iterations
    for (int i = 0; i < iterations; i++)
    {
        mGrid = DoSimulationStep(mGrid);
    }
}

int[,] DoSimulationStep(int[,] oldGrid)
{
    int[,] newGrid = new int[gridWidth, gridHeight];

    for (int x = 0; x < gridWidth; x++)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            int neighbors = CountAliveNeighbors(oldGrid, x, y);
            
            if (oldGrid[x,y] == 1)
            {
                newGrid[x,y] = (neighbors < deathLimit) ? 0 : 1;
            }
            else
            {
                newGrid[x,y] = (neighbors > birthLimit) ? 1 : 0;
            }
        }
    }

    return newGrid;
}

int CountAliveNeighbors(int[,] grid, int x, int y)
{
    int count = 0;
    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            int neighbor_x = x + i;
            int neighbor_y = y + j;

            if (i == 0 && j == 0) continue;

            if (neighbor_x < 0 || neighbor_y < 0 || 
                neighbor_x >= gridWidth || neighbor_y >= gridHeight)
            {
                count++;
                continue;
            }

            count += grid[neighbor_x, neighbor_y];
        }
    }
    return count;
}

// Debug visualization
void OnDrawGizmos()
{
    if (!Application.isPlaying || mGrid == null) return;
    
    float cellWidth = gameAreaWidth / gridWidth;
    float cellHeight = gameAreaHeight / gridHeight;
    
    // Draw path cells
    for (int x = 0; x < gridWidth; x++)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            if (mGrid[x,y] == 0)
            {
                Vector3 pos = new Vector3(
                    topRightCorner.x - (x + 0.5f) * cellWidth,
                    0f,
                    bottomLeftCorner.z + (y + 0.5f) * cellHeight
                );
                
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(pos, new Vector3(cellWidth * 0.8f, 0.1f, cellHeight * 0.8f));
            }
        }
    }
}
}