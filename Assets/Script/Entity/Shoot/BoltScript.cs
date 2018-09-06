using UnityEngine;

/// <summary>
/// Handle the projectile game logic
/// </summary>
public class BoltScript : MonoBehaviour {
    
  

    /// <summary>
    /// Dommage done by this projectile
    /// </summary>
    public int damage = 1;

    /// <summary>
    /// Projectile friend or ennemy ?
    /// </summary>
    public bool isEnemyShot = false;

    /// <summary>
    /// Time the projectile will last once shooted
    /// </summary>
    public int lifeTime = 10;


    void Start () {
        // planned destruction
        Destroy(gameObject, lifeTime);
    }
	

}
