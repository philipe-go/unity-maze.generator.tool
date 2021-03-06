<h1 align="center"> UNITY - Maze Generator Tool </h1>

<p align="center"> ######################################################## </p>

## Section 1: The Project

<p align="center"> <img src="./media/maze.gif"></p>


A Unity Tool created to generate mazes based on a list of prefabs (floors and walls). The maze uses the backtracking algorithm to traverse a XY grid and randomly removes walls (with bitshift operations) to create a maze. 

## Section 2: Developer 

[Philipe Gouveia](https://github.com/philipe-go)

## Section 3: Asymptotic Analysis [Logical side]

#### O(1):

```csharp
EWallType GetOpposite(EWallType wall)
```
```csharp
List<Neighbour> GetUnvisitedCells(MazeCell cell, EWallType[,] maze, int column, int row)
```

#### O(N):
```csharp
EWallType[,] RunBackTracker(EWallType[,] maze, int column, int row)
```
#### O(N^2):
```csharp
EWallType[,] GenerateMaze(int column, int row)
```
```csharp
void DrawMaze(EWallType[,] maze)
```

## Section 4: Concept *[GeeksForGeeks](https://www.geeksforgeeks.org/backtracking-algorithms/)*

"This is better than naive approach (generating all possible combinations of digits and then trying every combination one by one) as it drops a set of permutations whenever it backtracks."