using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Milestone.Models;
using Milestone.Utility;
using Milestone.Views.Services.Business;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

/**
 * Kacey Morris
 * Alex Vergara
 * March 28 2021
 * CST 247
 * Milestone 4 - Save, Load, and API
 * CellController.cs
 * 
 * This class controls all background for the game play. It sets up the game and returns views. 
 * 
 * This is our own work, as influenced by class time and video tutorials for previous projects.
 */

namespace Milestone.Controllers
{
    public class CellController : Controller
    {
        // reference to the class Board. Contains the values of the board.
        static Board myBoard;
        static GameBusinessService bs;

        // booleans to allow for the game to continue and for a win
        string winIndicator = "tbd";

        // to keep track of the user id
        int userID = -1;

        public IActionResult Index(string level)
        {
            // create the board passing the level 
            createBoard(level);

            // set up the bombs
            bs.setupLiveNeighbors();
            // calculate all live neighbors
            bs.calculateLiveNeighbors();
            // set the number of bombs to what was passed through
            bs.GetBoard().SetNumberOfBombs(10);

            // save the user ID right away
            try
            {
                userID = (int)HttpContext.Session.GetInt32("userID");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error - session expired");
                userID = -1;
            }

            // pass list of cells to view
            return View("Index", bs.GetBoard().GetCellList());
        }

        public IActionResult Index2()
        {
            GamesDAO gamesDAO = new GamesDAO();

            GameObject gameObject = gamesDAO.loadGame();
            List<Cell> cellList = JsonConvert.DeserializeObject<List<Cell>>(gameObject.JsonString);

            // make the boards cell list the cell list that was retrieved
            bs.GetBoard().SetCellList(cellList);
            // make sure the grid matches the cell list passed so flood fill and win works
            bs.refreshGrid();

            // pass list of cells to view
            return View("Index", cellList);
        }

        public IActionResult HandleButtonClick(int cellNum)
        {
            Cell currentCell = bs.GetBoard().GetCellList().ElementAt(cellNum);
            var buttonHTMLString = "";

            // only allow left click for non flagged cells
            if (!currentCell.GetFlagged())
            {
                // visit cell
                currentCell.SetVisited(true);

                // call flood fill on that cell spot
                // ONLY if there are no neighbors
                if (currentCell.GetNeighbors() == 0)
                {
                    // do the flood fill
                    bs.floodFill(currentCell.GetRowNumber(), currentCell.GetColumnNumber()); 
                    
                }

                // if they selected a bomb, end the game and let them know
                if (currentCell.GetLive())
                {
                    // set win to false, they lost
                    winIndicator = "false";
                }
                // if they cleared all non bomb spots, end the game and display win
                else if (bs.checkForWin())
                {
                    // set win to true, they won
                    winIndicator = "true";
                }
            }

            // if they have not won or loss yet, send an update for the grid without displaying all values
            if (winIndicator == "tbd")
            {
                // get the html string for entire grid
                buttonHTMLString = RenderRazorViewToString(this, "ShowGrid", bs.GetBoard().GetCellList());
            }
            else
            {
                // get the html string for entire grid showing all bombs and squares
                buttonHTMLString = RenderRazorViewToString(this, "ShowGridFinal", bs.GetBoard().GetCellList());
            }
            // redisplay the button grid
            return Json(new { part1 = buttonHTMLString, part2 = winIndicator });
        }

        // action for right button click 
        public IActionResult onRightButtonClick(int cellNum)
        {
            // int cellNum = buttonNumber;
            Cell currentCell = bs.GetBoard().GetCellList().ElementAt(cellNum);

            // only change the flag if the cell is not visited
            // we don't want them to not be able to win because they have a flag that doesn't show
            if (!currentCell.GetVisited())
            {
                // set the flagged state to the opposite of what it was before
                currentCell.SetFlagged(!currentCell.GetFlagged());
            }
            
            // get the html string for one button
            var buttonHTMLString = RenderRazorViewToString(this, "ShowOneButton", currentCell);

            // we want to return just the cell that was altered as a partial page update
            return Json(new { part1 = buttonHTMLString, part2 = winIndicator });
        }

        // from the link in the activity instructions
        public static string RenderRazorViewToString(Controller controller, string viewName, object model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine viewEngine =
                    controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as
                        ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult onSave()
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            GamesDAO gamesDAO = new GamesDAO();

            // get list
            List<Cell> cellList = bs.GetBoard().GetCellList();

            string jsonData = JsonConvert.SerializeObject(cellList);
            // string jsonData = JsonConvert.SerializeObject(new { jsonData = cellList });

            int getUserID = (int)HttpContext.Session.GetInt32("userID");
            GameObject gameObject = new GameObject(0, jsonData, getUserID, DateTime.Now);

            bool success = gamesDAO.saveGame(gameObject);

            Tuple<bool, string> resultsTuple = new Tuple<bool, string>(success, jsonData);

            return View("Index", cellList);

            //return View("Results", resultsTuple);
        }

        public ActionResult onLoad()
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            GamesDAO gamesDAO = new GamesDAO();
            // get cell List
            List<Cell> cellList = bs.GetBoard().GetCellList();
            // serialize cells List into JSON
            // return game object
            GameObject gameObject = gamesDAO.loadGame();
            //
            cellList = JsonConvert.DeserializeObject<List<Cell>>(gameObject.JsonString);

            // tuple to send multiple parts of data
            Tuple<bool, string> resultsTuple = new Tuple<bool, string>(true, JsonConvert.SerializeObject(cellList));

            return View("Results", resultsTuple);

        }

        public IActionResult onDelete(string gameIDString)
        {
            // delete the game
            GamesDAO dao = new GamesDAO();
            int gameID = Int32.Parse(gameIDString);
            bool success = dao.deleteGameByID(gameID);

            try
            {
                userID = (int)HttpContext.Session.GetInt32("userID");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error - session expired");
                userID = -1;
            }

            // update the list and return the view again
            List<GameObject> games = dao.getGameByUser(userID);
            return View("SavedGames", games);
        }

        public IActionResult ApiDeleteSavedGame(int id)
        {
            GamesDAO dao = new GamesDAO();
            bool success = dao.deleteGameByID(id);
            try
            {
                userID = (int)HttpContext.Session.GetInt32("userID");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error - session expired");
                userID = -1;
            }
            List<GameObject> games = dao.getGameByUser(userID);
            return View("SavedGames", games);
        }


        public IActionResult SavedGames()
        {
            // get the games for this user
            GamesDAO dao = new GamesDAO();
            // if we havent saved the ID yet
            try
            {
                userID = (int)HttpContext.Session.GetInt32("userID");
            } catch (Exception e)
            {
                Console.WriteLine("Error - session expired");
                userID = -1;
            }
            List<GameObject> games = dao.getGameByUser(userID);
            return View("SavedGames", games);
        }


        /// <summary>
        /// API Shows all Games in JSON format
        /// Changes were made to Startup.cs File around line 73
        /// Follow Route https://localhost:5001/cell/ApiShowAllSavedGames
        /// </summary>
        /// <returns></returns>        
        public string ApiShowAllSavedGames()
        {
            // Instantiate DAO
            GamesDAO gamesDAO = new GamesDAO();
            // get all games
            List<GameObject> games = gamesDAO.getAllGames();
            // convert all game objects into JSON
            string jsonData = JsonConvert.SerializeObject(games);
            // Return JSON Data
            return jsonData;

        }

        public IActionResult onContinue(string gameIDString)
        {
            string level = "easy";
            int gameID = Int32.Parse(gameIDString);
            GamesDAO gamesDAO = new GamesDAO();

            GameObject gameObject = gamesDAO.loadGameByID(gameID);
            List<Cell> cellList = JsonConvert.DeserializeObject<List<Cell>>(gameObject.JsonString);

            switch (Math.Sqrt(cellList.Count))
            {
                case 10:
                    level = "easy";
                    break;
                case 12:
                    level = "medium";
                    break;
                case 15:
                    level = "hard";
                    break;
                case 18:
                    level = "expert";
                    break;
            }
            createBoard(level);

            // make the boards cell list the cell list that was retrieved
            bs.GetBoard().SetCellList(cellList);
            // make sure the grid matches the cell list passed so flood fill and win works
            bs.refreshGrid();

            // pass list of cells to view
            return View("Index", cellList);
        }



        /// <summary>
        /// API - shows saved game by id
        /// Route Example: https://localhost:5001/cell/apiShowSavedGame/3
        /// Must be logged in to use
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ApiShowSavedGame(int id)
        {
            // Instantiate DAO
            GamesDAO gamesDAO = new GamesDAO();
            // gets game object by ID
            GameObject gameObject = gamesDAO.loadGameByID(id);
            // converts game object to JSON
            string jsonData = JsonConvert.SerializeObject(gameObject);
            // returns JSON data
            return jsonData;

        }

        public void createBoard(string level)
        {
            int size;
            int difficulty;
            // pass the size and difficulty depending on level to create the board
            switch (level)
            {
                // easy has 10 bombs on a 10 x 10 grid
                case "easy":
                    size = 10;
                    difficulty = 10;
                    break;
                // medium has 12 and 10
                case "medium":
                    size = 12;
                    difficulty = 10;
                    break;
                // hard has 15 and 15
                case "hard":
                    size = 15;
                    difficulty = 15;
                    break;
                // expert has 18 and 18
                case "expert":
                    size = 18;
                    difficulty = 18;
                    break;
                default:
                    size = 5;
                    difficulty = 5;
                    break;
            }
            myBoard = new Board(size, difficulty);
            bs = new GameBusinessService(myBoard);
        }
    }
}
