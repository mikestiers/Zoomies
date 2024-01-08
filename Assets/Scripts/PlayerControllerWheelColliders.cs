using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerWheelColliders : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody vehicleRigidbody;
    public WheelCollider frontLeftWheelCollider, frontRightWheelCollider, rearLeftWheelCollider, rearRightWheelCollider;
    public Transform frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;
    public GameObject brakeLights;
    public TrailRenderer[] skidTrails = new TrailRenderer[2];

    [Header("Capatilities")]
    public float motorForce = 1000f;
    public float steeringAngle = 30f; // Max angle for steering, cars are typically 25-45
    public int driveType = 1; // 1 = all wheel, 2 = front wheel, 3 = rear wheel
    public float maxSpeed = 200.0f;

    [Header("HUD")]
    public Text accelerationText;
    public Text speedText;
    public Text brakingText;
    public Text rpmText;
    public Text gearText;
    public Text turnAngleText;

    private Vector3 centerOfMass = new Vector3(0, 0.5f, 0);
    private int currentGear = 1;
    private float[] gearRpmThresholds = { 3000f, 6000f, 9000f };
    private float[] gearTorqueMultipliers = { 1f, 0.7f, 0.5f };

    WheelCollider[] wheelColliders = new WheelCollider[4];
    Transform[] wheelTransforms = new Transform[4];
    Vector3[] lastWheelPositions = new Vector3[4];

    private void Start()
    {
        wheelColliders[0] = frontLeftWheelCollider;
        wheelColliders[1] = frontRightWheelCollider;
        wheelColliders[2] = rearLeftWheelCollider;
        wheelColliders[3] = rearRightWheelCollider;

        wheelTransforms[0] = frontLeftWheel;
        wheelTransforms[1] = frontRightWheel;
        wheelTransforms[2] = rearLeftWheel;
        wheelTransforms[3] = rearRightWheel;

        lastWheelPositions[0] = frontLeftWheel.transform.position;
        lastWheelPositions[1] = frontRightWheel.transform.position;
        lastWheelPositions[2] = rearLeftWheel.transform.position;
        lastWheelPositions[3] = rearRightWheel.transform.position;

        vehicleRigidbody.centerOfMass = centerOfMass;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCar();
        }
        HUDManager.singleton.UpdateSpeedometer(currentGear, vehicleRigidbody.velocity.magnitude);
    }
    void FixedUpdate()
    {
        float accelerationInput = Input.GetAxis("Vertical");
        float steeringInput = Input.GetAxis("Horizontal");
        
        // Forward and reverse
        ApplyMotorTorque(accelerationInput);
        
        // Braking
        if (Input.GetButton("Jump"))
            ApplyBrake(10000f);
        else
            ApplyBrake(0f);

        // Steering
        if (Mathf.Abs(steeringInput) > 0.1f)// && vehicleRigidbody.velocity.magnitude > 0.1 && accelerationInput == 0)
        {
            ApplySteering(steeringInput);
            //ApplyMotorTorque(1);
        }
        else
            CorrectSteering();

        // Gear shifting
        ShiftGears();

        // Cap speed to max speed
        vehicleRigidbody.velocity = Vector3.ClampMagnitude(vehicleRigidbody.velocity, maxSpeed);
    }

    void ApplyMotorTorque(float accelerationInput)
    {
        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            wheelCollider.motorTorque = accelerationInput * motorForce * gearTorqueMultipliers[currentGear - 1];
            accelerationText.text ="Acceleration: " + accelerationInput.ToString("F2");
        }

        RotateWheels();
        UpdateHUD();
    }

    void ApplySteering(float steeringInput)
    {
        float targetSteeringAngle = steeringAngle * steeringInput;
        frontLeftWheelCollider.steerAngle = targetSteeringAngle;
        frontRightWheelCollider.steerAngle = targetSteeringAngle;

        // Update the collider rotation
        frontLeftWheelCollider.transform.localEulerAngles = new Vector3(0, targetSteeringAngle, 0);
        frontRightWheelCollider.transform.localEulerAngles = new Vector3(0, targetSteeringAngle, 0);

        // Update the wheel transform rotation
        frontLeftWheel.transform.localEulerAngles = new Vector3(0, targetSteeringAngle, 0);
        frontRightWheel.transform.localEulerAngles = new Vector3(0, targetSteeringAngle, 0);

        RotateWheels();
        UpdateHUD();

        turnAngleText.text = "Turn Angle: " + targetSteeringAngle.ToString("F2");
    }

    void CorrectSteering()
    {
        frontLeftWheelCollider.steerAngle = 0;
        frontRightWheelCollider.steerAngle = 0;
    }

    void RotateWheels()
    {
        // Rotate the wheels
        for (int i = 0; i < 4; i++) // using C style loop because need a convenient index
        {
            float distanceMoved = Vector3.Distance(transform.position, lastWheelPositions[i]);
            lastWheelPositions[i] = transform.position;
            float rotationAngle = (distanceMoved / (2 * Mathf.PI * wheelColliders[i].radius)) * 360f;
            wheelTransforms[i].Rotate(rotationAngle, 0, 0); // Rotate around the X-axis
        }

        UpdateHUD();
    }

    void UpdateHUD()
    {
        speedText.text = "Speed: " + vehicleRigidbody.velocity.magnitude.ToString("F2");
    }

    void ResetCar()
    {
        // Set the rotation to be upright.
        transform.rotation = Quaternion.identity;

        // Raise the car a bit in case it is stuck in something
        //newPosition.y += 2f;  // Just an arbitrary number to raise it
        //transform.position = newPosition;

        // Reset the car to the starting point
        Vector3 newPosition = transform.position;
        transform.position = new Vector3(141.800003f, 46.2999992f, -318.700012f);
        //transform.rotation = new Vector3(67.5265045f, 0f, 0f);

        // Reset momentum
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void ShiftGears()
    {
        // Should probably average out RPM or something instead of just checking one wheel
        float currentRpm = wheelColliders[0].rpm;
        if (currentRpm > gearRpmThresholds[currentGear - 1] && currentGear < gearRpmThresholds.Length)
        {
            // Shift up
            currentGear++;
            //ApplyBrake(500f);
        }
        else if (currentGear > 1 && currentRpm < gearRpmThresholds[currentGear - 2])
        {
            // Shift down
            currentGear--;
            //ApplyBrake(0f);
        }

        rpmText.text = "RPM: " + currentRpm.ToString("F2");
        gearText.text = "Gear: " + currentGear.ToString("F2");
    }

    private void ApplyBrake(float brakeForce)
    {
        brakeLights.SetActive(brakeForce > 0);
        foreach (TrailRenderer skidTrail in skidTrails)
        {
            skidTrail.emitting = brakeForce > 0;
        }
        foreach (var wheelCollider in wheelColliders)
        {
            wheelCollider.brakeTorque = brakeForce;
            brakingText.text = "Braking: " + brakeForce.ToString("F2");
        }
    }
}