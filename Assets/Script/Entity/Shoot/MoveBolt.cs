using UnityEngine;

/// <summary>
/// Move the projectile shooted from a cannon
/// </summary>
public class MoveBolt : MonoBehaviour {


    /// <summary>
    /// The speed of the projectile
    /// </summary>
    public float speed = 1f;

    /// <summary>
    /// The rigidbody of the projectile
    /// </summary>
    private Rigidbody _rig;


    void Start () {

        //Set the force to apply on the projectile's rigidbody
        _rig = GetComponent<Rigidbody>();
        _rig.AddForce(_rig.transform.TransformDirection(Vector3.forward * speed));

    }


}
