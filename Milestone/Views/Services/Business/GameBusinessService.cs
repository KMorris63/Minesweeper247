using Milestone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/**
 * Kacey Morris
 * Alex Vergara
 * March 15 2021
 * CST 247
 * Milestone 3 - Flags and Game Over
 * GameBusinessService.cs
 * 
 * This class contains all the game rules and logic to play minesweeper. 
 * 
 * This is my own work, as influenced by class time and video tutorials for previous projects.
 */

namespace Milestone.Views.Services.Business
{
    public class GameBusinessService
    {
        // pass the board of cells
        private static Board myBoard;
        // data constructor
        public GameBusinessService(Board board)
        {
            myBoard = board;
        }

        // GAME LOGIC
        // assign cells in the grid as bombs
        public void setupLiveNeighbors()
        {
            // random to generate location of bombs
            Random rand = new Random();

            // generate a random row and column, from 0 to the size of the board
            int randomRow = rand.Next(myBoard.GetSize());
            int randomCol = rand.Next(myBoard.GetSize());

            // the percentage will be a double value that will determine the number of bombs
            double percentage = myBoard.GetDifficulty() / 100.0;

            // the number of bombs will be an integer determined by the total number of cells on the board
            // multiplied by the percentage of cells that are bombs based on the difficulty, forced to be an int
            int numLiveCells = Convert.ToInt32((myBoard.GetSize() * myBoard.GetSize()) * percentage);

            // for however many bombs there are, choose random locations for them
            for (int i = 0; i < numLiveCells; i++)
            {
                // if the cell is already live, we want to keep generating more random locations
                while (myBoard.GetGrid()[randomRow, randomCol].GetLive())
                {
                    randomRow = rand.Next(myBoard.GetSize());
                    randomCol = rand.Next(myBoard.GetSize());
                }
                // set that cell to live
                myBoard.GetGrid()[randomRow, randomCol].SetLive(true);
            }
        }

        // calculate the live neighbors
        public void calculateLiveNeighbors()
        {
            // loop through all cells on the board
            for (int i = 0; i < myBoard.GetSize(); i++)
            {
                for (int j = 0; j < myBoard.GetSize(); j++)
                {
                    Cell currentCell = myBoard.GetGrid()[i, j];
                    // if the current cell is live, it has 9 live neighbors as per the directions
                    if (currentCell.GetLive())
                    {
                        currentCell.SetNeighbors(9);
                    }
                    else
                    {
                        // set default values off the board so an accidental neighbor is not counted 
                        int manipulatedRow = -1;
                        int manipulatedCol = -1;
                        // set the neighbor counter to 0 for each cell to start
                        int neighborCount = 0;
                        // there are 8 possible neighbors for each cell, so loop through 8 times
                        for (int x = 0; x < 8; x++)
                        {
                            // on each loop through, check a different neighboring cell for a bomb
                            switch (x)
                            {
                                // each of these cases chooses a cell immediately surrounding the current cell
                                // to the right and down
                                case 0:
                                    manipulatedRow = currentCell.GetRowNumber() + 1;
                                    manipulatedCol = currentCell.GetColumnNumber() + 1;
                                    break;
                                // to the right
                                case 1:
                                    manipulatedRow = currentCell.GetRowNumber() + 1;
                                    manipulatedCol = currentCell.GetColumnNumber();
                                    break;
                                // to the right and up
                                case 2:
                                    manipulatedRow = currentCell.GetRowNumber() + 1;
                                    manipulatedCol = currentCell.GetColumnNumber() - 1;
                                    break;
                                // up
                                case 3:
                                    manipulatedRow = currentCell.GetRowNumber();
                                    manipulatedCol = currentCell.GetColumnNumber() - 1;
                                    break;
                                // down
                                case 4:
                                    manipulatedRow = currentCell.GetRowNumber();
                                    manipulatedCol = currentCell.GetColumnNumber() + 1;
                                    break;
                                // left and down
                                case 5:
                                    manipulatedRow = currentCell.GetRowNumber() - 1;
                                    manipulatedCol = currentCell.GetColumnNumber() + 1;
                                    break;
                                // left
                                case 6:
                                    manipulatedRow = currentCell.GetRowNumber() - 1;
                                    manipulatedCol = currentCell.GetColumnNumber();
                                    break;
                                // left and up
                                case 7:
                                    manipulatedRow = currentCell.GetRowNumber() - 1;
                                    manipulatedCol = currentCell.GetColumnNumber() - 1;
                                    break;
                                default:
                                    break;
                            }
                            // after the neighboring cell is selected, make sure it is on the board and check if it is live
                            if ((manipulatedRow < myBoard.GetSize()) && (manipulatedRow >= 0) && (manipulatedCol < myBoard.GetSize()) && (manipulatedCol >= 0)
                                && myBoard.GetGrid()[manipulatedRow, manipulatedCol].GetLive())
                            {
                                // if live, up the bomb counter
                                neighborCount++;
                            }
                        }
                        // after all neigboring cells are looped through, set the neighbor count for that cell
                        currentCell.SetNeighbors(neighborCount);
                    }
                }
            }
        }

        // checks for a winning condition
        public Boolean checkForWin()
        {
            for (int i = 0; i < myBoard.GetSize(); i++)
            {
                for (int j = 0; j < myBoard.GetSize(); j++)
                {
                    // if there are spots which are not bombs and not visited, the user has not won
                    if (!myBoard.GetGrid()[i, j].GetVisited() && !myBoard.GetGrid()[i, j].GetLive())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // flood fill
        public void floodFill(int row, int col)
        {
            // different possible combination of surrounding cells
            int[] xMove = { 1, 1, 1, 0, -1, -1, -1, 0 };
            int[] yMove = { 1, 0, -1, -1, -1, 0, 1, 1 };

            // same logic as calculate live neighbors, but condensed
            int next_x, next_y;

            // make sure we have visited the cell
            // if the cell has neighbors, those will already show up on the print now
            myBoard.GetGrid()[row, col].SetVisited(true);

            // if the spot had been flagged, set it to not flagged and decrement the non bombs discovered
            if (myBoard.GetGrid()[row, col].GetFlagged())
            {
                myBoard.GetGrid()[row, col].SetFlagged(false);
                myBoard.SetNonBombsDiscovered(myBoard.GetNonBombsDiscovered() - 1);
            }

            // if it is blank, we want to check its surrounding cells too
            if (myBoard.GetGrid()[row, col].GetNeighbors() == 0)
            {
                // for all surrounding cells
                for (int k = 0; k < 8; k++)
                {
                    next_x = row + xMove[k];
                    next_y = col + yMove[k];
                    // check to make sure it is safe (in the grid and not yet visited)
                    // Console.WriteLine("Checking if (" + next_x  + ", " + next_y + ") is safe.");
                    if (safeSquare(next_x, next_y))
                    {
                        // Console.WriteLine("Safe.");
                        // recursion, call this method on the next new choices
                        floodFill(next_x, next_y);
                    }
                }
            }
        }

        // is this square on the board?
        bool safeSquare(int x, int y)
        {
            // check to see if x, y is on the board. no out of bounds index errors allowed
            // do we care if the cell has already been visited? We do. Stack overflow.
            return (x >= 0 && x < myBoard.GetSize() && y >= 0 && y < myBoard.GetSize() && !myBoard.GetGrid()[x, y].GetVisited());
        }

        public Board GetBoard()
        {
            return myBoard;
        }
        
        // for load, the list sometimes does not match the grid, refresh to make them match
        public void refreshGrid()
        {
            // keep count for the list
            int counter = 0;
            // for easier referencing
            Cell[,] grid = new Cell[myBoard.GetSize(), myBoard.GetSize()];
            List<Cell> cells = myBoard.GetCellList();
            // for every cell in the list, create a new grid 
            for (int i = 0; i < myBoard.GetSize(); i++)
            {
                for (int j = 0; j < myBoard.GetSize(); j++)
                {
                    // assign the cell to the location
                    grid[i, j] = cells.ElementAt(counter);
                    // counter will incremement to match with the list location
                    counter++;
                }
            }

            // replace the board grid
            myBoard.SetGrid(grid);
        }
    }
}
