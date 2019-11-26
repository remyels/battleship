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
    public static class GetState
    {
        [FunctionName("GetState")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (StartGame.game == null) {
                return new BadRequestObjectResult("Game hasn't been initialized!");
            }
            return new OkObjectResult(JsonConvert.SerializeObject(StartGame.game));
        }
    }
}
