using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// Move the projectile shooted from a enemy entity.
/// <remarks>
///      It's will only be managed by the server
/// </remarks>
/// </summary>
public class EnemyMoveBolt : NetworkBehaviour{

    /// <summary>
    /// The speed of the projectile
    /// </summary>
    public float speed = 1f;

    /// <summary>
    /// The rigidbody of the projectile
    /// </summary>
    private Rigidbody _rig;


    void Start()
    {
        if (!isServer)//If we are on the server set the force to apply on the projectile's rigidbody
        { return; }
        _rig = GetComponent<Rigidbody>();
        _rig.AddForce(_rig.transform.TransformDirection(Vector3.forward * speed));

    }


}
