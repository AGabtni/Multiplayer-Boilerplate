
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using PlayFab;
using PlayFab.MultiplayerAgent.Model;
using Mirror;

public class ServerHandler : MonoBehaviour
{

    [SerializeField] Configuration config;
    [SerializeField] NetworkManager networkManager;

    private List<ConnectedPlayer> _connectedPlayers;


   //public UnityNetworkServer UNetServer;

    void Start()
    {

        if (config.gameBuildType == BuildType.REMOTE_SERVER)
            StartPlayfabServer();
    }

    void StartPlayfabServer()
    {


        Debug.Log("[ServerStartUp].StartRemoteServer");
        _connectedPlayers = new List<ConnectedPlayer>();

        //Playfab multiplayer handlers
        PlayFabMultiplayerAgentAPI.Start();
        PlayFabMultiplayerAgentAPI.IsDebugging = config.playFabDebugging;
        PlayFabMultiplayerAgentAPI.OnMaintenanceCallback += OnMaintenance;
        PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnShutdown;
        PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
        PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnAgentError;

        //UNetServer.OnPlayerAdded.AddListener(OnPlayerAdded);
        //UNetServer.OnPlayerRemoved.AddListener(OnPlayerRemoved);

        StartCoroutine(ReadyForPlayers());
        //StartCoroutine(ShutdownServerInXTime());

    }

    #region Remote Server Event Handlers

    IEnumerator ShutdownServerInXTime()
    {
        yield return new WaitForSeconds(300f);
        StartShutdownProcess();
    }

    IEnumerator ReadyForPlayers()
    {
        yield return new WaitForSeconds(.5f);
        PlayFabMultiplayerAgentAPI.ReadyForPlayers();
    }

    private void OnServerActive()
    {
        //UNetServer.StartServer();
        networkManager.StartServer();
        Debug.Log("Server Started From Agent Activation");
    }
    private void OnAgentError(string error)
    {
        Debug.Log(error);
    }

    private void OnShutdown()
    {
        StartShutdownProcess();
    }

    private void StartShutdownProcess()
    {
        Debug.Log("Server is shutting down");
        /*
        foreach (var conn in networkManager.Connections)
        {
            conn.Connection.Send(CustomGameServerMessageTypes.ShutdownMessage, new ShutdownMessage());
        }
        */
        StartCoroutine(ShutdownServer());
    }

    IEnumerator ShutdownServer()
    {
        yield return new WaitForSeconds(5f);
        Application.Quit();
    }

    private void OnMaintenance(DateTime? NextScheduledMaintenanceUtc)
    {
        Debug.LogFormat("Maintenance scheduled for: {0}", NextScheduledMaintenanceUtc.Value.ToLongDateString());
        /*
        foreach (var conn in UNetServer.Connections)
        {
            conn.Connection.Send(CustomGameServerMessageTypes.ShutdownMessage, new MaintenanceMessage()
            {
                ScheduledMaintenanceUTC = (DateTime)NextScheduledMaintenanceUtc
            });
        }
        */
    }
    #endregion
}