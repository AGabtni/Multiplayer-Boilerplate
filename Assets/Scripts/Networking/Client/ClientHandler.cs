using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using System;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using Mirror;
public class ClientHandler : MonoBehaviour
{
    [SerializeField] Configuration config;
    [SerializeField] NetworkManager networkManager;
    [SerializeField] TelepathyTransport telepathyTransport;
    [SerializeField] ApathyTransport apathyTransport;

    void Awake()
    {
    }
    public void OnLoginUserButtonClick()
    {




        if (config.gameBuildType == BuildType.REMOTE_CLIENT)
        {

            LoginRemoteUser();

        }
    }


    public void LoginRemoteUser()
    {
        Debug.Log("[ClientStartUp].LoginRemoteUser");

        //We need to login a user to get at PlayFab API's. 
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true,
            CustomId = GUIDUtility.getUniqueID()
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnPlayFabLoginSuccess, OnLoginError);
    }

    #region User Login Handlers
    private void OnLoginError(PlayFabError response)
    {
        Debug.Log(response.ToString());
    }

    private void OnPlayFabLoginSuccess(LoginResult response)
    {
        Debug.Log(response.ToString());
        if (config.serverIpAdress == "")
        {   //We need to grab an IP and Port from a server based on the buildId. Copy this and add it to your Configuration.
            RequestMultiplayerServer();
        }
        else
        {
            ConnectRemoteClient();
        }
    }

    private void RequestMultiplayerServer()
    {
        Debug.Log("[ClientStartUp].RequestMultiplayerServer");
        RequestMultiplayerServerRequest requestData = new RequestMultiplayerServerRequest();
        requestData.BuildId = config.playfabBuildID;
        requestData.SessionId = System.Guid.NewGuid().ToString();
        requestData.PreferredRegions = new List<string>() { "EastUs" };
        PlayFabMultiplayerAPI.RequestMultiplayerServer(requestData, OnRequestMultiplayerServer, OnRequestMultiplayerServerError);
    }

    private void OnRequestMultiplayerServer(RequestMultiplayerServerResponse response)
    {
        Debug.Log(response.ToString());
        ConnectRemoteClient(response);
    }

    private void ConnectRemoteClient(RequestMultiplayerServerResponse response = null)
    {
        if (response == null)
        {
            networkManager.networkAddress = config.serverIpAdress;
            telepathyTransport.port = config.portNumber;
            apathyTransport.port = config.portNumber;
        }
        else
        {
            Debug.Log("**** ADD THIS TO YOUR CONFIGURATION **** -- IP: " + response.IPV4Address + " Port: " + (ushort)response.Ports[0].Num);
            networkManager.networkAddress = response.IPV4Address;
            telepathyTransport.port = (ushort)response.Ports[0].Num;
            apathyTransport.port = (ushort)response.Ports[0].Num;
        }

        networkManager.StartClient();
    }

    private void OnRequestMultiplayerServerError(PlayFabError error)
    {
        Debug.Log(error.ErrorDetails);
    }
    #endregion

}