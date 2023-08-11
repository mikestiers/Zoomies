using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Rigidbody vehicleRigidbody;
    public Vector3 centerOfMassOffset;
    public List<WheelCollider> wheelColliders;
    public Text accelerationText;
    public Text speedText;
    public Text brakeDecelerationText;
    public Text turnAngleText;
    public Text turnRadiusText;
    public bool isGrounded = true;
    public float maxSpeed = 200.0f;
    public float acceleration = 60.0f;
    public float brakeDeceleration = 30.0f;
    public float turnSensitivity = 0.5f;
    public float minTurnRadius = 5.0f;

    void FixedUpdate()
    {
        float accelerationInput = Input.GetAxis("Vertical");
        float brakeInput = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;
        float steeringInput = Input.GetAxis("Horizontal");

        accelerationText.text = "Acceleration: " + accelerationInput.ToString();
        speedText.text = "Speed: " + vehicleRigidbody.velocity.magnitude;
        brakeDecelerationText.text = "Brake Deceleration: " + brakeInput.ToString();

        HandleInput(accelerationInput, brakeInput, steeringInput);

        if (!isGrounded)
            vehicleRigidbody.centerOfMass = centerOfMassOffset;
    }

    private void HandleInput(float accelerationInput, float brakeInput, float steeringInput)
    {
        // Apply acceleration
        if (accelerationInput > 0.1 && isGrounded)
        {
            Debug.Log("Forward");
            Vector3 forwardForce = transform.forward * accelerationInput * acceleration;
            vehicleRigidbody.AddForce(forwardForce, ForceMode.Acceleration);
            // Apply forward movement based on turn angle (only when steering)
            if (Mathf.Abs(steeringInput) > 0.1f)
            {
                float newYRotation = steeringInput * 100 * Time.deltaTime;
                transform.Rotate(0, newYRotation, 0);
            }
        }
        // Reverse
        if (accelerationInput < 0 && isGrounded)
        {
            Debug.Log("Reverse");
            Vector3 reverseForce = -transform.forward * accelerationInput * acceleration;
            vehicleRigidbody.AddForce(reverseForce, ForceMode.Acceleration);
            // Apply reverse movement based on turn angle (only when steering)
            if (Mathf.Abs(steeringInput) > 0.1f)
            {
                float newYRotation = steeringInput * 100 * Time.deltaTime;
                transform.Rotate(0, newYRotation, 0);
            }
        }
        if (brakeInput > 0 && isGrounded)
        {
            vehicleRigidbody.AddForce(-vehicleRigidbody.velocity.normalized * brakeInput * brakeDeceleration);
        }

        // Cap speed to max speed
        vehicleRigidbody.velocity = Vector3.ClampMagnitude(vehicleRigidbody.velocity, maxSpeed);

        // Calculate turn angle based on steering input and speed
        //float turnAngle = steeringInput * turnSensitivity * vehicleRigidbody.velocity.magnitude;
        //turnAngleText.text = "Turn Angle: " + turnAngle.ToString();

        // Calculate turn radius based on speed
        //float turnRadius = Mathf.Max(minTurnRadius, vehicleRigidbody.velocity.magnitude / 10.0f);
        //turnRadiusText.text = "Turn Radius: " + turnRadius.ToString();

        // Apply turning logic
        //Quaternion turnRotation = Quaternion.AngleAxis(turnAngle, Vector3.up);
        //vehicleRigidbody.MoveRotation(vehicleRigidbody.rotation * turnRotation);    
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Collision: " + gameObject.name);
    //    isGrounded = true;
    //    //if (wheelColliders.Contains(collision.collider as WheelCollider) && collision.gameObject.tag == "Street")
    //    //{
    //    //    Debug.Log("Wheel Collision: " + gameObject.name);
    //    //    isGrounded = true;
    //    //}
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    Debug.Log("Exiting Collision" + gameObject.name);
    //    isGrounded = false;
    //}
}