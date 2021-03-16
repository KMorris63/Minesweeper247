using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Milestone.Models;
using Milestone.Views.Services.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

/**
 * Kacey Morris
 * Alex Vergara
 * March 15 2021
 * CST 247
 * Milestone 3 - Flags and Game Over
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

        public IActionResult Index()
        {
            // pass the size and difficulty depending on level to create the board
            myBoard = new Board(10, 10);
            bs = new GameBusinessService(myBoard);

            // set up the bombs
            bs.setupLiveNeighbors();
            // calculate all live neighbors
            bs.calculateLiveNeighbors();
            // set the number of bombs to what was passed through
            bs.GetBoard().SetNumberOfBombs(10);

            // pass list of cells to view
            return View("Index", bs.GetBoard().GetCellList());
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
    }
}
