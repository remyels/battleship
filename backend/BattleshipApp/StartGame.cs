using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;

namespace BattleshipApp
{
    public static class StartGame
    {

        public static bool gameStarted = false;
        public static Game game;
        public static Random random = new Random();

        [FunctionName("StartGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (game!=null)
            {
                return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("The game was already initialized!")));
            }

            game = new Game();
            game.p1.token = generateToken(6);
            game.p2.token = generateToken(6);

            gameStarted = true;

            //Populate ship types
            SetShip.ships = Board.getShips();
            SetShip.player1index = 0;
            SetShip.player2index = 0;

            return new OkObjectResult(JsonConvert.SerializeObject(game));
            //return new OkObjectResult("Testing");
        }

        public static string generateToken(int digits)
        {
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }
    }
}
