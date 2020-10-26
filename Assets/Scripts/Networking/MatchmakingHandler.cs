using UnityEngine;
using PlayFab;
using PlayFab.MultiplayerModels;


public class MatchmakingHandler : MonoBehaviour
{


    LoginHandler _loginHandler;
    void Start()
    {

        _loginHandler = GetComponent<LoginHandler>();

    }

    /*
    public void CreateMatch()
    {
        PlayFabMultiplayerAPI.CreateMatchmakingTicket(
            new CreateMatchmakingTicketRequest
            {
                // The ticket creator specifies their own player attributes.
                Creator = new MatchmakingPlayer
                {
                    Entity = new EntityKey
                    {
                        Id = _loginHandler.player.PlayFabId,
                        Type = "title_player_account",
                    },

                    // Here we specify the creator's attributes.
                    Attributes = new MatchmakingPlayerAttributes
                    {
                        DataObject = new
                        {
                            Skill = 24.4
                        },
                    },
                },

                // Cancel matchmaking if a match is not found after 120 seconds.
                GiveUpAfterSeconds = 120,

                // The name of the queue to submit the ticket into.
                QueueName = "myqueue",
            },

            // Callbacks for handling success and error.
            this.OnMatchmakingTicketCreated,
            this.OnMatchmakingError);


    }
    */
    
    
    //Ticket creation success callback
    public void OnMatchmakingTicketCreated(CreateMatchmakingTicketResult result)
    {
        Debug.Log("Success creating ticket with id : ");
        Debug.Log(result.TicketId);
    }

    //Ticket creation fail callback
    public void OnMatchmakingError(PlayFabError error)
    {

        Debug.Log("Error creating ticket");
        Debug.LogError(error);
    }




}