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
    public static class PlayTurn
    {

        [FunctionName("PlayTurn")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string token = req.Query["token"];
            int hitX = Convert.ToInt32(req.Query["hitX"]);
            int hitY = Convert.ToInt32(req.Query["hitY"]);
            
            if (StartGame.game.p1.token==token)
            {
                if (!StartGame.game.p1.isTurn)
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("It's not your turn!")));
                }
                else
                {
                    StartGame.game.p1.isTurn = false;
                    StartGame.game.p2.isTurn = true;
                    if (StartGame.game.p2.board.hit[hitX, hitY])
                    {
                        return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("You have already launched a missile here!")));
                    }
                    else
                    {
                        StartGame.game.p2.board.hit[hitX, hitY] = true;
                        State s;
                        if (StartGame.game.p2.board.placed[hitX, hitY])
                        {
                            s = new State(StartGame.game, "Hit!");
                            //Testing
                            StartGame.game.p1.isTurn = true;
                            StartGame.game.p2.isTurn = false;
                            //
                            StartGame.game.p2.livesLeft -= 1;
                            if (StartGame.game.p2.livesLeft==0)
                            {
                                s.message += " " + StartGame.game.p1.name + " has won the game!";
                                StartGame.game.p1.isTurn = false;
                                StartGame.game.p2.isTurn = false;
                            }
                        }
                        else
                        {
                            s = new State(StartGame.game, "Miss!");
                        }
                        return new OkObjectResult(JsonConvert.SerializeObject(s));
                    }
                }
            }
            else if (StartGame.game.p2.token==token)
            {
                if (StartGame.game.p1.isTurn)
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("It's not your turn!")));
                }
                else
                {
                    StartGame.game.p2.isTurn = false;
                    StartGame.game.p1.isTurn = true;
                    if (StartGame.game.p1.board.hit[hitX, hitY])
                    {
                        return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("You have already launched a missile here!")));
                    }
                    else
                    {
                        StartGame.game.p1.board.hit[hitX, hitY] = true;
                        State s;
                        if (StartGame.game.p1.board.placed[hitX, hitY])
                        {
                            s = new State(StartGame.game, "Hit!");
                            //Testing
                            StartGame.game.p1.isTurn = false;
                            StartGame.game.p2.isTurn = true;
                            //
                            StartGame.game.p1.livesLeft -= 1;
                            if (StartGame.game.p1.livesLeft == 0)
                            {
                                s.message += " " + StartGame.game.p2.name + " has won the game!";
                                StartGame.game.p1.isTurn = false;
                                StartGame.game.p2.isTurn = false;
                            }
                        }
                        else
                        {
                            s = new State(StartGame.game, "Miss!");
                        }
                        return new OkObjectResult(JsonConvert.SerializeObject(s));
                    }
                }
            }
            else
            {
                return new BadRequestObjectResult(JsonConvert.SerializeObject(new Error("Invalid token!")));
            }
        }
    }
}
