using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController1 : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public GameObject gameArea;
    private int[,] mGrid;
    
    // Cellular Automata parameters
    public int iterations = 50;
    public float fillProbability = 0.45f;
    public int birthLimit = 4;
    public int deathLimit = 4;
    
    // Grid properties
    private int gridWidth;
    private int gridHeight;
    private float asteroidSize;
    private float gameAreaWidth;
    private float gameAreaHeight;
    
    // Reference to the last spawned asteroid
    private GameObject lastSpawnedAsteroid;
    
    // Bounds tracking
    private Vector3 topRightCorner;
    private Vector3 bottomLeftCorner;

    void Start()
    {
        InitializeGridSize();
        InitializeBounds();
        SpawnAsteroidsFromCA();
    }

    void InitializeGridSize()
    {
        // Get asteroid size from prefab
        asteroidSize = asteroidPrefab.GetComponent<Renderer>().bounds.size.x;
        
        // Get game area dimensions
        gameAreaWidth = gameArea.GetComponent<Renderer>().bounds.size.x;
        gameAreaHeight = gameArea.GetComponent<Renderer>().bounds.size.z;  // Using Z as height
        
        // Calculate grid dimensions
        gridWidth = Mathf.FloorToInt(gameAreaWidth / asteroidSize);
        gridHeight = Mathf.FloorToInt(gameAreaHeight / asteroidSize);
        
        // Initialize the grid
        mGrid = new int[gridWidth, gridHeight];
        
        Debug.Log($"Grid size: {gridWidth}x{gridHeight}");
    }

    void InitializeBounds()
    {
        // Get the renderer bounds of the game area
        Bounds areaBounds = gameArea.GetComponent<Renderer>().bounds;
        
        // Calculate corners in world space
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
        
        Debug.Log($"Top Right Corner: {topRightCorner}");
        Debug.Log($"Bottom Left Corner: {bottomLeftCorner}");
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

        // Run cellular automata iterations
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

    void SpawnAsteroidsFromCA()
    {
        // First generate the CA pattern
        GenerateCAPattern();
        
        // Calculate cell size
        float cellWidth = gameAreaWidth / gridWidth;
        float cellHeight = gameAreaHeight / gridHeight;
        
        // Iterate through the grid and spawn asteroids
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (mGrid[x,y] == 1)
                {
                    // Calculate spawn position
                    // Note: Y coordinate in grid is flipped to spawn from bottom to top
                    Vector3 spawnPosition = new Vector3(
                        topRightCorner.x - (x + 0.5f) * cellWidth,  // Start from right, move left
                        0f,
                        bottomLeftCorner.z + (y + 0.5f) * cellHeight  // Start from bottom, move up
                    );
                    
                    // Spawn the asteroid
                    lastSpawnedAsteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
                    
                    // Optional: Add debug visualization
                    Debug.DrawLine(spawnPosition, spawnPosition + Vector3.up * 2f, Color.red, 5f);
                }
            }
        }
        
        if (lastSpawnedAsteroid != null)
        {
            Debug.Log($"Last asteroid spawned at: {lastSpawnedAsteroid.transform.position}");
        }
    }

    // Method to get the last spawned asteroid (can be useful for other scripts)
    public GameObject GetLastSpawnedAsteroid()
    {
        return lastSpawnedAsteroid;
    }
}