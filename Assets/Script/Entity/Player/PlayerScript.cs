using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Networking;

/// <summary>
/// Player Management
/// </summary>
public class PlayerScript : NetworkBehaviour
{






    //tocheck
    // protected Rigidbody _rigidbody;
    // protected Collider _collider;




    /// <summary>
    /// The color of the player
    /// <remarks>
    ///      Synchronized variable
    /// </remarks>
    /// </summary>
    [SyncVar]
    public Color color;


    /// <summary>
    /// The name of the player
    /// <remarks>
    ///      Synchronized variable
    /// </remarks>
    /// </summary>
    [SyncVar]
    public string playerName;


    /// <summary>
    /// Indicate if the game is in pause or not
    /// <remarks>
    ///      Synchronized variable
    /// </remarks>
    /// </summary>
    [SyncVar]
    public bool inPause = false;


    /// <summary>
    /// Remaining life of the ship 
    /// <remarks>
    ///      Synchronized variable
    /// </remarks>
    /// </summary>
    [SyncVar(hook = "OnShipLifeChanged")]
    public int baseshipLife;

    /// <summary>
    /// Remaining life of the cannon 1 
    /// <remarks>
    ///      Synchronized variable
    /// </remarks>
    /// </summary>
    [SyncVar(hook = "OnCannon1LifeChanged")]
    public int cannonLife1;

    /// <summary>
    /// Remaining life of the cannon 2 
    /// <remarks>
    ///      Synchronized variable
    /// </remarks>
    /// </summary>
    [SyncVar(hook = "OnCannon2LifeChanged")]
    public int cannonLife2;

    /// <summary>
    /// Remaining life of the cannon 3 
    /// <remarks>
    ///      Synchronized variable
    /// </remarks>
    /// </summary>
    [SyncVar(hook = "OnCannon3LifeChanged")]
    public int cannonLife3;

    /// <summary>
    /// Remaining life of the cannon 4 
    /// <remarks>
    ///      Synchronized variable
    /// </remarks>
    /// </summary>
    [SyncVar(hook = "OnCannon4LifeChanged")]
    public int cannonLife4;

    /// <summary>
    /// The score done by the team
    /// <remarks>
    ///      Synchronized variable
    /// </remarks>
    /// </summary>
    [SyncVar(hook = "OnScoreChanged")]
    public int score;


    /// <summary>
    /// Text to display for the score
    /// </summary>
    protected Text _scoreText;

    /// <summary>
    /// Text to display for life of the ship and the current cannon
    /// </summary>
    protected Text _lifeText;

    /*
     protected float _rotation = 0;
     protected float _acceleration = 0;

     protected float _shootingTimer = 0;

     protected bool _canControl = true;
     */


    /// <summary>
    /// Control if Init has been called
    /// <remarks>
    /// hard to control WHEN Init is called (networking make order between object spawning non deterministic)
    /// so we call init from multiple location (depending on what between player & manager is created first).
    /// </remarks>
    /// </summary>
    protected bool _wasInit = false;

    /// <summary>
    /// Remaining life of the current cannon
    /// </summary>
    private int _thisCannonLife;


    /// <summary>
    /// GameObject of the current cannon
    /// </summary>
    private GameObject _thisCannon;


    /// <summary>
    /// GameObject of the player's camera
    /// </summary>
    private GameObject _pCamera;

    /// <summary>
    /// Control if the player has a canon or if it's out of game(true)
    /// </summary>
    private bool _outOfGame = false;


    void Start()
    {
        //Get the player camera
        _pCamera = GameObject.FindWithTag("MainCamera");


        //hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


        if (NetworkGameManager.sInstance != null)
        {//we MAY be awake late (see comment on _wasInit above), so if the instance is already there we init
            Init();
        }         
    }



    /// <summary>
    /// Command to ask to the server to give the authority for a cannon to this player
    /// <param name=cannonId> id of the object to set the authority </param>
    /// <param name=playerID> id of the player that will get the authority </param>
    /// </summary>
    [Command]
    void CmdSetANewCannon(NetworkIdentity cannonId, NetworkIdentity playerID)
    {


        cannonId.AssignClientAuthority(playerID.connectionToClient);
    }


    /// <summary>
    /// Do the necessery initalisation after a cannon has been set to the player
    /// </summary>
    void SetANewCannon()
    {

        //get the cursor of the cannon
        Transform curseur = _thisCannon.transform.Find("Curseur").transform;



        //give to the camera the transform of the cannon and its target
        Transform cannonTran = _thisCannon.transform;
        CameraScript camInfo = _pCamera.GetComponent<CameraScript>();
        camInfo.PlaceCamera(cannonTran, curseur);
 
        //get the remaining life of this cannon and update the text diplayed with it
        _thisCannonLife = _thisCannon.GetComponent<HealthScript>().hp;
        UpdateScoreLifeText();

        //give the authority to this cannon
        if (isServer)
        {
            _thisCannon.GetComponent<NetworkIdentity>().AssignClientAuthority(GetComponent<NetworkIdentity>().connectionToClient);
        }
        else
        {
            
            CmdSetANewCannon(_thisCannon.GetComponent<NetworkIdentity>(), GetComponent<NetworkIdentity>());
        } 

    }


    /// <summary>
    /// Command to ask to the server to reinit the cannon
    /// before it will be released by the player or when it's destroyed,
    /// include releasing the authority for this cannon to the player
    /// <param name=cannonId> id of the object to remove the authority </param>
    /// <param name=playerID> id of the player that has released the canon </param>
    /// <param name=numCannon> number of the released canon </param>
    /// <param name=isDestroyed> cannon is destroyed or not </param>
    /// </summary>
    [Command]
    void CmdReinitCannon(NetworkIdentity cannonId, NetworkIdentity playerID,int numCannon, bool isDestroyed)
    {
        //get the cannon by its number
        GameObject tempCannon = GameObject.Find("Cannon_" + numCannon);
        CannonScript cannonInfo = tempCannon.GetComponent<CannonScript>();

        //set the cannon available if it's not destroyed,reinitialize the name of the owner and the position of the cannon
        cannonInfo.available = !isDestroyed;
        tempCannon.GetComponentInChildren<TextMesh>().text = "";
        cannonInfo.ReinitPosition();


        //release the authority for the cannon to the player
        cannonId.RemoveClientAuthority(playerID.connectionToClient);
    }


    /// <summary>
    /// Reinit the cannon before it will be released by the player or when it's destroyed.
    /// <param name=isDestroyed> cannon is destroyed or not </param>
    /// </summary>
    private void ReinitCannon(bool isDestroyed)
    {
        CannonScript thiscannonInfo = _thisCannon.GetComponent<CannonScript>();
        /*thiscannonInfo.available = !isDestroyed;
        _thisCannon.GetComponentInChildren<TextMesh>().text = "";
        thiscannonInfo.ReinitPosition();*/


        if (!isServer)
        {
            //call the server to do the reinitialisation
            CmdReinitCannon(_thisCannon.GetComponent<NetworkIdentity>(), GetComponent<NetworkIdentity>(), thiscannonInfo.numCannon, isDestroyed);
        }
       else
        {
            //set the cannon available if it's not destroyed,reinitialize the name of the owner and the position of the cannon
            thiscannonInfo.available = !isDestroyed;
            _thisCannon.GetComponentInChildren<TextMesh>().text = "";
            thiscannonInfo.ReinitPosition();

            //release the authority for the cannon to the player
            _thisCannon.GetComponent<NetworkIdentity>().RemoveClientAuthority(GetComponent<NetworkIdentity>().connectionToClient);
        }
    }

    /// <summary>
    /// Command to ask to the server to init the cannon for the player who get this cannon
    /// <param name=numCannon> number of the canon </param>
    /// <param name=pName> name of the player who get the canon </param>
    /// <param name=col> color of the player who get the canon </param>
    /// </summary>
    [Command]
    void CmdSetAnAvailableCannon(int cannonNum,string pName, Color col)
    {

        //get the cannon by its number
        GameObject tempCannon = GameObject.Find("Cannon_" + cannonNum);
        CannonScript cannonInfo = tempCannon.GetComponent<CannonScript>();

        //set the cannon not available as it is now occupied,initialize the name and the color of the owner
        cannonInfo.available = false;
        tempCannon.GetComponentInChildren<TextMesh>().text = pName;
        tempCannon.GetComponentInChildren<TextMesh>().color = col;


    }


    /// <summary>
    /// search for an available cannon and assign it to the player if on has been found
    /// <returns>true if a available cannon has been found  </returns>
    /// </summary>
    bool findAnAvailableCannon()
    {

        //get all the cannons
        GameObject[] lCannon = GameObject.FindGameObjectsWithTag("Cannon");

        //get the first cannon available
        foreach (GameObject cannon in lCannon)
        {
            CannonScript cannonInfo = cannon.GetComponent<CannonScript>();

            if (cannonInfo.available)
            {

                //init the cannon found for the player
                    if (!isServer)
                {
                    CmdSetAnAvailableCannon(cannonInfo.numCannon, playerName, color);

                }
                    else
                {
                    cannonInfo.available = false;
                    cannon.GetComponentInChildren<TextMesh>().text = playerName;
                    cannon.GetComponentInChildren<TextMesh>().color = color;
                }
                _thisCannon = cannon;


                //a cannon has been found so the player is not out of the game
                _outOfGame = false;
                //return true as a cannon has been found
                return true;
            }

        }
        //no cannon has been found so the player is out of the game
        _outOfGame = true;
        //instead of moving a a cannon the player will move from the view out of game
        _thisCannon = GameObject.FindWithTag("GameOverView");

        //return false as no cannon has been found
        return false;
    }

   
    [ClientCallback]
    void Update()
    {
        //if its not the local player or the game are in pause we don't want to do the update 
        if (!isLocalPlayer || inPause)
        { return; }


        if (_thisCannon == null || _outOfGame)
        {
            //Get the list of the cannons then take the first one free
            if (findAnAvailableCannon())
            {
                SetANewCannon();
            }

        }
        else
        {
            //rotate the canon with the mouse's axis
            _thisCannon.GetComponent<MouseLook>().MouseLookUpdate();
        }
        if (_outOfGame)
        {
            return;//the player is out of game, so only the view should be updated 
        }


        //get the cannon script of the player's cannon
        CannonScript cannonInfo = _thisCannon.GetComponent<CannonScript>();
        GameObject tempCannon;


        //on left click the cannon will shoot it's projectiles
        if (Input.GetKey(KeyCode.Mouse0))
        {
            fire();

        }


        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {//if 1 is pressed on the alphanumeric keys, the player will switch cannon to the left if it's possible


            do
            {
                tempCannon = cannonInfo.leftCannon;
                cannonInfo = tempCannon.GetComponent<CannonScript>();
                if (tempCannon.GetComponent<CannonScript>().available)
                {
                    tempCannon.transform.rotation = _thisCannon.transform.rotation;
                    ReinitCannon(false);
                    _thisCannon = tempCannon;
                    SetANewCannon();

                    cannonInfo.available = false;
                    return;
                }
            } while (tempCannon != _thisCannon);


        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {//if 2 is pressed on the alphanumeric keys, the player will switch cannon to the right if it's possible


            do
            {
                tempCannon = cannonInfo.rightCannon;
                cannonInfo = tempCannon.GetComponent<CannonScript>();
                if (tempCannon.GetComponent<CannonScript>().available)
                {

                    tempCannon.transform.rotation = _thisCannon.transform.rotation;
                    ReinitCannon(false);
                    _thisCannon = tempCannon;

                    SetANewCannon();

                    cannonInfo.available = false;
                    return;
                }
            } while (tempCannon != _thisCannon);


        }




    }





    //network


    void Awake()
    {
        //register the player in the gamemanager, that will allow to loop on it.
        NetworkGameManager.sPLayers.Add(this);
    }

    /// <summary>
    /// Initialise the text's fileds of the player
    /// </summary>
    public void Init()
    {
        if (_wasInit)
            return;

        GameObject scoreGO = new GameObject("score");
        scoreGO.transform.SetParent(NetworkGameManager.sInstance.uiScoreZone.transform, false);
        scoreGO.transform.Translate(NetworkGameManager.sInstance.uiScoreZone.transform.position);
        _scoreText = scoreGO.AddComponent<Text>();
        _scoreText.alignment = TextAnchor.LowerRight;
        _scoreText.font = NetworkGameManager.sInstance.uiScoreFont;
        _scoreText.resizeTextForBestFit = true;
        _scoreText.color = color;

        GameObject lifeGO = new GameObject("life");
        lifeGO.transform.SetParent(NetworkGameManager.sInstance.uiLifeZone.transform, false);
        lifeGO.transform.Translate(NetworkGameManager.sInstance.uiLifeZone.transform.position);
        _lifeText = lifeGO.AddComponent<Text>();
        _lifeText.alignment = TextAnchor.LowerLeft;
        _lifeText.font = NetworkGameManager.sInstance.uiScoreFont;
        _lifeText.resizeTextForBestFit = true;
        _lifeText.color = color;
        _wasInit = true;

        UpdateScoreLifeText();

    }


    /// <summary>
    ///  Shoot projectile from the shooters of the cannon
    /// </summary>
    private void fire()
    {
        CannonScript thiscannonInfo = _thisCannon.GetComponent<CannonScript>();
        if(!isServer)
        {
            //ask to the server to make the player'scannon shoot bullet
            CmdFire(thiscannonInfo.numCannon);
        }
        else
        {
          

            ShootScript shootR1 = _thisCannon.transform.Find("BoltSpawner1").GetComponent<ShootScript>();
            ShootScript shootR2 = _thisCannon.transform.Find("BoltSpawner2").GetComponent<ShootScript>();

                shootR1.ShootBolt();
                shootR2.ShootBolt();
            //inform the other client that this canon shoot bullet
            RpcFire(thiscannonInfo.numCannon);
        }

    }

    /// <summary>
    ///  Ask to the server to make the player's cannon Shoot projectile from its shooters
    /// <param name=numCannon>number of the cannon </param>
    /// </summary>
    [Command]
    public void CmdFire(int numCannon)
    {

        GameObject tempCannon = GameObject.Find("Cannon_" + numCannon);

        ShootScript shootR1 = tempCannon.transform.Find("BoltSpawner1").GetComponent<ShootScript>();
        ShootScript shootR2 = tempCannon.transform.Find("BoltSpawner2").GetComponent<ShootScript>();
        if (isServer && isClient) //avoid to create bullet twice (here & in Rpc call) on hosting client
        {
            shootR1.ShootBolt();
            shootR2.ShootBolt();
        }
        RpcFire(numCannon);
    }

    /// <summary>
    ///  inform the other client that the player's cannon Shoot projectile from its shooters
    /// <param name=numCannon>number of the cannon </param>
    /// </summary>
    [ClientRpc]
    public void RpcFire(int numCannon)//(string s1, string s2)
    {

        GameObject tempCannon = GameObject.Find("Cannon_" + numCannon);

        ShootScript shootR1 = tempCannon.transform.Find("BoltSpawner1").GetComponent<ShootScript>();
        ShootScript shootR2 = tempCannon.transform.Find("BoltSpawner2").GetComponent<ShootScript>();

            shootR1.ShootBolt();
            shootR2.ShootBolt();
        

    }

    // --- Score & Life management & display

    /// <summary>
    ///  When the score value is modified, update the text displayed to the player
    /// <param name=newValue>team's score </param>
    /// </summary>
    void OnScoreChanged(int newValue)
    {
        score = newValue;
        UpdateScoreLifeText();
    }

    /// <summary>
    ///  When the Ship life value is modified, update the text displayed to the player
    /// <param name=newValue>Ship life</param>
    /// </summary>
    void OnShipLifeChanged(int newValue)
    {



        baseshipLife = newValue;
        UpdateScoreLifeText();
    }

    /// <summary>
    ///  When the Ship life value is modified, update the text displayed to the player
    /// <param name=newValue>Cannon 1 life</param>
    /// </summary>
    void OnCannon1LifeChanged(int newValue)
    {



        cannonLife1 = newValue;
        UpdateScoreLifeText();
    }

    /// <summary>
    ///  When the Ship life value is modified, update the text displayed to the player
    /// <param name=newValue>Cannon 2 life</param>
    /// </summary>
    void OnCannon2LifeChanged(int newValue)
    {



        cannonLife2 = newValue;
        UpdateScoreLifeText();
    }

    /// <summary>
    ///  When the Ship life value is modified, update the text displayed to the player
    /// <param name=newValue>Cannon 3 life</param>
    /// </summary>
    void OnCannon3LifeChanged(int newValue)
    {



        cannonLife3 = newValue;
        UpdateScoreLifeText();
    }

    /// <summary>
    ///  When the Ship life value is modified, update the text displayed to the player
    /// <param name=newValue>Cannon 4 life</param>
    /// </summary>
    void OnCannon4LifeChanged(int newValue)
    {



        cannonLife4 = newValue;
        UpdateScoreLifeText();
    }



    /// <summary>
    ///  Update the text displayed to the player for the ship life,current cannon life an score
    /// </summary>
    private void UpdateScoreLifeText()
    {

        if (!isLocalPlayer )
            return;

        if (_scoreText != null)
        {

            _scoreText.text = "TEAM SCORE:" + score+"";

          
        }
        if (_lifeText != null)
        {
            _lifeText.text = " SHIP LIFE :";
            int shiplife = baseshipLife;
            _lifeText.text += shiplife + "\nCANNON LIFE: " + _thisCannonLife + "" ;

        }

            
               
         
    }

    /// <summary>
    ///  Update the  the ship and cannon life 
    /// <param name=gameObject>the gameobject that has life value updated</param>
    /// <param name=hp>new life value for this objet</param>
    /// </summary>
    public void Updatelife(GameObject gameObject, int hp)// à modifier
    {
 

            if (gameObject.tag == "BaseShip")
            {
                baseshipLife = hp;
            }
            else if(gameObject.tag == "Cannon")
        {
            if (gameObject == _thisCannon)
            {
                _thisCannonLife = hp;
            }
            if (gameObject.name == "Cannon_1")
            {
                cannonLife1 = hp;
            }
            else if (gameObject.name == "Cannon_2")
            {
                cannonLife2 = hp;
            }
            else if (gameObject.name == "Cannon_3")
            {
                cannonLife3 = hp;
            }
            else if (gameObject.name == "Cannon_4")
            {
                cannonLife4 = hp;
            }
        }
        UpdateScoreLifeText();

    }

    /// <summary>
    /// Inform a player that a cannon has been destroyed.
    /// If it's the plyer's cannon, leave it and search a new one if possible
    /// <param name=cannonDestroyed>cannon that is destroyed</param>
    /// </summary>
    public void CannonIsDestroyed( GameObject cannonDestroyed)
    {

       if(_thisCannon== cannonDestroyed)
        {
            ReinitCannon(true);
            if (findAnAvailableCannon())
            {
                SetANewCannon();
            }
        }

    }




}



