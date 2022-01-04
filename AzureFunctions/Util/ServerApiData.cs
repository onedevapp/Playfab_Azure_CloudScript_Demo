using PlayFab;
using PlayFab.ServerModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwipeWire.PlayfabCloudScript.Util
{
    public class ServerApiData
    {

        /*
         *  For more API details refer
         *  https://github.com/PlayFab/CSharpSDK/blob/master/PlayFabSDK/source/PlayFabServerInstanceAPI.cs
         *  https://github.com/PlayFab/CSharpSDK/blob/master/PlayFabSDK/source/PlayFabServerModels.cs
        */

        public static async Task<Dictionary<string, string>> GetTitleDataForKeys(List<string> keys, PlayFabApiSettings apiSettings, PlayFabAuthenticationContext authenticationContext)
        {
            // Instantiate a server api client using the settings
            var serverApi = new PlayFabServerInstanceAPI(apiSettings, authenticationContext);

            GetTitleDataRequest request = new GetTitleDataRequest()
            {
                Keys = keys
            };

            var result = await serverApi.GetTitleDataAsync(request);

            if (result.Error != null)
            {
                //throw new Exception($"An error occurred while fetching the title data: Error: {result.Error.GenerateErrorReport()}");

                return new Dictionary<string, string>(){};
            }

            return result.Result.Data;            
        }

        public static async Task SetUserDataForKeys(string playFabId,  Dictionary<string, string> newKeys, UserDataPermission keyPermission, PlayFabApiSettings apiSettings, PlayFabAuthenticationContext authenticationContext)
        {            
            var request = new UpdateUserDataRequest()
            {
                PlayFabId = playFabId,
                Data = newKeys,
                Permission = keyPermission
            };

            var serverApi = new PlayFabServerInstanceAPI(apiSettings, authenticationContext);

            var result = await serverApi.UpdateUserDataAsync(request);

            if (result.Error != null)
            {
                throw new Exception($"An error occured while add new user data: {result.Error.GenerateErrorReport()}");
            }
        }

        public static async Task<Dictionary<string, PlayFab.ServerModels.UserDataRecord>> GetUserDataForKeys(string playFabId, List<string> keys, PlayFabApiSettings apiSettings, PlayFabAuthenticationContext authenticationContext)
        {            
            
            var request = new GetUserDataRequest()
            {
                PlayFabId = playFabId,
                Keys = keys
            };

            var serverApi = new PlayFabServerInstanceAPI(apiSettings, authenticationContext);

            var result = await serverApi.GetUserDataAsync(request);

            if (result.Error != null)
            {
                //throw new Exception($"An error occured while add new user data: {result.Error.GenerateErrorReport()}");

                return new Dictionary<string, PlayFab.ServerModels.UserDataRecord>(){};
            }

            return result.Result.Data;
        }
        
        public static async Task SetUserReadOnlyDataForKeys(string playFabId,  Dictionary<string, string> newKeys, PlayFabApiSettings apiSettings, PlayFabAuthenticationContext authenticationContext)
        {            
            var request = new UpdateUserDataRequest()
            {
                PlayFabId = playFabId,
                Data = newKeys
            };

            var serverApi = new PlayFabServerInstanceAPI(apiSettings, authenticationContext);

            var result = await serverApi.UpdateUserReadOnlyDataAsync(request);

            if (result.Error != null)
            {
                throw new Exception($"An error occured while add new user data: {result.Error.GenerateErrorReport()}");
            }
        }

        public static async Task<Dictionary<string, PlayFab.ServerModels.UserDataRecord>> GetUserReadOnlyDataForKeys(string playFabId, List<string> keys, PlayFabApiSettings apiSettings, PlayFabAuthenticationContext authenticationContext)
        {            
            
            var request = new GetUserDataRequest()
            {
                PlayFabId = playFabId,
                Keys = keys
            };

            var serverApi = new PlayFabServerInstanceAPI(apiSettings, authenticationContext);

            var result = await serverApi.GetUserReadOnlyDataAsync(request);

            if (result.Error != null)
            {
                //throw new Exception($"An error occured while add new user data: {result.Error.GenerateErrorReport()}");

                return new Dictionary<string, PlayFab.ServerModels.UserDataRecord>(){};
            }

            return result.Result.Data;
        }
        
        public static async Task<List<StatisticValue>> GetPlayerStatisticsForNames(string playFabId,  List<string> statisticNames, PlayFabApiSettings apiSettings, PlayFabAuthenticationContext authenticationContext)
        {            
            var request = new GetPlayerStatisticsRequest
            {
                PlayFabId = playFabId,
                StatisticNames = statisticNames
            };

            var serverApi = new PlayFabServerInstanceAPI(apiSettings, authenticationContext);

            var result = await serverApi.GetPlayerStatisticsAsync(request);

            if (result.Error != null)
            {
                //throw new Exception($"An error occured while add new user data: {result.Error.GenerateErrorReport()}");
                return new List<StatisticValue>(){};
            }

            return result.Result.Statistics;
        }

        public static async Task UpdatePlayerStatisticsForNames(string playFabId,  List<StatisticUpdate> statisticValues, PlayFabApiSettings apiSettings, PlayFabAuthenticationContext authenticationContext)
        {            
            var request = new UpdatePlayerStatisticsRequest
            {
                PlayFabId = playFabId,
                Statistics = statisticValues
            };

            var serverApi = new PlayFabServerInstanceAPI(apiSettings, authenticationContext);

            var result = await serverApi.UpdatePlayerStatisticsAsync(request);

            if (result.Error != null)
            {
                throw new Exception($"An error occured while add new user data: {result.Error.GenerateErrorReport()}");
            }
        }

        public static async Task AddUserVC(string playFabId, string virtualCurrency, int amount, PlayFabApiSettings apiSettings, PlayFabAuthenticationContext authenticationContext)
        {
            var request = new AddUserVirtualCurrencyRequest 
            {
                PlayFabId  = playFabId,
                VirtualCurrency = virtualCurrency,
                Amount = amount
            };
            
            var serverApi = new PlayFabServerInstanceAPI(apiSettings, authenticationContext);

            var result = await serverApi.AddUserVirtualCurrencyAsync(request);

            if (result.Error != null)
            {
                throw new Exception($"An error occured while add virtual currency to user: {result.Error.GenerateErrorReport()}");
            }
        }

        public static async Task SubUserVC(string playFabId, string virtualCurrency, int amount, PlayFabApiSettings apiSettings, PlayFabAuthenticationContext authenticationContext)
        {
            var request = new SubtractUserVirtualCurrencyRequest 
            {
                PlayFabId  = playFabId,
                VirtualCurrency = virtualCurrency,
                Amount = amount
            };
            
            var serverApi = new PlayFabServerInstanceAPI(apiSettings, authenticationContext);

            var result = await serverApi.SubtractUserVirtualCurrencyAsync(request);
            
            if (result.Error != null)
            {
                throw new Exception($"An error occured while add virtual currency to user: {result.Error.GenerateErrorReport()}");
            }
        }
    }

}