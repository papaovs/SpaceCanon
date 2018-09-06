using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Network;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Manage the game and call the update for each client
/// </summary>
public class NetworkGameManager : NetworkBehaviour
{
    /// <summary>
    /// list of the player
    /// </summary>
    static public List<PlayerScript> sPLayers = new List<PlayerScript>();

    /// <summary>
    /// Instance of this manager
    /// </summary>
    static public NetworkGameManager sInstance = null;


    /// <summary>
    /// Zone in the canevas where the score text will be displayed
    /// </summary>
    public GameObject uiScoreZone;

    /// <summary>
    /// font used for the score text
    /// </summary>
    public Font uiScoreFont;

    /// <summary>
    /// Zone in the canevas where the life text will be displayed
    /// </summary>
    public GameObject uiLifeZone;

    /// <summary>
    /// font used for the life text
    /// </summary>
    public Font uiLifeFont;


    /// <summary>
    /// boolean to know if all the cannon or the base ship has been destroyed
    /// and then that the game it's over
    /// </summary>
    private static bool _allDestroyed = false;


    [Space]

    /// <summary>
    /// boolean to know if we are in game or in the lobby
    /// </summary>
    private bool _running = true;

    void Awake()
    {
         sInstance = this;
    }

    void Start()
    {

        //initialise all the players
        for (int i = 0; i < sPLayers.Count; ++i)
        {
            sPLayers[i].Init();
        }
    }



    [ServerCallback]
    void Update()
    {
        if (!_running || sPLayers.Count == 0)//do the update only if we are in game and there is player
            return;


        if (_allDestroyed)//if the all the sips or the cannon has been destroyed return to the lobby
        {
            StartCoroutine(ReturnToLoby());
        }
    }


    /// <summary>
    /// Update the team's score for each player
    /// <param name=level> amount(level of the ennemy destroyed) that will be added to the score </param>
    /// </summary>
    public static void UpdateScore(int level) 
    {
        for (int i = 0; i < sPLayers.Count; ++i)
        {
            sPLayers[i].score += level;
        }
    }


    /// <summary>
    /// Update the life of a part of the ship for each player
    /// <param name=gameObject> part of the ship that is updated </param>
    /// <param name=hp> new hp value for the part of the ship </param>
    /// </summary>
    public static void Updatelife(GameObject gameObject,int hp)
    {

        if (gameObject.name == "BaseShip_1")
        {
            GameObject.Find("BaseShip_2").GetComponent<HealthScript>().hp = hp;
        }
        else if (gameObject.name == "BaseShip_2")
        {
            GameObject.Find("BaseShip_1").GetComponent<HealthScript>().hp = hp;
        }


        for (int i = 0; i < sPLayers.Count; ++i)
        {
            sPLayers[i].Updatelife( gameObject, hp);

        }

    }

    /// <summary>
    /// Inform all the player that the game is in pause or not
    /// </summary>
    public static void SwitchPlayerinPause()
    {
        for (int i = 0; i < sPLayers.Count; ++i)
        {
            sPLayers[i].inPause = !sPLayers[i].inPause;
        }  
            
    }

    /// <summary>
    /// Do the process for a destroyed part of the ship and
    /// inform all the player that a part of the ship has been destroyed.
    /// If its the base of the ship that is destroyed set alldestroyed at tru
    /// and the game it's over.
    /// If it's a cannon, set it to not available.
    /// <param name=gameObject> part of the ship that is updated </param>
    /// <param name=hp> new hp value for the part of the ship </param>
    /// </summary>
    public static void DestroyShipsPart(GameObject gameObject)
    {

        if (gameObject.tag == "Cannon")
        {
            for (int i = 0; i < sPLayers.Count; ++i)
            {
                sPLayers[i].CannonIsDestroyed(gameObject);
            }
            gameObject.GetComponent<CannonScript>().available=false;
        }
        else if (gameObject.tag == "BaseShip")
        {
            GameObject[] lCannon = GameObject.FindGameObjectsWithTag("Cannon");
            foreach (GameObject cannon in lCannon)
            {

                for (int i = 0; i < sPLayers.Count; ++i)
                {
                    sPLayers[i].CannonIsDestroyed(cannon);
                }
            }
            GameObject ship = GameObject.Find("Ship");
            ship.SetActive(false);
            _allDestroyed = true;
        }



    }

    /// <summary>
    /// retrurn to lobby after 3 sec
    /// </summary>
    IEnumerator ReturnToLoby()
    {
        _running = false;
        yield return new WaitForSeconds(3.0f);
        LobbyManager.s_Singleton.ServerReturnToLobby();
    }




}
