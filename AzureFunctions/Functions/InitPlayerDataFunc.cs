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
    public static class InitPlayerDataFunc
    {
        [FunctionName("InitPlayerData")]
        public static async Task Run(
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

            string signUpBonusVal = "0";

            // Grab the Title data
            List<string> Keys = new List<string> {"SignUpBonus"};
            var gameTitleDatas = await ServerApiData.GetTitleDataForKeys(Keys, settings, authContext);
            signUpBonusVal = gameTitleDatas["SignUpBonus"];

            // update the user data public           
            Dictionary<string, string> dataToPlayFab = new Dictionary<string, string>();
            dataToPlayFab.Add("PlayerLevel", "1");
            await ServerApiData.SetUserDataForKeys(playFabId, dataToPlayFab, UserDataPermission.Public, settings, authContext);

            // update the user data read only
            int signUpBonus = 0;
            int.TryParse(signUpBonusVal, out signUpBonus);
            await ServerApiData.AddUserVC(playFabId, "SL", signUpBonus, settings, authContext);

        }
    }
}