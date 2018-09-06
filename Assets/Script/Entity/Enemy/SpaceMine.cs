using UnityEngine;

/// <summary>
///Behavior of SpaceMine 
/// </summary>
public class SpaceMine : MoveEnnemy
{



    protected override void Start()
    {
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
        sRallyPoint = spawner.Find("SpaceMineRallyPointInfo").GetComponent<RallyPointInfo>().sRallyPOint;

    }


    public override void FixedUpdate()
    {
        if (!isServer || sRallyPoint == null)//only the server will update the enemy position
        {
            return;
        }
        base.FixedUpdate();

    }

}
