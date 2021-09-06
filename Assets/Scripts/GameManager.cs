using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private const string PLAYER_ID_PREFIX = "Player "; 

    private static Dictionary<string, PlayerManager> players = new Dictionary<string, PlayerManager>();

    public static void registerPlayer(string _netID, PlayerManager _player)
    {
        string playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(playerID, _player);
        _player.transform.name = playerID;
    }

    public static void unRegisterPlayer(string playerID)
    {
        players.Remove(playerID);
    }

    public static PlayerManager getPlayer(string playerID)
    {
        return players[playerID];
    }
}
