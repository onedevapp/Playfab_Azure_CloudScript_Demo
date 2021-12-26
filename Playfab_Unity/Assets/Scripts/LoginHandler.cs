using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginHandler
{
    public void Login(Action<PlayerInfo> OnSuccess, Action OnError)
    {
        if (!String.IsNullOrEmpty(Constants.TITLE_ID))
        {
            PlayFabSettings.TitleId = Constants.TITLE_ID;
        }

        var request = new LoginWithCustomIDRequest
        {
            CustomId = GetPlayerCustomId(),
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request,
            (result) =>
            {
                var currentPlayer = new PlayerInfo
                {
                    EntityToken = result.EntityToken.EntityToken,
                    PlayFabId = result.PlayFabId,
                    SessionTicket = result.SessionTicket
                };

                OnSuccess(currentPlayer);
            },
            (error) =>
            {
                    // Provide error feedback to user in case of failure at login
                    Debug.LogError("Could not login to PlayFab for Player.");
                Debug.LogError($"Response code: {error.HttpCode}");
                Debug.LogError($"Error details: {error.ErrorDetails}");
                Debug.LogError($"Error message: {error.ErrorMessage}");

                OnError();
            });
    }

    private string GetPlayerCustomId()
    {
        if (!PlayerPrefs.HasKey(Constants.PLAYFAB_PLAYER_CUSTOM_ID))
        {
            var newId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            PlayerPrefs.SetString(Constants.PLAYFAB_PLAYER_CUSTOM_ID, newId);
        }
        return PlayerPrefs.GetString(Constants.PLAYFAB_PLAYER_CUSTOM_ID);
    }
}
