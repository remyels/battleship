using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BattleshipApp
{
    public static class ConnectPlayer
    {
        public static bool p1connected;
        public static bool p2connected;

        [FunctionName("ConnectPlayer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string token = req.Query["token"];
            string name = req.Query["name"];

            if (StartGame.game==null)
            {
                return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("Game hasn't been initialized yet!")));
            }

            if (p1connected&&p2connected)
            {
                return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("Both players are already connected!")));
            }

            if (string.Compare(StartGame.game.p1.token, token)==0)
            {
                if (p1connected)
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("This player is already connected!")));
                }
                else
                {
                    p1connected = true;
                    StartGame.game.p1.isConnected = true;
                    StartGame.game.p1.name = name;
                    StartGame.game.p1.currentShipTBP = SetShip.ships[0];
                    StartGame.game.p1.currentShipTBPindex = 0;
                    SetShip.player1 = StartGame.game.p1;
                    return new OkObjectResult("Successfully connected! " + (p2connected ? "" : "Awaiting opponent's connection..."));
                }
            }
            else if (string.Compare(StartGame.game.p2.token,token)==0)
            {
                if (p2connected)
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("This player is already connected!")));
                }
                else
                {
                    p2connected = true;
                    StartGame.game.p2.isConnected = true;
                    StartGame.game.p2.name = name;
                    StartGame.game.p2.currentShipTBP = SetShip.ships[0];
                    StartGame.game.p2.currentShipTBPindex = 0;
                    SetShip.player2 = StartGame.game.p2;
                    return new OkObjectResult("Successfully connected! " + (p1connected ? "" : "Awaiting opponent's connection..."));
                }
            }
            else
            {
                return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("This token doesn't match any player!")));
            }
        }
    }
}
