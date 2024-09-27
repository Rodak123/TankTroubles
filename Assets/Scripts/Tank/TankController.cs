using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Tank), typeof(TankDamage))]
public class TankController : MonoBehaviour
{
    private float accelerationInput = 0;
    private float steeringInput = 0;

    private float rotationAngle = 0;

    private float forwardVelocity = 0f;

    private Tank tank;
    private Rigidbody2D rb2D;
    private TankDamage damage;

    [SerializeField, Range(-1, 1)] private float steerDirectionFactor = 0;

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
    [SerializeField] private float noTrackSpeedFactor = 0.2f;
    [SerializeField] private float noTrackMaxSpeedFactor = 0.66f;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();

        tank = GetComponent<Tank>();

        damage = GetComponent<TankDamage>();

        damage.OnLeftTrackDestroyed += (object sender, EventArgs args) => steerDirectionFactor -= noTrackSpeedFactor;
        damage.OnRightTrackDestroyed += (object sender, EventArgs args) => steerDirectionFactor += noTrackSpeedFactor;
    }

    private void Update()
    {
        UpdateInput();
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
            rb2D.drag = Mathf.Lerp(rb2D.drag, maxDrag, Time.fixedDeltaTime * maxDragLerpSpeed);
        else rb2D.drag = dragWhileDriving;

        //Caculate how much "forward" we are going in terms of the direction of our velocity
        forwardVelocity = Vector2.Dot(transform.up, rb2D.velocity);

        float maxSpeed = this.maxSpeed * (damage.AreTracksDestroyed() ? noTrackMaxSpeedFactor : 1);

        //Limit so we cannot go faster than the max speed in the "forward" direction
        if (forwardVelocity > maxSpeed && accelerationInput > 0)
            return;

        if (forwardVelocity < -maxSpeed * reverseSpeedFactor && accelerationInput < 0)
            return;

        //Limit so we cannot go faster in any direction while accelerating
        if (rb2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
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
            minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(rb2D.velocity.magnitude * minVelocityForTurningFactor);
        }

        float alteredSteeringInput = steeringInput + steerDirectionFactor * (damage.AreTracksDestroyed() ? noTrackSpeedFactor : 1);

        //Update the rotation angle based on input
        rotationAngle -= alteredSteeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;

        //Apply steering by rotating the car object
        rb2D.MoveRotation(rotationAngle);
    }

    private void ReduceDrift()
    {
        //Get forward and right velocity of the car
        Vector2 forwardVelocityVec = transform.up * Vector2.Dot(rb2D.velocity, transform.up);
        Vector2 rightVelocityVec = transform.right * Vector2.Dot(rb2D.velocity, transform.right);

        //Kill the orthogonal velocity (side velocity) based on how much the car should drift. 
        rb2D.velocity = forwardVelocityVec + rightVelocityVec * driftFactor;
    }

    private void UpdateInput()
    {
        Vector2 inputVector = tank.Input.GetAxisInput();
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    public float GetVelocityMagnitude()
    {
        return rb2D.velocity.magnitude;
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
