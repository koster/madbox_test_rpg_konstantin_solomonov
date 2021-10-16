using UnityEngine;

public class JoystickInput : GameService
{
    public float pixelRadius = 100f;
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
            axis = Vector3.ClampMagnitude(axis, pixelRadius);
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
        var axisNormalizedToPixelRadius = new Vector3(vector.x / pixelRadius, vector.y / pixelRadius, 0);
        return new Vector3(axisNormalizedToPixelRadius.x, 0, axisNormalizedToPixelRadius.y);
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