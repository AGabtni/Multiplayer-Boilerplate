using UnityEngine;
using System;
using PlayFab;
using PlayFab.ClientModels;

public class LoginHandler
{

    void Start()
    {
    }

    public void Login(Action<PlayerInfo> OnSuccess, Action OnError, String playerName)
    {

        //Create login request with custom player id
        var request = new LoginWithCustomIDRequest
        {
            CustomId = GetPlayerCustomId(),
            CreateAccount = true
        };

        //Send Login request
        PlayFabClientAPI.LoginWithCustomID(request,
            //Create 
            (result) =>
            {

                //Fill new player model instance
                PlayerInfo currentPlayer = new PlayerInfo
                {
                    EntityToken = result.EntityToken.EntityToken,
                    PlayFabId = result.PlayFabId,
                    PlayerName = playerName,
                    SessionTicket = result.SessionTicket,
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
        if (!PlayerPrefs.HasKey("PLAYER_CUSTOM_ID"))
        {
            var newId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            PlayerPrefs.SetString("PLAYER_CUSTOM_ID", newId);
        }
        return PlayerPrefs.GetString("PLAYER_CUSTOM_ID");
    }
}


        

    



