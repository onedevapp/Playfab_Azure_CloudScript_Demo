using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine.UI;

public class DemoScript : MonoBehaviour
{
    public PlayerInfo CurrentPlayer { get; private set; }
    public Button onUpdateLeaderBoardBtn;

    private void Start()
    {
        onUpdateLeaderBoardBtn.interactable = false;

        var loginHandler = new LoginHandler();
        loginHandler.Login(OnPlayerLogin, OnLoginFail);

        onUpdateLeaderBoardBtn.onClick.AddListener(() =>
        {
            CallExecuteFunction(Constants.UPDATELEADERBOARD_FUNCTION_NAME, new Dictionary<string, object>());
        });
    }

    private void UpdateGameStatus(string statusText)
    {
        Debug.Log(statusText);
    }

    private void OnLoginFail()
    {
        UpdateGameStatus("Login failed");
    }

    private void OnPlayerLogin(PlayerInfo playerInfo)
    {
        UpdateGameStatus("Login successful");

        // Store the player info
        CurrentPlayer = playerInfo;

        onUpdateLeaderBoardBtn.interactable = true;

        onUpdateLeaderBoardBtn.onClick.AddListener(() =>
        {
            var fParams = new Dictionary<string, object>();
            fParams.Add("inputValue", "Test");
            CallExecuteFunction(Constants.HELLOWORLD_FUNCTION_NAME, fParams);
        });
    }

    private void CallExecuteFunction(string functionName, Dictionary<string, object> fParams)
    {
        // Create the reset request
        var request = new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId, //Get this from when you logged in,
                Type = PlayFabSettings.staticPlayer.EntityType, //Get this from when you logged in
            },
            FunctionName = functionName, //This should be the name of your Azure Function that you created.
            FunctionParameter = fParams, //This is the data that you would want to pass into your function.
            GeneratePlayStreamEvent = false, //Set this to true if you would like this call to show up in PlayStream

            AuthenticationContext = new PlayFabAuthenticationContext
            {
                EntityToken = PlayFabSettings.staticPlayer.EntityToken
            }
        };

        Debug.Log("request::" + request.ToJson());
        PlayFabCloudScriptAPI.ExecuteFunction(request, (ExecuteFunctionResult result) =>
        {
            if (result.FunctionResultTooLarge ?? false)
            {
                Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                return;
            }
            Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
            Debug.Log($"Result: {result.FunctionResult.ToString()}");
        }, (PlayFabError error) =>
        {
            Debug.Log($"Opps Something went wrong: {error.GenerateErrorReport()}");
        });
    }
}
