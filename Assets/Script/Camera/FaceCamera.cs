using UnityEngine;
using System.Collections;


/// <summary>
/// Make an object with this script always face to the camera
/// </summary>
public class FaceCamera : MonoBehaviour {


	void Start () {
	
	}
	

	void Update () {
        this.transform.LookAt(Camera.main.transform.position);
        this.transform.Rotate(new Vector3(0, 180, 0));
             }
}
