using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    [Header("Car settings")]
    //public FixedJoystick Joystick;
    public float driftFactor = 0.95f;
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;
    public float maxSpeed = 20;

    float accelerationInput = 0;
    float steeringInput = 0;
    float velocityVsUp = 0;
    float rotationAngle = 0;

    Rigidbody2D CARRigidbody2d;

    void Awake() {
        CARRigidbody2d = GetComponent<Rigidbody2D>();

    }


    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        ApplyEngineForce();

        KillOrthogonalVelocity();

        ApplySteering();

    }

    void ApplyEngineForce() {

        velocityVsUp = Vector2.Dot(transform.up, CARRigidbody2d.velocity);

        if (velocityVsUp > maxSpeed && accelerationInput > 0)
            return;
        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
            return;
        if (CARRigidbody2d.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
            return;

        if (accelerationInput == 0)
            CARRigidbody2d.drag = Mathf.Lerp(CARRigidbody2d.drag, 3.0f, Time.fixedDeltaTime * 3);
        else CARRigidbody2d.drag = 0;
        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

        CARRigidbody2d.AddForce(engineForceVector, ForceMode2D.Force);
    }
    

    
	
    void ApplySteering() {
        

        float minSpeedBeforeAllowTurningFactor = (CARRigidbody2d.velocity.magnitude / 8);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);
        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;

        CARRigidbody2d.MoveRotation(rotationAngle);
    }

    void KillOrthogonalVelocity() {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(CARRigidbody2d.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(CARRigidbody2d.velocity, transform.right);

        CARRigidbody2d.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    float GetLateralVelocity() {
        return Vector2.Dot(transform.right, CARRigidbody2d.velocity);
    }

    public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;
        if (accelerationInput < 0 && velocityVsUp > 0) {
            isBraking = true;
            return true;
        }
        if (Mathf.Abs(GetLateralVelocity()) > 4.0f)
            return true;

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Terrain"))
        {
            maxSpeed = 3;
            driftFactor = 0.95f;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Terrain"))
        {
             
            maxSpeed = 3;
            driftFactor = 0.95f;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Terrain"))
        {
            maxSpeed = 11;
            driftFactor = 0f;
        }
    }




    public void SetInputVector(Vector2 inputVector) {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

}
