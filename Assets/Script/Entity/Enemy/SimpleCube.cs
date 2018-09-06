using UnityEngine;
using System.Collections;

/// <summary>
///Behavior of SimpleCube 
/// </summary>
public class SimpleCube : MoveEnnemy
{


    


    protected override void Start () {
        base.Start();
        //get the bolt spawner from where the projectiles will be shooted
        shooter = transform.Find("BoltSpawner").GetComponent<EnemyShootScript>();

        //update the delay to make that even with diferrent speed the distance between enemy is always the same
        shooter.delayTime = shooter.delayTime / speedFactor;

    }

    /// <summary>
    /// Get the rally point for the spacmine from the spawner given
    /// <param name=spawner>spawner where the enemy will be instantiated </param>
    /// </summary>
    public override void SetRallyPoint(Transform spawner)
    {
        base.SetRallyPoint(spawner);
        sRallyPoint = spawner.Find("SimpleCubeRallyPointInfo").GetComponent<RallyPointInfo>().sRallyPOint;

    }


    public override void FixedUpdate() {
        if(!isServer || sRallyPoint == null)//only the server will update the enemy position
        {
            return;
        }
        base.FixedUpdate();
        if (firstPointReached && indexRallyPoint < 7)//if it's between the first and the last rally point, it will shoot randomly with 20% chance
        {

            if (Random.Range(0, 100) < 20)
                Shoot();
        }

        if (indexRallyPoint == 2 && previousUpdateIndex!= indexRallyPoint)//rotate at the third rally point to always shoot in the direction of the ship
        {
            Quaternion xQuaternion = Quaternion.AngleAxis(90, Vector3.up);

            rig.MoveRotation(rig.rotation * xQuaternion);


         }
        else if (indexRallyPoint == 5 && previousUpdateIndex != indexRallyPoint)//rotate at the sixth rally point to always shoot in the direction of the ship
        {
            Quaternion xQuaternion = Quaternion.AngleAxis(90, Vector3.right);
            rig.MoveRotation(rig.rotation * xQuaternion);


        }
        else if (indexRallyPoint == 6 && previousUpdateIndex != indexRallyPoint)//rotate at the 7th rally point to always shoot in the direction of the ship
        {
            Quaternion xQuaternion = Quaternion.AngleAxis(90, Vector3.right);
            rig.MoveRotation(rig.rotation * xQuaternion);


        }
        else if (indexRallyPoint == 7)//accelerate to go to the last rally point 
        { speedFactor = 10; }

       
    }



}
