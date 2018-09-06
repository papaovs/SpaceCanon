using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///Behavior of Hunter 
/// </summary>
public class HunterScript : MoveEnnemy
{

    //private const int NB_CANNON = 4;
    //public Vector3[] sPosCannon=new Vector3[NB_CANNON];

    /// <summary>
    ///Ship's position
    /// </summary>
    private Vector3 _posShip;

    protected override void Start()
    {
        base.Start();
        //get the bolt spawners from where the projectiles will be shooted
        shooter = transform.Find("BoltSpawner1").GetComponent<EnemyShootScript>();
        shooterBis = transform.Find("BoltSpawner2").GetComponent<EnemyShootScript>();

        //update the delay to make that even with diferrent speed the distance between enemy is always the same
        shooter.delayTime = shooter.delayTime / speedFactor;
        shooterBis.delayTime = shooter.delayTime / speedFactor;


      // GameObject[] gCannon =  GameObject.FindGameObjectsWithTag("Cannon");

        _posShip=GameObject.Find("Ship").transform.position;
        /*for (int i=0; i<gCannon.Length && i<NB_CANNON; i++)
        {
           sPosCannon[i]= gCannon[i].transform.position;
        }*/
    }

    /// <summary>
    /// Get the rally point for the spacmine from the spawner given
    /// <param name=spawner>spawner where the enemy will be instantiated </param>
    /// </summary>
    public override void SetRallyPoint(Transform spawner)
    {
        base.SetRallyPoint(spawner);
        sRallyPoint = spawner.Find("HunterRallyPointInfo").GetComponent<RallyPointInfo>().sRallyPOint;

    }


    public override void Update()
    {
        if (!isServer || sRallyPoint==null)//only the server will update the enemy position
        {
            return;
        }
        base.Update();
        if (firstPointReached &&  indexRallyPoint < 16)//if it's between the first and the 17th rally point, start to shoot bullet.
            {
            // int indCannon = Random.Range(0, 4);
            // transform.LookAt(sPosCannon[indCannon] + Vector3.up * Random.Range(-4f, 5f));

            //look at the ship with a random delta
            transform.LookAt(_posShip + Vector3.up * Random.Range(-3f, 3f));

            //shoot randomly with 0.1% at 1%(depending of the speed factor) chance for each canon
            if (Random.Range(0, 1000 - (45 * speedFactor)) < 1)
                    Shoot();

                if (Random.Range(0, 1000-(45*speedFactor)) < 1)
                    ShootBis();
            }
            else if(indexRallyPoint > 16 && indexRallyPoint < sRallyPoint.Count)//if it's after the 17th rally point,look at next rally point and accelerate.
        {
               transform.LookAt(sRallyPoint[indexRallyPoint].transform.position);
            speedFactor = 10;
        }
       


    }


    public override void FixedUpdate()
    {

        if (!isServer)//only the server will update the enemy position
        {
            return;
        }
        base.FixedUpdate();
    }



}
