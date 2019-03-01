using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public GameObject characterAgentPrefab;
    CharacterAgent charAgent;

    Rigidbody body;

    public Material selectedMaterial;
    public Material deselectedMaterial;

    private enum State
    {
        WALKING,
        CLIMBING_1,
        CLIMBING_2,
        CLIMBING_END,
        SWIMMING
    }
    private State state;

    private float climbSpeed = 3.5f;

    public enum ClimbDirection
    {
        UP,
        DOWN
    }
    private State postClimbState;

    private Transform climbPoint1;
    private Transform climbPoint2;
    private Transform climbPointEnd;

    void Start()
    {
        GameObject agent = Instantiate(characterAgentPrefab, transform.localPosition, transform.rotation);
        agent.transform.parent = GameObject.FindWithTag("ShipHitbox").transform;
        charAgent = agent.GetComponent<CharacterAgent>();

        SetState(State.WALKING);
        body = GetComponent<Rigidbody>();
    }

    /*
     * Returns true if point is reached.
     */
    private bool MoveTo(Vector3 point, float speed)
    {
        if (Vector3.Distance(point, transform.position) < 0.1)
        {
            return true;
        }

        Vector3 moveDirection = (point - transform.position).normalized;
        body.MovePosition(transform.position + Time.deltaTime * moveDirection * speed);

        return false;
    }

    private void SetState(State newState)
    {
        if (state == State.WALKING)
        {
            if (charAgent != null)
            {
                Destroy(charAgent.gameObject);
                charAgent = null;
            }
        }
        if (newState == State.WALKING)
        {
            GameObject agent = Instantiate(characterAgentPrefab, transform.localPosition, transform.rotation);
            agent.transform.parent = GameObject.FindWithTag("ShipHitbox").transform;
            charAgent = agent.GetComponent<CharacterAgent>();
        }
        state = newState;
    }

    private void FixedUpdate()
    {
        if (state == State.WALKING)
        {
            transform.localPosition = charAgent.transform.localPosition;
        }
        else if (state == State.CLIMBING_1)
        {
            //ShowAndroidToastMessage("Climbing 1");
            if (MoveTo(climbPoint1.position, climbSpeed))
            {
                SetState(State.CLIMBING_2);
            }
        }
        else if (state == State.CLIMBING_2)
        {
            //ShowAndroidToastMessage("Climbing 2");
            if (MoveTo(climbPoint2.position, climbSpeed))
            {
                SetState(State.CLIMBING_END);
            }
        }
        else if (state == State.CLIMBING_END)
        {
            //ShowAndroidToastMessage("Climbing 3");
            if (MoveTo(climbPointEnd.position, climbSpeed))
            {
                SetState(postClimbState);
            }
        }
        else if (state == State.SWIMMING)
        {
            // TODO
        }
    }

    public void OnTargetReached(Target target)
    {
        // TODO
    }

    public void ClimbLadder(Transform point1, Transform point2, Transform pointEnd, ClimbDirection dir)
    {
        climbPoint1 = point1;
        climbPoint2 = point2;
        climbPointEnd = pointEnd;

        SetState(State.CLIMBING_1);
        if (dir == ClimbDirection.UP)
        {
            postClimbState = State.WALKING;
        }
        else if (dir == ClimbDirection.DOWN)
        {
            postClimbState = State.SWIMMING;
        }
    }

    public bool IsClimbing()
    {
        return state == State.CLIMBING_1 || state == State.CLIMBING_2 || state == State.CLIMBING_END;
    }

    private Color highlightOffset = new Color(0.0f, 0.5f, 0.5f);

    public void Select()
    {
        GetComponent<Renderer>().sharedMaterial = selectedMaterial;
    }

    public void Deselect()
    {
        GetComponent<Renderer>().sharedMaterial = deselectedMaterial;
    }

    public void SetTarget(Target target)
    {
        Vector3 targetPosition = target.transform.localPosition;
        if (charAgent != null) {
            charAgent.SetDestination(targetPosition);
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
