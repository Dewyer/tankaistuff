using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TankServers.Models;
using TankServers.Services;

namespace TankServers.Controllers
{
    public class HomeController : Controller
    {
        private IGameServer gameServer;

        public HomeController(IGameServer gameServer)
        {
            this.gameServer = gameServer;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult OpenRoom(string name)
        {
            return new JsonResult(gameServer.OpenNewRoom(name));
        }

        [HttpGet]
        public IActionResult ConnectToRoom(string name)
        {
            return new JsonResult(gameServer.ConnectToRoom(name));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
