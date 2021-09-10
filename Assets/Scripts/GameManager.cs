using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public ImportantSettings importantSettings;

    //makes sure there is only one GameManager file in a given scene
    public static GameManager instance;

    [SerializeField] private GameObject sceneCamera;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Multiple GameManager instances in scene!");
        } 
        else
        {
            instance = this;
        }
        
    }

    public void setSceneCamera(bool _isActive)
    {
        if(sceneCamera == null)
        {
            return;
        }

        sceneCamera.SetActive(_isActive);
    }

    #region Player registering

    private const string PLAYER_ID_PREFIX = "Player "; 

    //create new dictionary to store player id and player names
    private static Dictionary<string, PlayerManager> players = new Dictionary<string, PlayerManager>();

    public static void registerPlayer(string _netID, PlayerManager _player)
    {
        string playerID = PLAYER_ID_PREFIX + _netID;
        //add playerID (key) and _player (value) to the dictionary
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
    #endregion
}
