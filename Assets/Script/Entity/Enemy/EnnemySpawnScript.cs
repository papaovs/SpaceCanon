using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Instantiate the wave of enemy
/// </summary>
public class EnnemySpawnScript : NetworkBehaviour
{

    /// <summary>
    /// To know if the spawner can continue to send enemies(true if is the case)
    /// </summary>
    public bool sendingWave = false;

    /// <summary>
    /// Instantiate a wave of enemy
    /// <param name=ennemy>enemy prefab </param>
    /// <param name=speedFactor>speed multiplier </param>
    /// <param name=number>number of enemy </param>
    /// <param name=delay>delay between enemy's instantiation </param>
    /// </summary>
    public void LaunchWave(GameObject ennemy, int speedFactor, int number, float delay)
    {

        StartCoroutine(SpawnEnnemy(ennemy, speedFactor, number, delay));

    }


    /// <summary>
    /// Instantiate an enemy and wait the for the delay time
    /// <param name=ennemy>enemy prefab </param>
    /// <param name=speedFactor>speed multiplier </param>
    /// <param name=number>number of enemy </param>
    /// <param name=delay>delay between enemy's instantiation </param>
    /// </summary>
    private IEnumerator SpawnEnnemy(GameObject ennemy, int speedFactor,int number, float delay)
    {
        sendingWave = true; //stop the enemy of this kind to spawn
        for (int i = 0; i < number; ++i)
        {

            if(sendingWave)
            {
                GameObject newEnnemy = Instantiate(ennemy, transform.position, transform.rotation) as GameObject;
                MoveEnnemy moveE = newEnnemy.GetComponent<MoveEnnemy>();
                moveE.speedFactor = speedFactor;
                moveE.SetRallyPoint(transform);
                NetworkServer.Spawn(newEnnemy);
                yield return new WaitForSeconds(delay);
            }
            else
            {
                i = number;//stop the enemy to comming
            }

            
        }
        sendingWave = false;
    }


}
