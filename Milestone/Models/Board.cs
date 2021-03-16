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
 * Board.cs
 * 
 * This class creates the board for the minesweeper game. The properties are defined with getters and setters and
 * methods to setup the bombs and calculate the live neigbors are created. Methods to check for a win
 * and flood fill with safe squares are also present. 
 * 
 * This is my own work, as influenced by class time and video tutorials for previous projects.
 */

namespace Milestone.Models
{
    public class Board
    {
        // the size of the board
        private int Size;

        // difficulty will be an integer between 0 and 100, which will determine the percentage of bombs
        private int Difficulty;

        // 2d array of type cell
        private Cell[,] theGrid;

        // need a list for display on the webpage
        private List<Cell> cellList;

        // properties to keep track
        private int numberOfBombs;
        private int bombsDiscovered = 0;
        private int nonBombsDiscovered = 0;

        // constructor which takes the size and difficulty of the board

        public Board(int size, int difficulty)
        {
            // initial size of the board is defined by s 
            Size = size;
            Difficulty = difficulty;

            // create a new 2D array of type cell
            theGrid = new Cell[Size, Size];

            // create new list
            cellList = new List<Cell>();

            // fill the 2D array with new Cells, each with unique x and y coordiates
            // will set the ID's for each cell from 0 to 99, should correspond to index in list
            int k = 0;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    theGrid[i, j] = new Cell(i, j);
                    theGrid[i, j].SetID(k);
                    cellList.Add(theGrid[i, j]);
                    k++;
                }
            }
        }
        // getters and setters for all properties
        public int GetSize()
        {
            return Size;
        }
        public void SetSize(int value)
        {
            Size = value;
        }
        public int GetDifficulty()
        {
            return Difficulty;
        }
        public void SetDifficulty(int value)
        {
            Difficulty = value;
        }
        public Cell[,] GetGrid()
        {
            return theGrid;
        }
        public void SetGrid(Cell[,] value)
        {
            theGrid = value;
        }
        public int GetNumberOfBombs()
        {
            return numberOfBombs;
        }
        public void SetNumberOfBombs(int value)
        {
            numberOfBombs = value;
        }
        public int GetBombsDiscovered()
        {
            return bombsDiscovered;
        }
        public void SetBombsDiscovered(int value)
        {
            bombsDiscovered = value;
        }
        public int GetNonBombsDiscovered()
        {
            return nonBombsDiscovered;
        }
        public void SetNonBombsDiscovered(int value)
        {
            nonBombsDiscovered = value;
        }
        public List<Cell> GetCellList()
        {
            return cellList;
        }
        public void SetCellList(List<Cell> value)
        {
            cellList = value;
        }
    }
}
