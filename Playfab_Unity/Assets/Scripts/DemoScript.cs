using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.CloudScriptModels;

public class DemoScript : MonoBehaviour
{
    public PlayerInfo CurrentPlayer { get; private set; }
    //This snippet assumes that your game client is already logged into PlayFab.

    private void Start()
    {
        var loginHandler = new LoginHandler();
        loginHandler.Login(OnPlayerLogin, OnLoginFail);
    }

    private void UpdateGameStatus(string statusText)
    {
        //GameStatusText.text = statusText;
        Debug.Log(statusText);
    }

    private void OnLoginFail()
    {
        UpdateGameStatus("Login failed");
        //throw new Exception("Failed to login.");
    }

    private void OnPlayerLogin(PlayerInfo playerInfo)
    {
        UpdateGameStatus("Login successful");

        // Store the player info
        CurrentPlayer = playerInfo;

        // Start the game loop
        //StartCoroutine(StartGameLoop());
        CallCSharpExecuteFunction();
    }

    private void CallCSharpExecuteFunction()
    {
        // Create the reset request
        var request = new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId, //Get this from when you logged in,
                Type = PlayFabSettings.staticPlayer.EntityType, //Get this from when you logged in
            },
            FunctionName = "HelloWorld", //This should be the name of your Azure Function that you created.
            FunctionParameter = new Dictionary<string, object>() { { "inputValue", "Test" } }, //This is the data that you would want to pass into your function.
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
