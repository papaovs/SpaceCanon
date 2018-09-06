using UnityEngine;
using System.Collections;

public class MoveShipScript : MonoBehaviour {


   public float speed = 0.007f;
    float acceleration = 0.0001f;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey("z"))
        {
            // transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.007f);
            if (speed + acceleration <= 10)
                speed += acceleration;
        }
        if (Input.GetKey("s"))
        {
            //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z-0.007f);
            if(speed-acceleration>=0)
            speed -= acceleration;
        }
        if (Input.GetKey("d"))
        {
           // transform.position = new Vector3(transform.position.x+ 0.007f, transform.position.y, transform.position.z );
        }
        if (Input.GetKey("q"))
        {
           // transform.position = new Vector3(transform.position.x-0.007f, transform.position.y, transform.position.z );
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed);
    }
}
