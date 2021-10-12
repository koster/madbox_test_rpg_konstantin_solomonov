using UnityEngine;

public class Hero : MonoBehaviour
{
    public float speed = 5f;
    public float sensitivity = 0.5f;
    public float motionDamping = 0.9f;
    public float rotationDamping = 0.25f;
    
    Vector3 origin;
    bool input;
    Vector3 moveVector;

    Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            input = true;
            origin = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            input = false;
        }

        animator.SetFloat("MoveInput", moveVector.magnitude);
    }

    void FixedUpdate()
    {
        if (input)
        {
            var axis = origin - Input.mousePosition;
            axis = Vector3.ClampMagnitude(axis * sensitivity, 1f);
            axis = new Vector3(axis.x, 0, axis.y);

            moveVector = Vector3.Lerp(moveVector, axis, motionDamping);
        }
        else
        {
            moveVector *= motionDamping;
        }

        transform.position += moveVector * Time.fixedDeltaTime * speed;

        var rotationThreshold = 0.1f;
        if (moveVector.magnitude > rotationThreshold)
            transform.forward = Vector3.Lerp(transform.forward, -moveVector.normalized, rotationDamping);
    }
}