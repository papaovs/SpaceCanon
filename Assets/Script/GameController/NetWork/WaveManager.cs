using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;


/// <summary>
/// Manage the ennemy wave
/// Manage the GUI to modify and send the waves
/// Send the enemy wave
/// </summary>
public class WaveManager : NetworkBehaviour
{

    /// <summary>
    /// List of the GameObjec for the different ennemies
    /// </summary>
    public List<GameObject> sEnnemyType = new List<GameObject>();

    /// <summary>
    /// List of the name for the different ennemies
    /// </summary>
    public List<string> sEnnemyName = new List<string>();


    /// <summary>
    /// List of the the different spawner
    /// </summary>
    public List<EnnemySpawnScript> sSpawner = new List<EnnemySpawnScript>();

    /// <summary>
    /// List of the name of the different spawner
    /// </summary>
    public List<string> sSpawnerName = new List<string>();

    /// <summary>
    /// Panel for the wave managment
    /// </summary>
    public GameObject panelWaveManagment;

    /// <summary>
    /// Maximum value to the speed factor
    /// </summary>
    public int maxspeedFactor=10;

    /// <summary>
    /// Miminum value to the speed factor
    /// </summary>
    public int minspeedFactor = 1;

    /// <summary>
    /// Maximum number of enemy per wave
    /// </summary>
    public int maxnumberPerWave = 20;

    /// <summary>
    /// Miminum number of enemy per wave
    /// </summary>
    public int minnumberPerWave = 1;


    /// <summary>
    /// Delay between two ennemy instantiation
    /// </summary>
    public float delayBTWNEnnemy = 3;


    /// <summary>
    /// Initial speed factor
    /// </summary>
    private int _speedFactor=1;

    /// <summary>
    /// Initial number of enemy per wave
    /// </summary>
    private int _numberPerWave=1;


    /// <summary>
    /// Initial index for the list of enemy type
    /// </summary>
    private int _indexType=0;

    /// <summary>
    /// Initial index for the list of spawner
    /// </summary>
    private int _indexSpawner=0;




    void Start () {

        //send an exception if the list of enemy name and the liste of enemy has diferent size
	if(sEnnemyName.Count== sEnnemyType.Count)
        {

        }
    else
        {
            //send an exception
            throw new System.Exception("The Ennemy Type list and the Ennemy Name list should have the same size");
        }
        //send an exception if the list of spawner name and the liste of spawner has diferent size
        if (sSpawnerName.Count == sSpawner.Count)
        {
         
        }
        else
        {
            //send an exception
            throw new System.Exception("The Spawner list and the Spawner Name list should have the same size");
        }

        //update the wave GUI and set it no active
        UpdateWaveGUI();
        SwitchVisibility(panelWaveManagment);
    }

     //connected
    void Update () {
        if(!isServer)//the wave manger GUI will only be diplayed on the server
        {
            return;
        }
        //Display or not the the Wave Panel Management
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {

            SwitchVisibilityAndPAuse(panelWaveManagment);
           

        }
    }




    /// <summary>
    /// Switch the visibility of a panel
    /// </summary>
    void SwitchVisibility(GameObject objectToSwitch)
    {
        objectToSwitch.SetActive(!objectToSwitch.activeSelf);

    }

    /// <summary>
    /// Switch the visibility of a panel
    /// and put the game in pause, or remove the pause if it was already paused
    /// </summary>
    void SwitchVisibilityAndPAuse(GameObject objectToSwitch)
    {

        Cursor.visible = !objectToSwitch.activeSelf;//hide the cursor if we will hide the panel
        if (Cursor.visible)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;


        SwitchPause();
        SwitchVisibility(objectToSwitch);
    }

    /// <summary>
    /// Put the game in pause, or remove the pause if it was already paused
    /// </summary>
    void SwitchPause()
    {
        Time.timeScale = 1 - Time.timeScale;
        NetworkGameManager.SwitchPlayerinPause();
    }

    /// <summary>
    /// Send a wave of enemy with the current parameter of the wave manager
    /// </summary>
    public void SendWave()
    {
        float delay = delayBTWNEnnemy/ _speedFactor;

        sSpawner[_indexSpawner].LaunchWave(sEnnemyType[_indexType], _speedFactor,_numberPerWave, delay);
        SwitchVisibilityAndPAuse(panelWaveManagment);

    }


    /// <summary>
    /// Destroy all entity of the specified types
    /// </summary>
    public void DestroyAllOfOneType()
    {

        foreach (EnnemySpawnScript spawner in sSpawner)//stop all the spawner to send wave
        {
            spawner.sendingWave = false;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(sEnnemyType[_indexType].tag);
        foreach (GameObject enemy in enemies)
        {
            EnemyShootScript[] enemyShoots = enemy.GetComponentsInChildren<EnemyShootScript>();
            //enemy.GetComponentsInChildren
            //GameObject[] enemyBolts = enemy.FindGameObjectsWithTag("BoltSpawner");
            foreach (EnemyShootScript enemyShoot in enemyShoots)
            {
                if(enemyShoot.bullet!=null)
                {
                    GameObject[] enemyBolts = GameObject.FindGameObjectsWithTag(enemyShoot.bullet.tag);
                    foreach (GameObject enemyBolt in enemyBolts)
                    {

                        enemyBolt.GetComponent<HealthScript>().DestroyIt();
                    }
                }


            }
                enemy.GetComponent<HealthScript>().DestroyIt();
        }


        SwitchVisibilityAndPAuse(panelWaveManagment);

    }

    /// <summary>
    /// Modify all entity of the specified types with the current speed parameter of the wave manager
    /// </summary>
    public void ModifySpeedAllOfOneType()
    {
        float delay = delayBTWNEnnemy / _speedFactor;
        sSpawner[_indexSpawner].LaunchWave(sEnnemyType[_indexType], _speedFactor, _numberPerWave, delay);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(sEnnemyType[_indexType].tag);
        foreach (GameObject enemy in enemies)
        {

           // MoveEnnemy moveE = enemy.GetComponent<MoveEnnemy>();
            enemy.GetComponent<MoveEnnemy>().speedFactor = _speedFactor;
        }


        SwitchVisibilityAndPAuse(panelWaveManagment);

    }

    /// <summary>
    /// Increment the speed factor if it's possible
    /// </summary>
    public void IncrementSpeedFactor()
    {
        if(_speedFactor<maxspeedFactor)
        {
            _speedFactor++;
            UpdateWaveGUI();
        }
       
    }

    /// <summary>
    /// Decrement the speed factor if it's possible
    /// </summary>
    public void DecrementSpeedFactor()
    {
        if (_speedFactor > minspeedFactor)
        {
            _speedFactor--;
            UpdateWaveGUI();
        }
    }

    /// <summary>
    /// Increment the numberof enemy per wave if it's possible
    /// </summary>
    public void IncrementNumberPerWave()
    {
        if (_numberPerWave < maxnumberPerWave)
        {
            _numberPerWave++;
            UpdateWaveGUI();
        }
    }

    /// <summary>
    /// Decrement the numberof enemy per wave if it's possible
    /// </summary>
    public void DecrementNumberPerWave()
    {
        if (_numberPerWave > minnumberPerWave)
        {
            _numberPerWave--;
            UpdateWaveGUI();
        }
    }

    /// <summary>
    /// Increment the enemy type index if it's possible
    /// and then change the type of enemy
    /// </summary>
    public void IncrementEnnemyTypeIndex()
    {
        if (_indexType < sEnnemyName.Count-1)
        {
            _indexType++;
            UpdateWaveGUI();
        }
    }

    /// <summary>
    /// Decrement the enemy type index if it's possible
    /// and then change the type of enemy
    /// </summary>
    public void DecrementEnnemyTypeIndex()
    {
        if (_indexType > 0)
        {
            _indexType--;
            UpdateWaveGUI();
        }
    }

    /// <summary>
    /// inccrement the spawner index if it's possible
    /// and then change the spawner
    /// </summary>
    public void IncrementSpawnerIndex()
    {
        if (_indexSpawner < sSpawner.Count - 1)
        {
            _indexSpawner++;
            UpdateWaveGUI();
        }
    }

    /// <summary>
    /// Decrement the spawner index if it's possible
    /// and then change the spawner
    /// </summary>
    public void DecrementSpawnerIndex()
    {
        if (_indexSpawner > 0)
        {
            _indexSpawner--;
            UpdateWaveGUI();
        }
    }


    /// <summary>
    /// Update the Wave GUI with the current value of the Wave Manager
    /// </summary>
    public void UpdateWaveGUI()
    {
        Text ennemyTypeText;
        Text ennemyNumberText;
        Text ennemySpawnerText;
        Text speedFactorText;



        ennemyTypeText = GameObject.Find("/Canvas/WaveManagement/EnnemyType").GetComponent<Text>();
        ennemyNumberText = GameObject.Find("/Canvas/WaveManagement/EnnemyNumber").GetComponent<Text>();
        speedFactorText = GameObject.Find("/Canvas/WaveManagement/EnnemySpeed").GetComponent<Text>();
        ennemySpawnerText = GameObject.Find("/Canvas/WaveManagement/Spawner").GetComponent<Text>();

        ennemyTypeText.text = "Type: " + sEnnemyName[_indexType];
        ennemySpawnerText.text = "Spawner: " + sSpawnerName[_indexSpawner];
        ennemyNumberText.text = "Number: " + _numberPerWave + "  ";
        speedFactorText.text = "Speed: x" + _speedFactor + "  ";

    }

}
