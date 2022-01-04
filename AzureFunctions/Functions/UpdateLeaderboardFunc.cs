using System;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ServerModels;
using PlayFab.Samples;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SwipeWire.PlayfabCloudScript.Util;

namespace SwipeWire.PlayfabCloudScript
{
    public static class UpdateLeaderboardFunc
    {

        [FunctionName("UpdateLeaderboard")]
        public static async Task UpdateLeaderboard(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());

            dynamic args = context.FunctionArgument;
            
            string playFabId = context.FunctionArgument.PlayFabId;

            var TitleId = Environment.GetEnvironmentVariable(Constants.PLAYFAB_TITLE_ID, EnvironmentVariableTarget.Process);
            var cloudName = Environment.GetEnvironmentVariable(Constants.PLAYFAB_CLOUD_NAME, EnvironmentVariableTarget.Process);
            var secretKey = Environment.GetEnvironmentVariable(Constants.PLAYFAB_DEV_SECRET_KEY, EnvironmentVariableTarget.Process);

           
            var settings = new PlayFabApiSettings
            {
                TitleId = TitleId,
            };
            
            if (!string.IsNullOrEmpty(TitleId)) 
                settings.TitleId = TitleId; 
            else 
                settings.TitleId = context.TitleAuthenticationContext.Id;
            
            if (!string.IsNullOrEmpty(secretKey)) settings.DeveloperSecretKey = secretKey;                
            if (!string.IsNullOrEmpty(cloudName)) settings.VerticalName = cloudName;

            var authContext = new PlayFabAuthenticationContext
            {
                EntityToken = context.TitleAuthenticationContext.EntityToken
            };

            //var serverApi = new PlayFabServerInstanceAPI(settings, authContext);
                           
            string statName = "";
            if (context.FunctionArgument.ContainsKey("statName"))
                statName = context.FunctionArgument.statName;

            if(string.IsNullOrEmpty(statName)) return;
                
            string gameResult = "";
            if (context.FunctionArgument.ContainsKey("gameResult"))
                gameResult = context.FunctionArgument.gameResult;

            var _statsRequest = new List<string>(){ "statName" };
            var result = await ServerApiData.GetPlayerStatisticsForNames(playFabId, _statsRequest, settings, authContext);
            
            // Apply the delta on the stat
            int oldValue = 0;
            // Try catch in case the stat was not found then assign it to the player
            try
            {
                oldValue = result[0].Value;
            }
            catch (InvalidOperationException) {} // Do not handle stat not found for player, simply create it with update.

            var newValue = oldValue + (gameResult == "Won" ? 3 : 1);

             // Update the player's stat with the new value
            var _updatedStats = new List<StatisticUpdate>
                {
                    new StatisticUpdate{
                        StatisticName =  statName,
                        Value =  newValue
                    }
                };
            
            await ServerApiData.UpdatePlayerStatisticsForNames(playFabId, _updatedStats, settings, authContext);
        }

        
    }
}