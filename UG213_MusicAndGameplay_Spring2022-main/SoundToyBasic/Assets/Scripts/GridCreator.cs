using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a grid of HopperDirectors
/// </summary>
public class GridCreator : MonoBehaviour
{
    public float xMinimum = -5f;
    public float yMinimum = -5f;

    public float zPos = 0f;

    public int numRows = 4;
    public int numCols = 5;

    public float xSpacing = 1;
    public float ySpacing = 1;

    public GameObject objectToSpawn;

    public GameObject hopperObject;

    public GameObject[,] grid;

    public PositionToPitch posToPitch;



    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();

        //Spawning a few of the HopperMove Objects (the things that traverse the grid and make sound)
        CreateHopper(3, 5, 1);
        CreateHopper(1, 1, 2);
        CreateHopper(3, 3, 3);
        CreateHopper(numCols - 1, numRows - 1, 5);
    }

    public void CreateGrid()
    {
        grid = new GameObject[numCols, numRows];
        for(int i = 0; i < numCols; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                grid[i, j] = Instantiate(objectToSpawn,
                    new Vector3(
                        xMinimum + i * xSpacing,
                        yMinimum + j * ySpacing,
                        zPos
                    ),
                    Quaternion.identity);

                HopperDirector hDir = grid[i, j].GetComponent<HopperDirector>();
                hDir.myCol = i;
                hDir.myRow = j;

                hDir.gridCreator = this;
            }
        }

        //set the initial direction and direction object for each hopperDirector, after we've created the grid;
        foreach(GameObject go in grid)
        {
            go.GetComponent<HopperDirector>().InitHopperDirector();
        }
    }

    /// <summary>
    /// Create the hoppers after we create the grid
    /// </summary>
    /// <param name="startColumn">corresponds to x position</param>
    /// <param name="startRow">corresponds to y position</param>
    /// <param name="intervalCount">number of "beats" or beat subdivisions, controls the speed of the hopper</param>
    public void CreateHopper(int startColumn, int startRow, double intervalCount)
    {
        GameObject newHopper = Instantiate(hopperObject, grid[startColumn, startRow].transform.position, Quaternion.identity);
        HopperMove hopperScript = newHopper.GetComponent<HopperMove>();

        //need to initialize which director the hoppers start at, which in turn determines where they will go next
        hopperScript.prevDirector = grid[startColumn, startRow].GetComponent<HopperDirector>();
        hopperScript.posToPitch = posToPitch;

        //setting the boundary conditions so the hoppers can screen wrap
        hopperScript.xBoundLow = xMinimum - (xSpacing / 2f);
        hopperScript.xBoundHigh = xMinimum + (xSpacing * (numCols - 1)) + (xSpacing / 2f);
        hopperScript.yBoundLow = yMinimum - (ySpacing / 2f);
        hopperScript.yBoundHigh = yMinimum + (ySpacing * (numRows - 1)) + (ySpacing / 2f);

        //so we can have more odd numbered subdivisions (3s, 5s, etc)
        hopperScript.intervalCount = intervalCount;
    }

}
