using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;


/// <summary>
///Handle the enemy movment 
/// </summary>
public class MoveEnnemy : NetworkBehaviour
{
    /// <summary>
    ///Enemy speed 
    /// </summary>
    public float speed = 0.01f;

    /// <summary>
    ///speed multiplier
    /// </summary>
    public int speedFactor = 1;

    /// <summary>
    ///speed multiplier
    /// </summary>
    protected Transform rallyPoint;

    /// <summary>
    ///Current rally point
    /// </summary>
    protected List<Transform> sRallyPoint;


    /// <summary>
    ///Indicate if the first rally point has been reched
    /// </summary>
    protected bool firstPointReached = false;

    /// <summary>
    ///index for the rally point list
    /// </summary>
    protected int indexRallyPoint = 0;

    /// <summary>
    ///Rigidbody of this enemy
    /// </summary>
    protected Rigidbody rig;


    /// <summary>
    ///Shooter of this enemy
    /// </summary>
    protected EnemyShootScript shooter;

    /// <summary>
    ///Second shooter of this enemy, if it has two shooter
    /// </summary>
    protected EnemyShootScript shooterBis;

    /// <summary>
    ///index of the previous rally point
    /// </summary>
    protected int previousUpdateIndex = 0;



    protected virtual void Start () {

       rig = GetComponent<Rigidbody>();//get the rigidbody


    }

    /// <summary>
    ///Set the rally point info for this ennemy
    ///has to be implemented for each kind of enemy as they use their own rally points
    /// <param name=spawner>spawner from where the enemy will spawn </param>
    /// </summary>
    public virtual void SetRallyPoint(Transform spawner)
    {


    }


    public virtual void FixedUpdate() {
        
        if (sRallyPoint==null)
        {
            return;
        }
        float step = speed * Time.deltaTime; 
        if(indexRallyPoint>= sRallyPoint.Count)
        {
            Destroy(gameObject); 
            return;
        }
        else
        {
            //update the current rally point and set the the previousUpdateIndex with the current index while it has not be incremented
            previousUpdateIndex = indexRallyPoint;
            rallyPoint = sRallyPoint[indexRallyPoint];
        }
        if (!firstPointReached)
        {
            //move to the first rally point
            //set a big acceleration to get to the first rally point.
            rig.MovePosition(Vector3.MoveTowards(rig.position, rallyPoint.position, step * speedFactor * 100));
            if (rig.position == rallyPoint.position)
            {
                firstPointReached = true;
                indexRallyPoint++;
            }
            
        }
        else
        {
            //move to the next rally point
            rig.MovePosition(Vector3.MoveTowards(rig.position, rallyPoint.position, step * speedFactor));
            if (rig.position == rallyPoint.position)
            {
                indexRallyPoint++;
            }

        }


    }

    public virtual void Update()
    {


    }

    /// <summary>
    ///  Shoot a projectile from the shooter
    /// </summary>
    protected void Shoot()
    {
        shooter.ShootBolt();

       // RpcShoot();
    }


    /// <summary>
    ///  Shoot a projectile from the shooterBis
    /// </summary>
    protected void ShootBis()
    {
        shooterBis.ShootBolt();

 
    }

}
