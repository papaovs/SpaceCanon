using UnityEngine;


/// <summary>
/// Make the player's cannon orientation follow the mouse's axis
/// </summary>
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
    /// <summary>
    /// sensitivity on X
    /// </summary>
    public float sensitivityX = 15F;

    /// <summary>
    /// sensitivity on Y
    /// </summary>
    public float sensitivityY = 15F;

    /// <summary>
    /// minimum angle on the X axis
    /// </summary>
    public float minimumX = -60F;

    /// <summary>
    /// maximal angle on the X axis
    /// </summary>
    public float maximumX = 60F;

    /// <summary>
    /// minimum angle on the X axis
    /// </summary>
    public float minimumY = -20F;

    /// <summary>
    /// maximal angle on the X axis
    /// </summary>
    public float maximumY = 55F;

    /// <summary>
    /// current rotation on the X axis
    /// </summary>
    private float _rotationX = 0F;

    /// <summary>
    /// current rotation on the Y axis
    /// </summary>
    private float _rotationY = 0F;

    /// <summary>
    /// original rotation of the cannon
    /// </summary>
    private Quaternion _originalRotation;



    /// <summary>
    /// Make the player's cannon orientation follow the mouse's axis
    /// </summary>
    public void MouseLookUpdate()
    {


            // Read the mouse input axis
            _rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            _rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            _rotationX = ClampAngle(_rotationX, minimumX, maximumX);
            _rotationY = ClampAngle(_rotationY, minimumY, maximumY);
            Quaternion xQuaternion = Quaternion.AngleAxis(_rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(_rotationY, Vector3.right);
            RotateCannon(_originalRotation * xQuaternion * yQuaternion);
  

    }
    void Start()
    {
        

        _originalRotation = this.transform.localRotation;//get original rotation of the cannon
    }

    /// <summary>
    /// Clamp the angle between the  min and max angle
    /// <param name=angle>angle to modify </param>
    /// <param name=min> minimum angle </param>
    /// <param name=max> maximum angle </param>
    /// </summary>
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
         angle += 360F;
        if (angle > 360f)
         angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    /// <summary>
    /// Rotate the cannon to the new orientation
    /// </summary>
    private void RotateCannon(Quaternion newRotation)
    {
        transform.localRotation = newRotation;
        
    }
}