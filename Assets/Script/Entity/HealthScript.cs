using System;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Dammage and lifepoint management
/// </summary>
public class HealthScript : NetworkBehaviour
{
    /// <summary>
    /// Life points
    /// </summary>
    public int hp = 1;

    /// <summary>
    ///  Points if killed
    /// </summary>
    public int level = 10;


    /// <summary>
    ///  Damage if it's suicide by collision with the player
    /// </summary>
    public int damage = 10;


    /// <summary>
    /// Ennemy or Friend ?
    /// </summary>
    public bool isEnemy = true;

    /// <summary>
    /// The explosion that will be used
    /// </summary>
    public GameObject explosion;


    void Start()
    {
        if (!isEnemy) //if it's a part of the ship, inform all the player of it's current hp.
        NetworkGameManager.Updatelife(this.gameObject, hp);
    }


    /// <summary>
    /// Test the collision wuth a projectile or a enemy kamikaze
    /// <param name=collider> collider the enter in collision with it, in case if it's a projectile or an enemy kamikaze </param>
    /// </summary>
    [ServerCallback]
    void OnTriggerEnter(Collider collider)
    {
        // is it a bolt?
        BoltScript shot = collider.gameObject.GetComponent<BoltScript>();
        if (shot != null)
        {
            // ally or enemy shoot?
            if (shot.isEnemyShot != isEnemy)
            {
                hp -= shot.damage; //if the projectile can damege it remove the damage value from hp.

                if (!isEnemy) //if it's a part of the ship, inform all the player of it's new hp value.
                    NetworkGameManager.Updatelife(this.gameObject,hp);

               
                if (hp <= 0)
                {
                    // Destruction !
                    //Create an explosion where this GameObject is
                    //if it's an ennemy: destroy it and update the team score.
                    //if it's a part of the ship, call DestroyShipsPart in NetworkGameManager to manage it correctly
                    Instantiate(explosion, transform.position, transform.rotation);
                    RpcCreateExplosion(transform.position, transform.rotation);
                    if (isEnemy)
                    {
                        DestroyIt();
                        NetworkGameManager.UpdateScore(level);
                    }
                    else
                    {
                        NetworkGameManager.DestroyShipsPart(this.gameObject);

                    }

                   

                }
                //  Projectile's destruction
                //Create also an explosion at the place of the projectile
                //the destroy it
                Instantiate(explosion, collider.transform.position, collider.transform.rotation);
                RpcCreateExplosion(collider.transform.position, collider.transform.rotation);
                Destroy(shot.gameObject);
                if(shot.isEnemyShot)
                {
                    collider.gameObject.GetComponent<HealthScript>().RpcDestroyIt(); ;
                }
            }
            else if (!isEnemy)
            {
                //  friend's projectiles can't go througth the ship or the cannon
                //But ennemy projectil can go trhought ennemy for gameplay reason
                Destroy(shot.gameObject);
            }



        }
        else
        {

            //in the case it's a kamikaze ennemy that enter in collision with a ship's part

            // is it an enemy?
            HealthScript kamikaze = collider.gameObject.GetComponent<HealthScript>();
            if (kamikaze != null && kamikaze.isEnemy)
            {
                // in this case we just test the collision on the ship'part
                if (!isEnemy)
                {
                    hp -= kamikaze.damage;                    
                    NetworkGameManager.Updatelife(this.gameObject, hp);


                    if (hp <= 0)
                    {
                        // Destruction !
                        Instantiate(explosion, transform.position, transform.rotation);
                        RpcCreateExplosion(transform.position, transform.rotation);

                            NetworkGameManager.DestroyShipsPart(this.gameObject);

                        
                        

                    }
                    //  Kamikaze's destruction
                    Instantiate(explosion, collider.transform.position, collider.transform.rotation);
                    RpcCreateExplosion(collider.transform.position, collider.transform.rotation);
                    kamikaze.RpcDestroyIt();
                    Destroy(kamikaze.gameObject);
                }

            }
        }
    }


    /// <summary>
    /// Send to all the client that this entity has to be destroyed
    /// </summary>
    [ClientRpc]
    private void RpcDestroyIt()
    {
       NetworkServer.Destroy(gameObject);


    }

    /// <summary>
    /// Send to all the client that an explosion must take place at this position
    /// <param name=eposition> position of the explosion </param>
    /// <param name=erotation> rotation of the explosion </param>
    /// </summary>
    [ClientRpc]
    private void RpcCreateExplosion(Vector3 eposition, Quaternion erotation)
    {
        Instantiate(explosion, eposition, erotation);


    }



    /// <summary>
    /// send to all the client the new hp of the entity
    /// <param name=newhp> the new hp value of this entity </param>
    /// </summary>
    [ClientRpc]
    private void RpcLifupdate(int newhp)
    {
        hp = newhp;
       

    }

    /// <summary>
    /// Destroy this entity and send to all the client that it has to be destroyed
    /// <param name=newhp> the new hp value of this entity </param>
    /// </summary>
    public void DestroyIt()
    {
        Destroy(gameObject);
        RpcDestroyIt();
    }


}