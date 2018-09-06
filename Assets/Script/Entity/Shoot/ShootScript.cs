using UnityEngine;


/// <summary>
/// Shoot the projectile from a friendly entity
/// <remarks>
///       Has to be place on the shooter transform of the entity that must shoot the bullet
/// </remarks>
/// </summary>
public class ShootScript : MonoBehaviour
{
    /// <summary>
    /// The projectile that will be used
    /// </summary>
    public GameObject bullet;


    /// <summary>
    /// Delay time between two shot
    /// </summary>
    public float delayTime = 1;

    /// <summary>
    /// Counter for the delay time. 
    /// </summary>
    private float _counter;



	void Start () {
        _counter = delayTime;// set the counter to the same amount of the dely time to be able to shoot directly

    }

   
    void Update()
    {
        if (_counter <= delayTime)//while the counter is not high enough,count the time passed since the last shoot
            _counter = _counter + Time.deltaTime;
    }

    /// <summary>
    /// Shoot a bullet if possible
    /// </summary>
    public void ShootBolt()
    {
        if ( _counter > delayTime)//if the counter is high enough we can shoot
        {
            Instantiate(bullet, transform.position, transform.rotation);
            _counter = 0;
        }
     
    }



}
