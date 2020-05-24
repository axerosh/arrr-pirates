using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Crewman : MonoBehaviour {
    private bool showCrewmanHint = true;
    public TextMeshPro hintText;
    public GameObject carriedTreasure;

    public GameObject characterAgentPrefab;
    CharacterAgent charAgent;

    Rigidbody body;

    private enum State
    {
        NONE,
        WALKING,
        CLIMBING_1,
        CLIMBING_2,
        CLIMBING_END,
        SWIMMING
    }
    private State state = State.NONE;

    private bool m_isCarryingTreasure = false;
    private bool isCarryingTreasure
    {
        get {return m_isCarryingTreasure; }
        set {
            if (m_isCarryingTreasure == value) return;
            m_isCarryingTreasure = value;
            carriedTreasure.SetActive(value);
        }
     }

    private float climbSpeed = 3.5f;
    private float swimSpeed = 5.0f;
    private const float ALREADY_AT_LADDER_DISTANCE = 2.5f;

    public enum ClimbDirection
    {
        UP,
        DOWN
    }
    private State postClimbState;

    private Transform climbPoint1;
    private Transform climbPoint2;
    private Transform climbPointEnd;

    private Transform swimTarget;
    private Transform walkTarget;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        Selectable selectable = GetComponent<Selectable>();
        selectable.onTargetSet = SetTargetExternal;
        selectable.onSelected = OnSelected;
        SetState(State.WALKING);
    }

    private Vector3 lastMoveDirection = Vector3.zero;
    private Transform lastMoveTarget;
    /*
     * Returns true if point is reached.
     */
    private bool MoveTo(Transform target, float speed)
    {
        Vector3 moveDirection = (target.position - transform.position).normalized;
        
        // Check target passed
        if (lastMoveTarget != null
            && lastMoveTarget.gameObject == target.gameObject // lastMoveDirection is set
            && Vector3.Dot(lastMoveDirection, moveDirection) < 0) // Passed the target
        {
            return true;
        }
        lastMoveTarget = target;
        lastMoveDirection = moveDirection;

        body.MovePosition(transform.position + Time.deltaTime * moveDirection * speed);
        return false;
    }

    private Vector3 ToShipLocal(Vector3 point)
    {
        return GameObject.FindWithTag("Ship").transform.InverseTransformPoint(point);
    }

    /*
     * Change state. This is done here outside FixedUpdate to avoid heavy load.
     */
    private void SetState(State newState)
    {
        if (newState != state)
        {
            // Previous state
            switch (state)
            {
                case State.WALKING:
                    if (charAgent != null)
                    {
                        Debug.Log("Destroy agent");
                        Destroy(charAgent.gameObject);
                        charAgent = null;
                    }
                    break;

                case State.SWIMMING:
                    transform.parent = GameObject.FindWithTag("Ship").transform;
                    break;

                default:
                    break;
            }

            // New state
            switch (newState)
            {
                case State.WALKING:
                    swimTarget = null;

                    // Snap to navmesh
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(transform.localPosition, out hit, 10.0f, NavMesh.AllAreas))
                    {
                        transform.localPosition = hit.position;

                        // Init nav agent
                        GameObject agent = Instantiate(characterAgentPrefab, hit.position, transform.rotation);
                        agent.transform.parent = GameObject.FindWithTag("ShipHitbox").transform;
                        charAgent = agent.GetComponent<CharacterAgent>();
                    }
                    
                    if (charAgent != null)
                    {
                        if (walkTarget == null || !isCarryingTreasure)
                        {
                            // Roaming
                            charAgent.SetRoaming();
                        }
                        else
                        {
                            // Walking to target
                            charAgent.SetDestination(ToShipLocal(walkTarget.position));
                            Debug.Log("Walk to target");
                        }
                    }
                    break;

                case State.SWIMMING:
                    walkTarget = null;
                    transform.parent = GameObject.FindWithTag("SeaFloor").transform;
                    break;

                default:
                    break;
            }

            state = newState;
        }
    }

    private void LookAt2D(Vector3 targetPosition)
    {
        Vector3 delta = targetPosition - transform.position;
        transform.forward = new Vector3(delta.x, 0.0f, delta.z);
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.WALKING:
                transform.localPosition = charAgent.transform.localPosition;
                transform.localRotation = charAgent.transform.localRotation;
                break;

            case State.CLIMBING_1:
                if (MoveTo(climbPoint1, climbSpeed))
                {
                    SetState(State.CLIMBING_2);
                }
                LookAt2D(climbPoint2.position);
                break;

            case State.CLIMBING_2:
                if (MoveTo(climbPoint2, climbSpeed))
                {
                    SetState(State.CLIMBING_END);
                }
                LookAt2D(climbPoint2.position);
                break;

            case State.CLIMBING_END:
                if (MoveTo(climbPointEnd, climbSpeed))
                {
                    SetState(postClimbState);
                }
                LookAt2D(climbPoint1.position);
                break;

            case State.SWIMMING:
                if (swimTarget != null)
                {
                    if (MoveTo(swimTarget, swimSpeed)) {
                        swimTarget = null;
                    }
                    else
                    {
                        LookAt2D(swimTarget.position);
                    }
                }
                else
                {
                    SetTarget(GameObject.FindWithTag("Dropoff"));

                    var entryPoints = GameObject.FindObjectsOfType<EntryPoint>();
                    EntryPoint closestEntry = null;
                    float closestDistance = float.MaxValue;
                    foreach (EntryPoint entry in entryPoints)
                    {
                        float distance = Vector3.Distance(transform.position - new Vector3(0.0f, 1.0f, 0.0f), entry.transform.position);
                        if (distance < closestDistance)
                        {
                            closestEntry = entry;
                            closestDistance = distance;
                        }
                    }
                    if (closestEntry != null && closestDistance <= ALREADY_AT_LADDER_DISTANCE)
                    {
                        // Already at entry point
                        closestEntry.GetComponentInParent<Ladder>().ClimbUp(this);
                        Debug.Log("Immediately climb up ladder");
                    };
                }
                break;

            default:
                break;
        }
    }

    public void OnTreasureReached(Treasure treasure)
    {
        if (treasure != null)
        {
            GameObject.Destroy(treasure.gameObject);
            isCarryingTreasure = true;
            SetTarget(GameObject.FindWithTag("Dropoff"));
        }
    }

    public void OnDropoffReached()
    {
        if (isCarryingTreasure)
        {
            var player = GameObject.FindObjectOfType<ArrrController>();
            player.AddScore(1);
            isCarryingTreasure = false;
        }
        charAgent.SetRoaming();
    }

    public void ClimbLadder(Transform point1, Transform point2, Transform pointEnd, ClimbDirection dir)
    {
        if (dir == ClimbDirection.DOWN)
        {
            if (swimTarget == null)
            {
                // Accidental
                return;
            }
            postClimbState = State.SWIMMING;
        }
        else if (dir == ClimbDirection.UP)
        {
            if (walkTarget == null)
            {
                // Accidental
                return;
            }
            postClimbState = State.WALKING;
        }

        SetState(State.CLIMBING_1);
        climbPoint1 = point1;
        climbPoint2 = point2;
        climbPointEnd = pointEnd;
    }

    public bool IsClimbing()
    {
        return state == State.CLIMBING_1 || state == State.CLIMBING_2 || state == State.CLIMBING_END;
    }

    public void SetTargetExternal(GameObject target)
    {
        if (!isCarryingTreasure)
        {
            SetTarget(target);
        }
    }

    private void SetTarget(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        // Underwater target
        if (target.GetComponentInParent<Treasure>()) {
            //Deselect any previously selected chest.
            if (swimTarget) {
                swimTarget.GetComponent<SelectorIndicatorController>()?.SetSelected(false);
            }
            //Set chest as new target.
            swimTarget = target.transform;
            swimTarget.GetComponent<SelectorIndicatorController>()?.SetSelected(true);

            // Go to ladder
            if (state == State.WALKING)
            {
                Transform closestEntryPoint = null;
                float closestDistance = float.MaxValue;
                var deckEntryPoints = GameObject.FindGameObjectsWithTag("DeckEntryPoint");
                foreach (var entryPoint in deckEntryPoints)
                {
                    EntryPoint exitPoint = entryPoint.GetComponent<EntryPoint>().exit;
                    float distanceDeck = Vector3.Distance(entryPoint.transform.position, transform.position);
                    float distanceWater = Vector3.Distance(exitPoint.transform.position, target.transform.position);
                    float distance = distanceDeck + distanceWater;
                    if (distance < closestDistance)
                    {
                        closestEntryPoint = entryPoint.transform;
                        closestDistance = distance;
                    }
                }
                if (closestEntryPoint != null) {
                    if (closestDistance < ALREADY_AT_LADDER_DISTANCE)
                    {
                        closestEntryPoint.GetComponentInParent<Ladder>().ClimbDown(this);
                        Debug.Log("Immediately climb down ladder");
                    }
                    else if (charAgent != null)
                    {
                        charAgent.SetDestination(ToShipLocal(closestEntryPoint.position));
                        Debug.Log("Walk to ladder " + ToShipLocal(closestEntryPoint.position));
                    }
                }
            }
        }

        // On-ship target
        else
        {
            walkTarget = target.transform;

            // Go there now
            if (state == State.WALKING)
            {
                if (charAgent != null)
                {
                    Debug.Log("Walk to target " + ToShipLocal(target.transform.position));
                    charAgent.SetDestination(ToShipLocal(target.transform.position));
                }
            }

            // Swim to ladder
            else if (state == State.SWIMMING)
            {
                Transform closestEntryPoint = null;
                float closestDistance = float.MaxValue;
                var waterEntryPoints = GameObject.FindGameObjectsWithTag("WaterEntryPoint");
                foreach (var entryPoint in waterEntryPoints)
                {
                    EntryPoint exitPoint = entryPoint.GetComponent<EntryPoint>().exit;
                    float distanceWater = Vector3.Distance(entryPoint.transform.position, transform.position);
                    float distanceDeck = Vector3.Distance(exitPoint.transform.position, target.transform.position);
                    float distance = distanceWater + distanceDeck;
                    if (distance < closestDistance)
                    {
                        closestEntryPoint = entryPoint.transform;
                        closestDistance = distance;
                    }
                }
                if (closestEntryPoint != null)
                {
                    swimTarget = closestEntryPoint;
                    Debug.Log("Swim to Ladder");
                }
            }
        }
    }

    public void DisableClickMeHint() {
        showCrewmanHint = false;
        hintText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when this crewman is selected.
    /// </summary>
    void OnSelected() {
        // The first time a crewman is selected
        if (showCrewmanHint) {
            GameObject.FindWithTag("UI").GetComponentInChildren<GamePlayUI>().DisplayCrewmanHint();
            DisableClickMeHint();
        }
    }

    private void ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
