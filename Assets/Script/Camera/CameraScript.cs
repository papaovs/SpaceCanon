using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Place and orient the player's camera
/// </summary>
public class CameraScript : MonoBehaviour {




    /// <summary>
    /// The point of view where the camera has to be placed
    /// </summary>
    public Transform pointOfView;

    /// <summary>
    /// The target that the camera mus look at
    /// </summary>
    public Transform target;

    /// <summary>
    /// The offset of the camera with the point of view
    /// </summary>
    public Vector3 offset = new Vector3(0f, 0f, 0f);


    /// <summary>
    /// The rigidbody of the projectile
    /// </summary>
    private Transform _thisTransform;


    
    void Start () {
        _thisTransform = transform; // cache the transform of the camera so it doesn't need to be looked up at each frame.

        
	}
	



    void LateUpdate()
    {
        if(!pointOfView)
        {
            return;
        }

        //control the playerCamera position
        Vector3 wantedposition = pointOfView.position+  (pointOfView.rotation * offset);
        transform.position = wantedposition;

        //look at the target, it will be the target of the cannon or the gameover view
         _thisTransform.transform.LookAt(target.position);

    }


    /// <summary>
    /// Send to all the client that an explosion must take place at this position
    /// <param name=PosCam> transform of the new point of view </param>
    /// <param name=erotation> transform of the new target </param>
    /// </summary>
    public void PlaceCamera(Transform PosCam, Transform cursor)
    {
        pointOfView = PosCam;
       target = cursor;
    }
}
