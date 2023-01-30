using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Controls the direction of the node
/// Hoppers use the direction to determine where to go next
/// </summary>
public class HopperDirector : MonoBehaviour
{

    public enum Direction { up = 0, right = 1, down = 2, left = 3}
    public Direction myDirection;

    public int myRow, myCol;

    public GridCreator gridCreator;

    public GameObject targetObject;
    public Vector3 targetPosition;

    public void InitHopperDirector()
    {
        //set initial direction
        myDirection = Direction.up;

        //set the target to the object in the bottom row if we're at the top
        Debug.Log("num cols: " + gridCreator.grid.GetLength(0));
        Debug.Log("num rows: " + gridCreator.grid.GetLength(1));

        targetObject = gridCreator.grid[myCol, (myRow + 1) % gridCreator.grid.GetLength(1)];

        //and set the target position to a "dummy position" so the screen wrap works
        if (myRow == gridCreator.numRows - 1)
        {
            targetPosition = transform.position + new Vector3(0f, gridCreator.ySpacing, 0);
        }
        else
        {
            targetPosition = targetObject.transform.position;
        }
    }




    private void OnMouseDown()
    {
        //rotate direction
        myDirection += 1;
        transform.Rotate(new Vector3(0, 0, -90f));
        Debug.Log("my direction num: " + myDirection);
        if((int)myDirection > 3)
        {
            myDirection = Direction.up; 
        }

        switch (myDirection)
        {
            case Direction.up:
                targetObject = gridCreator.grid[myCol, (myRow + 1) % gridCreator.grid.GetLength(1)];
                if (myRow == gridCreator.numRows - 1)
                {
                    targetPosition = transform.position + new Vector3(0f, gridCreator.ySpacing, 0f);
                }
                else
                {
                    targetPosition = targetObject.transform.position;
                }
                break;
            case Direction.right:
                targetObject = gridCreator.grid[(myCol + 1) % gridCreator.grid.GetLength(0), myRow];
                if (myCol == gridCreator.numCols - 1)
                {
                    targetPosition = transform.position + new Vector3(gridCreator.xSpacing, 0f, 0f);
                }
                else
                {
                    targetPosition = targetObject.transform.position;
                }
                break;
            case Direction.down:
                //if we're at the bottom pointing down, set the target to the top
                if(myRow == 0)
                {
                    targetObject = gridCreator.grid[myCol, gridCreator.numRows - 1];
                    targetPosition = transform.position - new Vector3(0f, gridCreator.ySpacing, 0f);
                }
                else
                {
                    targetObject = gridCreator.grid[myCol, myRow - 1];
                    targetPosition = targetObject.transform.position;
                }
                
                break;
            case Direction.left:
                if(myCol == 0)
                {
                    targetObject = gridCreator.grid[gridCreator.numCols - 1, myRow];
                    targetPosition = transform.position - new Vector3(gridCreator.xSpacing, 0f, 0f);
                }
                else
                {
                    targetObject = gridCreator.grid[myCol - 1, myRow];
                    targetPosition = targetObject.transform.position;
                }

                break;

        }

    }
}
