using UnityEngine;
using UnityStandardAssets.Network;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Synchronize the player in game with the player in the lobby
/// </summary>
public class NetworkLobbyHook : LobbyHook
{

    /// <summary>
    ///On the Loading of the game, synchronize the player in game with the player in the lobby
    /// <param name=manager>the network manager </param>
    /// <param name=lobbyPlayer>plyer in the lobby </param>
    /// <param name=gamePlayer>player in game </param> 
    /// </summary>
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerScript player = gamePlayer.GetComponent<PlayerScript>();

        player.playerName = lobby.playerName;
        player.color = lobby.playerColor;
        player.score = 0;
        
    }
}
