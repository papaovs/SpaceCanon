using UnityEngine;
using System.Collections;
using UnityEngine.Networking; //to remove if text moved on an other classes
using System;


/// <summary>
/// Manage the Cannon
/// </summary>
public class CannonScript : NetworkBehaviour
{



    /// <summary>
    /// Cannon on the left of this cannon
    /// </summary>
    public GameObject leftCannon;

    /// <summary>
    /// Cannon on the right of this cannon
    /// </summary>
    public GameObject rightCannon;

    /// <summary>
    /// Number of the cannon
    /// </summary>
    public int numCannon=0;

    /// <summary>
    /// Cannon available for a player or not
    /// false if occupied or destroyed
    /// </summary>
    [SyncVar]
    public bool available = true;

    /// <summary>
    /// Original position of the cannon
    /// </summary>
    [SyncVar]
    Vector3 positionInit;

    /// <summary>
    /// Original rotation of the cannon
    /// </summary>
    [SyncVar]
    Quaternion rotationInit;



    void Start () {

        //get original position and rotation of the cannon
        positionInit = transform.position;
        rotationInit = transform.rotation;
    }
	
    public void ReinitPosition()
    {
        //reset the transform of the cannon to its original position and rotation
        transform.position = positionInit;
        transform.rotation = rotationInit;
    }


}
