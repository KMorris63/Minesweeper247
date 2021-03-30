using System;
using System.Runtime.Serialization;

/**
 * Kacey Morris
 * Alex Vergara
 * March 28 2021
 * CST 247
 * Milestone 4 - Save, Load, and API
 * GameObject.cs
 * 
 * This class represents a game object to be saved in the database.
 * 
 * This is our own work, as influenced by class time and video tutorials for previous projects.
 */

namespace Milestone.Utility
{
    public class GameObject
    {
        [DataMember(Name = "id")]
        public int id { get; set; }
        [DataMember(Name = "JsonString")]
        public string JsonString { get; set; }
        [DataMember(Name = "userID")]
        public int userID { get; set; }
        [DataMember(Name = "datePlayed")]
        public DateTime datePlayed { get; set; }
        [DataMember(Name = "level")]
        public string level { get; set; }
        public GameObject(int ID, string JSONString, int UserID, DateTime DatePlayed, string Level)
        {
            this.id = ID;
            this.JsonString = JSONString;
            this.userID = UserID;
            this.datePlayed = DatePlayed;
            this.level = Level;
        }
        public GameObject()
        {

        }
    }
}