using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;

    private float maxXDistance;
    private float maxXVel;
    private float jumpTime;
    private float quitTime;
    private bool sphereJumped;
    private Vector3 startPos;

    /*
     * Exemplary EVALUATION PARAMETERS
     */
    public float forceXMultiplier;
    public float forceJumpMultiplier;
    public int mass;

    /*
     * Exemplary OBJECTIVE VALUES
     */
    [HideInInspector]
    public float highestPosition;
    [HideInInspector]
    public float totalDistanceTravelled;
    [HideInInspector]
    public bool quitDueToTimeConstraint;
    [HideInInspector]
    public float totalForceApplied;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        startPos = transform.position;
        rb.mass = mass;

        // initialize objective values
        highestPosition = 0f;
        totalDistanceTravelled = 0f;
        quitDueToTimeConstraint = false;
        totalForceApplied = 0f;

        // set timings
        jumpTime = 5f;
        quitTime = 10f;

        // maximum distance in x direction until end of plane
        maxXDistance = 100f;

        // maximum velocity in x direction
        maxXVel = 20f;

        sphereJumped = false;
    }

    private void FixedUpdate()
    {
        // track highest y-position of sphere
        if (transform.position.y > highestPosition)
            highestPosition = transform.position.y;

        // don't add force if the velocity of the sphere is larger than maxXVel
        if (rb.velocity.x <= maxXVel)
        {
            rb.AddForce(Vector3.right * forceXMultiplier, ForceMode.Force);
            totalForceApplied += forceXMultiplier;
        }

        // make sphere jump
        if (Time.time >= jumpTime && !sphereJumped)
        {
            rb.AddForce(Vector3.up * forceJumpMultiplier, ForceMode.Impulse);
            sphereJumped = true;
        }
    }

    void Update()
    {
        if (Time.time >= quitTime || transform.position.x >= maxXDistance)
        {
            if (Time.time >= quitTime)
                quitDueToTimeConstraint = true;

            // total distance travelled by the sphere
            totalDistanceTravelled = Vector3.Distance(startPos, transform.position);

            // quit
            Application.Quit();
        }
    }
}
