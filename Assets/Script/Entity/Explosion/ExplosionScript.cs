using UnityEngine;


/// <summary>
/// Handle the explosion game logic
/// </summary>
public class ExplosionScript : MonoBehaviour {


	void Start () {
        // planned destruction
        Destroy(gameObject, 3); // 3sec
    }

}
