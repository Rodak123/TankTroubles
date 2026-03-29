using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Tank), typeof(TankDamage))]
public class TankController : MonoBehaviour
{

    [Header("Drag")]
    [SerializeField] private float maxDrag = 5;
    [SerializeField] private float maxDragLerpSpeed = 2;
    [SerializeField] private float dragWhileDriving = 0;

    [Header("Engine")]
    [SerializeField] private float accelerationFactor = 30;
    [SerializeField] private float maxSpeed = 20;
    [SerializeField] private float reverseSpeedFactor = 0.5f;

    [Header("Steering")]
    [SerializeField] private float turnFactor = 3.5f;
    [SerializeField] private float driftFactor = 0.95f;

    [Header("Other")]
    [SerializeField] private float minVelocityForTurningFactor = 0.5f;

    [Header("Damage")]
    [SerializeField] private float noTrackSpeedFactor = 0.2f;
    [SerializeField] private float noTrackMaxSpeedFactor = 0.66f;
    [SerializeField] private float noTracksSteerFactor = 0.5f;

    private Tank tank;
    private Rigidbody2D rb2D;
    private TankDamage damage;

    private float steerDirectionFactor = 0;

    [Header("Debug Info")]
    [SerializeField, ReadOnly] private float accelerationInput = 0;
    [SerializeField, ReadOnly] private float steeringInput = 0;

    [SerializeField, ReadOnly] private float rotationAngle = 0;

    [SerializeField, ReadOnly] private float forwardVelocity = 0f;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();

        tank = GetComponent<Tank>();

        damage = GetComponent<TankDamage>();

        damage.OnLeftTrackDestroyed += (object sender, TankDamage.TankDamageState damageState) => steerDirectionFactor -= noTrackSpeedFactor;
        damage.OnRightTrackDestroyed += (object sender, TankDamage.TankDamageState damageState) => steerDirectionFactor += noTrackSpeedFactor;

        rotationAngle = transform.eulerAngles.z;
    }

    private void Update()
    {
        if (tank.IsActive())
            UpdateInput();
        else
        {
            steeringInput = 0;
            accelerationInput = 0;
        }
    }

    private void FixedUpdate()
    {
        ApplyEngineForce();

        ReduceDrift();

        ApplySteering();
    }

    private void ApplyEngineForce()
    {
        if (damage.IsTankDestroyed())
            return;

        if (accelerationInput == 0)
            rb2D.linearDamping = Mathf.Lerp(rb2D.linearDamping, maxDrag, Time.fixedDeltaTime * maxDragLerpSpeed);
        else rb2D.linearDamping = dragWhileDriving;

        //Caculate how much "forward" we are going in terms of the direction of our velocity
        forwardVelocity = Vector2.Dot(transform.up, rb2D.linearVelocity);

        float maxSpeed = this.maxSpeed * (damage.AreTracksDestroyed() ? noTrackMaxSpeedFactor : 1);

        //Limit so we cannot go faster than the max speed in the "forward" direction
        if (forwardVelocity > maxSpeed && accelerationInput > 0)
            return;

        if (forwardVelocity < -maxSpeed * reverseSpeedFactor && accelerationInput < 0)
            return;

        //Limit so we cannot go faster in any direction while accelerating
        if (rb2D.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
            return;

        //Create a force for the engine
        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor * (damage.AreTracksDestroyed() ? noTrackSpeedFactor : 1);

        //Apply force and pushes the car forward
        rb2D.AddForce(engineForceVector, ForceMode2D.Force);
    }

    private void ApplySteering()
    {
        if (damage.IsTankDestroyed() || (steeringInput == 0 && accelerationInput == 0))
        {
            rb2D.angularVelocity = 0;
            return;
        }

        float minSpeedBeforeAllowTurningFactor = 1;
        if (minVelocityForTurningFactor != 0)
        {
            //Limit the cars ability to turn when moving slowly
            minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(rb2D.linearVelocity.magnitude * minVelocityForTurningFactor);
        }

        float damagedSteeringInput = steerDirectionFactor * (damage.AreTracksDestroyed() ? noTracksSteerFactor : 1) * (GetForwardVelocity() >= 0 ? 1 : -1);

        //Update the rotation angle based on input
        rotationAngle -= (steeringInput + damagedSteeringInput) * turnFactor * minSpeedBeforeAllowTurningFactor;

        //Apply steering by rotating the car object
        rb2D.MoveRotation(rotationAngle);
    }

    private void ReduceDrift()
    {
        //Get forward and right velocity of the car
        Vector2 forwardVelocityVec = transform.up * Vector2.Dot(rb2D.linearVelocity, transform.up);
        Vector2 rightVelocityVec = transform.right * Vector2.Dot(rb2D.linearVelocity, transform.right);

        //Kill the orthogonal velocity (side velocity) based on how much the car should drift. 
        rb2D.linearVelocity = forwardVelocityVec + rightVelocityVec * driftFactor;
    }

    private void UpdateInput()
    {
        Vector2 inputVector = tank.Input.GetAxisInput();
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    public float GetVelocityMagnitude()
    {
        return rb2D.linearVelocity.magnitude;
    }

    public float GetForwardVelocity()
    {
        return forwardVelocity;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }
}
