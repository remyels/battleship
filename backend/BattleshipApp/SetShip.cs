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
    public static class SetShip
    {
        public static Player player1;
        public static int player1index;
        public static Player player2;
        public static int player2index;

        public static string[] ships;

        [FunctionName("SetShip")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            int startingX = Convert.ToInt32(req.Query["startingX"]);
            int startingY = Convert.ToInt32(req.Query["startingY"]);
            int endingX = Convert.ToInt32(req.Query["endingX"]);
            int endingY = Convert.ToInt32(req.Query["endingY"]);
            string shiptype = req.Query["type"];
            string token = req.Query["token"];

            if (player1 == null || player2 == null)
            {
                return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("One or both players aren't connected yet!")));
            }

            if (!Ship.validForType(shiptype, startingX, startingY, endingX, endingY))
            {
                return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("Selection does not match ship size")));
            }


            if (!Ship.inBounds(startingX, startingY, endingX, endingY))
            {
                return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("Coordinates out of grid bounds!")));
            }

            if (string.Compare(StartGame.game.p1.token,token)==0)
            {
                if (StartGame.game.p1.doneplacement)
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("Player is done placing their ships!")));
                }
                else if (!StartGame.game.p1.board.checkVacant(startingX, startingY, endingX, endingY))
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("Selection overlaps with another ship")));
                }
                else if (StartGame.game.p1.board.checkFull(shiptype))
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("The maximum number of this ship was already placed!")));
                }
                else if (string.Compare(ships[player1index], shiptype)!=0)
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("Wrong ship type!")));
                }
                else
                { 
                    State s = new State(StartGame.game, "Ship successfully placed");
                    StartGame.game.p1.board.addShip(new Ship(shiptype, startingX, startingY, endingX, endingY));
                    if (StartGame.game.p1.board.checkFull(shiptype))
                    {
                        if (player1index<ships.Length-1)
                        {
                            player1index++;
                            StartGame.game.p1.currentShipTBPindex++;
                            StartGame.game.p1.currentShipTBP = ships[StartGame.game.p1.currentShipTBPindex]; 
                        }
                        else
                        {
                            StartGame.game.p1.doneplacement = true;
                            if (StartGame.game.p2.doneplacement)
                            {
                                StartGame.game.p1.isTurn = true;
                            }
                        }
                    }
                    return new OkObjectResult(JsonConvert.SerializeObject(s));
                }
            }
            else if (string.Compare(StartGame.game.p2.token, token)==0)
            {
                if (StartGame.game.p2.doneplacement)
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("Player is done placing their ships!")));
                }
                else if (!StartGame.game.p2.board.checkVacant(startingX, startingY, endingX, endingY))
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("Selection overlaps with another ship")));
                }
                else if (StartGame.game.p2.board.checkFull(shiptype))
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("The maximum number of this ship was already placed!")));
                }
                else if (string.Compare(ships[player2index], shiptype) != 0)
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("Wrong ship type!")));
                }
                else
                {
                    State s = new State(StartGame.game, "Ship successfully placed");
                    StartGame.game.p2.board.addShip(new Ship(shiptype, startingX, startingY, endingX, endingY));
                    if (StartGame.game.p2.board.checkFull(shiptype))
                    {
                        if (player2index < ships.Length - 1)
                        {
                            player2index++;
                            StartGame.game.p2.currentShipTBPindex++;
                            StartGame.game.p2.currentShipTBP = ships[StartGame.game.p2.currentShipTBPindex];
                        }
                        else
                        { 
                            StartGame.game.p2.doneplacement = true;
                            if (StartGame.game.p1.doneplacement)
                            {
                                StartGame.game.p1.isTurn = true;
                            }
                        }
                    }
                    return new OkObjectResult(JsonConvert.SerializeObject(s));
                }
            }
            else
            {
                return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("Invalid token!")));
            }

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //return name != null
            //    ? (ActionResult)new OkObjectResult($"Hello, {name}")
            //    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
