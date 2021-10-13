using UnityEngine;

public class JoystickInput : MonoBehaviour
{
    public float sensitivity = 0.01f;
    public float motionDamping = 0.9f;

    bool isHolding;
    
    Vector3 origin;
    Vector3 vector;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isHolding = true;
            origin = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
        }
    }

    void FixedUpdate()
    {
        if (isHolding)
        {
            var axis = origin - Input.mousePosition;
            axis = Vector3.ClampMagnitude(axis * sensitivity, 1f);

            vector = Vector3.Lerp(vector, axis, motionDamping);
        }
        else
        {
            vector *= motionDamping;
        }
    }

    public Vector3 GetOriginScreenPos()
    {
        return origin;
    }
    
    public Vector3 GetMoveVector()
    {
        return new Vector3(vector.x, 0, vector.y);
    }
    
    public Vector3 GetRawVector()
    {
        return vector;
    }

    public bool IsDown()
    {
        return isHolding;
    }
}