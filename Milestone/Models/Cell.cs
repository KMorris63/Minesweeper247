using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

/**
 * Kacey Morris
 * Alex Vergara
 * March 28 2021
 * CST 247
 * Milestone 4 - Save, Load, and API
 * GameObject.cs
 * 
 * This class creates individual cells for the board. It defines the properties of a cell and the different constructors.
 * 
 * This is our own work, as influenced by class time and video tutorials for previous projects.
 */

namespace Milestone.Models
{
    [DataContract]
    public class RootObject
    {
        [DataMember(Name = "cell")]
        public Cell cell { get; set; }
    }

    [DataContract]
    public class Cell
    {
        // NEW
        [DataMember(Name = "id")]
        private int ID = -1;
        // initialize properties with default values
        [DataMember(Name = "RowNumber")]
        private int RowNumber = -1;
        [DataMember(Name = "ColumnNumber")]
        private int ColumnNumber = -1;
        [DataMember(Name = "Visited")]
        private bool Visited = false;
        [DataMember(Name = "Live")]
        private bool Live = false;
        [DataMember(Name = "Neighbors")]
        private int Neighbors = 0;
        [DataMember(Name = "Flagged")]
        private bool Flagged;
        // private string imgName = "";
        // empty constructor
        public Cell()
        {
            // do nothing
        }

        // for when the board is created, the cells only have locations
        public Cell(int row, int col)
        {
            // set the row and column number
            RowNumber = row;
            ColumnNumber = col;
            Visited = false;
            // this.imgName = "";
        }
        // data constructor with all possible parameters
        public Cell(int row, int col, bool visited, bool live, int neighbors)
        {
            // assign passed values to properties
            SetRowNumber(row);
            SetColumnNumber(col);
            SetVisited(visited);
            SetLive(live);
            SetNeighbors(neighbors);
            this.Flagged = false;
            // this.imgName = "";
        }
        /*
        public string getImgName()
        {
            return imgName;
        }

        public void setImgName(string imgName1)
        {
            this.imgName = imgName1;
        }
        */

        // getters and setters for all properties
        public int GetRowNumber()
        {
            return RowNumber;
        }

        public void SetRowNumber(int value)
        {
            RowNumber = value;
        }

        public int GetColumnNumber()
        {
            return ColumnNumber;
        }

        public void SetColumnNumber(int value)
        {
            ColumnNumber = value;
        }

        public bool GetVisited()
        {
            return Visited;
        }

        public void SetVisited(bool value)
        {
            Visited = value;
        }

        public bool GetLive()
        {
            return Live;
        }

        public void SetLive(bool value)
        {
            Live = value;
        }

        public int GetNeighbors()
        {
            return Neighbors;
        }

        public void SetNeighbors(int value)
        {
            Neighbors = value;
        }

        public bool GetFlagged()
        {
            return Flagged;
        }

        public void SetFlagged(bool value)
        {
            Flagged = value;
        }
        public int GetID()
        {
            return ID;
        }

        public void SetID(int value)
        {
            ID = value;
        }

    }
}
