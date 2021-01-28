using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/*
#############################
  TITLE: MazeGeneratorTool.logic.cs
  AUTHOR: Philipe Go.
  DATE: 2021-Jan
#############################
*/

//FOR BITSHIFT OPERATIONS
[Flags]
public enum EWallType
{
    WEST = 1, //0001
    EAST = 2, //0010
    NORTH = 4, //0100
    SOUTH = 8, //1000

    VISITED = 128, //1000 0000
}

//CELL COORDINATES IN THE GRID
public struct MazeCell
{
    public int X;
    public int Y;
}

//VISITED NEIGHBOURS FOR BACKTRACKING ANALYSIS
public struct Neighbour
{
    public MazeCell mazeCell;
    public EWallType CommonWall;
}

public partial class MazeGeneratorTool : EditorWindow
{
    #region Parameters
    int column = 10;
    int row = 10;

    int wallArraySize = 0;
    int floorArraySize = 0;
    GameObject[] Wall;
    GameObject[] Floor;

    GameObject generatedMaze;

    const float GRIDSIZE = 1.0f;
    #endregion

    #region BackTrack Algorithm

    /// <summary> Check the opposite walls from a wall - O(1) </summary>
    /// <param name='wall'> the wall to check if there is an opposite </param>
    EWallType GetOpposite(EWallType wall)
    {
        switch (wall)
        {
            case EWallType.EAST: return EWallType.WEST;
            case EWallType.WEST: return EWallType.EAST;
            case EWallType.NORTH: return EWallType.SOUTH;
            case EWallType.SOUTH: return EWallType.NORTH;
            default: return EWallType.WEST;
        }
    }

    /// <summary> The backtracker algorithm to clean walls in the generated grid maze - O(n) </summary>
    /// <param name='EWallType[,]'> the 2Dmatrix grid - base for the maze </param>
    /// <param name='column'> number of columns in the grid </param>
    /// <param name='row'> number of rows in the grid </param>
    /// <returns> the cleaned 2D Grid - alread defined maze </returns>
    EWallType[,] RunBackTracker(EWallType[,] maze, int column, int row)
    {
        Stack<MazeCell> cellStack = new Stack<MazeCell>();
        System.Random random = new System.Random();
        MazeCell cell = new MazeCell { X = random.Next(0, column), Y = random.Next(0, row) };

        maze[cell.X, cell.Y] |= EWallType.VISITED;
        cellStack.Push(cell);

        while (cellStack.Count > 0)
        {
            MazeCell currentCell = cellStack.Pop();
            List<Neighbour> neighbours = GetUnvisitedCells(currentCell, maze, column, row);

            if (neighbours.Count > 0)
            {
                cellStack.Push(currentCell);

                //Get a random neighbour from the unvisited nodes list
                int randIndex = random.Next(0, neighbours.Count);
                Neighbour randNeighbour = neighbours[randIndex];

                MazeCell aCell = randNeighbour.mazeCell;

                maze[currentCell.X, currentCell.Y] &= ~randNeighbour.CommonWall;

                maze[aCell.X, aCell.Y] &= ~GetOpposite(randNeighbour.CommonWall);
                maze[aCell.X, aCell.Y] |= EWallType.VISITED;

                cellStack.Push(aCell);
            }
        }

        return maze;
    }

    /// <summary> Check for unvisited cells in the 2D matrix - O(1) </summary>
    /// <param name='cell'> the cell to be checked </param>
    /// <param name='maze'> the grid where the cell is located  </param>
    /// <param name='column'> number of columns in the grid </param>
    /// <param name='row'> number of rows in the grid </param>
    /// <returns> a list of unvisited nodes </returns>
    List<Neighbour> GetUnvisitedCells(MazeCell cell, EWallType[,] maze, int column, int row)
    {
        List<Neighbour> cellList = new List<Neighbour>();

        if (cell.X > 0) //0001
        {
            if (!maze[cell.X - 1, cell.Y].HasFlag(EWallType.VISITED))
            {
                cellList.Add(new Neighbour
                {
                    mazeCell = new MazeCell
                    {
                        X = cell.X - 1,
                        Y = cell.Y
                    },
                    CommonWall = EWallType.SOUTH,
                });
            }
        }

        if (cell.Y > 0) //1000
        {
            if (!maze[cell.X, cell.Y - 1].HasFlag(EWallType.VISITED))
            {
                cellList.Add(new Neighbour
                {
                    mazeCell = new MazeCell
                    {
                        X = cell.X,
                        Y = cell.Y - 1,
                    },
                    CommonWall = EWallType.EAST,
                });
            }
        }

        if (cell.X < column - 1) //0010
        {
            if (!maze[cell.X + 1, cell.Y].HasFlag(EWallType.VISITED))
            {
                cellList.Add(new Neighbour
                {
                    mazeCell = new MazeCell
                    {
                        X = cell.X + 1,
                        Y = cell.Y,
                    },
                    CommonWall = EWallType.NORTH,
                });
            }
        }

        if (cell.Y < row - 1) //0100
        {
            if (!maze[cell.X, cell.Y + 1].HasFlag(EWallType.VISITED))
            {
                cellList.Add(new Neighbour
                {
                    mazeCell = new MazeCell
                    {
                        X = cell.X,
                        Y = cell.Y + 1,
                    },
                    CommonWall = EWallType.WEST,
                });
            }
        }

        return cellList;
    }
    #endregion

    #region Maze Generation
    /// <summary> Generate a 2D Matrix grid to be used as base for the maze - O(n^2)</summary>
    /// <param name='column'> number of columns in the 2D matrix </param>
    /// <param name='row'> number of rows in the 2D matrix </param>
    /// <returns>A 2D matrix grid for the maze to be build NORTHon</returns>
    EWallType[,] GenerateMaze(int column, int row)
    {
        EWallType[,] maze = new EWallType[column, row];
        EWallType init = EWallType.SOUTH | EWallType.NORTH | EWallType.WEST | EWallType.EAST;
        for (int i = 0; i < column; ++i)
        {
            for (int j = 0; j < row; ++j)
            {
                maze[i, j] = init;
            }
        }
        return RunBackTracker(maze, column, row);
    }

    /// <summary>Method to draw the generated maze - O(n^2)</summary>
    /// <param name='maze'> The maze generated in the Build Method</param>
    void DrawMaze(EWallType[,] maze)
    {

        generatedMaze = new GameObject("Maze");
        Vector3 position = new Vector3();
        EWallType cell = new EWallType();
        System.Random random = new System.Random();

        for (int i = 0; i < column; ++i)
        {
            for (int j = 0; j < row; ++j)
            {
                cell = maze[i, j];
                position = new Vector3(-column / 2 + i, 0, -row / 2 + j);

                //TODO Change to instantiate random prefabs from a list of walls / floors
                if (cell.HasFlag(EWallType.NORTH))
                {
                    GameObject northWall = Instantiate(Wall[random.Next(0,Wall.Length)], generatedMaze.transform, true);
                    northWall.name = $"column:{i} / row:{j}";
                    northWall.transform.position = position + new Vector3(GRIDSIZE / 2, GRIDSIZE / 2, 0);
                    // topWall.transform.localScale = new Vector3(GRIDSIZE, topWall.transform.localScale.y, topWall.transform.localScale.z);
                }
                if (cell.HasFlag(EWallType.WEST))
                {
                    GameObject westWall = Instantiate(Wall[random.Next(0,Wall.Length)], generatedMaze.transform);
                    westWall.name = $"column:{i} / row:{j}";
                    westWall.transform.position = position + new Vector3(0, GRIDSIZE / 2, GRIDSIZE / 2);
                    westWall.transform.eulerAngles = new Vector3(90, 0, 0);
                    // WESTWall.transform.localScale = new Vector3(1,WESTWall.transform.localScale.y,WESTWall.transform.localScale.z);
                }

                if (j == 0)
                {
                    if (cell.HasFlag(EWallType.EAST))
                    {
                        GameObject eastWall = Instantiate(Wall[random.Next(0,Wall.Length)], generatedMaze.transform);
                        eastWall.name = $"column:{i} / row:{j}";
                        eastWall.transform.position = position + new Vector3(0, GRIDSIZE / 2, -GRIDSIZE / 2);
                        eastWall.transform.eulerAngles = new Vector3(90, 0, 0);
                    }
                }

                if (i == 0)
                {
                    if (cell.HasFlag(EWallType.SOUTH))
                    {
                        GameObject southWall = Instantiate(Wall[random.Next(0,Wall.Length)], generatedMaze.transform);
                        southWall.name = $"column:{i} / row:{j}";
                        southWall.transform.position = position + new Vector3(-GRIDSIZE / 2, GRIDSIZE / 2, 0);
                    }
                }


                GameObject cellFloor = Instantiate(Floor[random.Next(0,Floor.Length)], generatedMaze.transform);
                cellFloor.transform.position = position;
            }

        }
    }

    ///<summary>
    /// Method to build the maze calling the draw method
    ///</summary>
    void BuildMaze()
    {
        EWallType[,] maze = GenerateMaze(column, row);
        DrawMaze(maze);
    }
    #endregion
}
