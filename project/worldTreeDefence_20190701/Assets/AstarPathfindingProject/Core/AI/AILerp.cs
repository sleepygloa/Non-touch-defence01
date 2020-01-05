using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

/** Linearly interpolating movement script.
 * This movement script will follow the path exactly, it uses linear interpolation to move between the waypoints in the path.
 * This is desirable for some types of games.
 * It also works in 2D.
 *
 * Recommended setup:
 *
 * This depends on what type of movement you are aiming for.
 * If you are aiming for movement where the unit follows the path exactly (you are likely using a grid or point graph)
 * the default settings on this component should work quite well, however I recommend that you adjust the StartEndModifier
 * on the Seeker component: set the 'Exact Start Point' field to 'NodeConnection' and the 'Exact End Point' field to 'SnapToNode'.
 *
 * If you on the other hand want smoother movement I recommend adding the Simple Smooth Modifier to the GameObject as well.
 * You may also want to tweak the #rotationSpeed.
 *
 * \ingroup movementscripts
 */
[RequireComponent(typeof(Seeker))]
[AddComponentMenu("Pathfinding/AI/AISimpleLerp (2D,3D generic)")]
[HelpURL("http://arongranberg.com/astar/docs/class_a_i_lerp.php")]
public class AILerp : MonoBehaviour
{
    /** Determines how often it will search for new paths.
	 * If you have fast moving targets or AIs, you might want to set it to a lower value.
	 * The value is in seconds between path requests.
	 */
    public float repathRate = 0.5F; //길찾기 빈도.AI를 빠르게 할려면 수치를 낮출것.

    /** Target to move towards.
	 * The AI will try to follow/move towards this target.
	 * It can be a point on the ground where the player has clicked in an RTS for example, or it can be the player object in a zombie game.
	 */
    public Transform target; //타겟.

    /** Enables or disables searching for paths.
	 * Setting this to false does not stop any active path requests from being calculated or stop it from continuing to follow the current path.
	 * \see #canMove
	 */
    public bool canSearch = true; //길찾기 기능 OnOff

    /** Enables or disables movement.
	 * \see #canSearch */
    public bool canMove = true; //이동 OnOff

    /** Speed in world units */
    public float speed = 3; //속도. 

    /** If true, the AI will rotate to face the movement direction */
    public bool enableRotation = true; //이동 방향 회전.

    /** If true, rotation will only be done along the Z axis */
    public bool rotationIn2D = false; //2D용 회전. z축만 관여한다.

    /** How quickly to rotate */
    public float rotationSpeed = 10; // 회전 속도.

    /** If true, some interpolation will be done when a new path has been calculated.
	 * This is used to avoid short distance teleportation.
	 */
    public bool interpolatePathSwitches = true; // true면 두경로 간에 보간을 하게 된다. 짧은거리 순간이동 버그 방지.

    /** How quickly to interpolate to the new path */
    public float switchPathInterpolationSpeed = 5; // 얼마나 빨리 새로운길과 보간을 하게 될지 속도.

    /** Cached Seeker component */
    protected Seeker seeker;

    /** Cached Transform component */
    protected Transform tr;

    /** Time when the last path request was sent */
    protected float lastRepath = -9999;

    /** Current path which is followed */
    protected ABPath path;

    /** Current index in the path which is current target */
    protected int currentWaypointIndex = 0;

    /** How far the AI has moved along the current segment */
    protected float distanceAlongSegment = 0;

    /** True if the end-of-path is reached.
	 * \see TargetReached */
    public bool targetReached { get; private set; }

    /** Only when the previous path has been returned should be search for a new path */
    protected bool canSearchAgain = true;

    /** When a new path was returned, the AI was moving along this ray.
	 * Used to smoothly interpolate between the previous movement and the movement along the new path.
	 * The speed is equal to movement direction.
	 */
    protected Vector3 previousMovementOrigin;
    protected Vector3 previousMovementDirection;
    protected float previousMovementStartTime = -9999;

    /** Holds if the Start function has been run.
	 * Used to test if coroutines should be started in OnEnable to prevent calculating paths
	 * in the awake stage (or rather before start on frame 0).
	 */
    private bool startHasRun = false;

    public bool isUseMultiTargetPath { get; set; }

    public GridNode gridNode { get; set; }
    //unit
    public BaseController unitController;

    public int move_type = 0;//0:지상 1:공중 2: 공중(레이어가 위이다.) - 0아니면 모두 공중취급한다. - 메타파일기준.

    //Path Tag 
    //일반적인 지나갈수 있는 tag는 0번이다(tagname : Basic Ground)
    //건물등으로 막힌 tag는 1번이다(tagname : obstacle)

    int[] tag_array;

    /** Initializes reference variables.
	 * If you override this function you should in most cases call base.Awake () at the start of it.
	 * */
    protected virtual void Awake()
    {
        //This is a simple optimization, cache the transform component lookup
        tr = transform;

        seeker = GetComponent<Seeker>();

        // Tell the StartEndModifier to ask for our exact position
        // when post processing the path
        // This is important if we are using prediction and
        // requesting a path from some point slightly ahead of us
        // since then the start point in the path request may be far
        // from our position when the path has been calculated.
        // This is also good because if a long path is requested, it may
        // take a few frames for it to be calculated so we could have
        // moved some distance during that time
        seeker.startEndModifier.adjustStartPoint = () =>
        {
            return tr.position;
        };
    }

    /** Starts searching for paths.
	 * If you override this function you should in most cases call base.Start () at the start of it.
	 * \see OnEnable
	 * \see RepeatTrySearchPath
	 */
    //protected virtual void Start()
    //{
    //    startHasRun = true;
    //    OnEnable();
    //}

    //  /** Run at start and when reenabled.
    //* Starts RepeatTrySearchPath.
    //*
    //* \see Start
    //*/
    //protected virtual void OnEnable()
    //{
    //    lastRepath = -9999;
    //    canSearchAgain = true;

    //    if (startHasRun)
    //    {
    //        // Make sure we receive callbacks when paths complete
    //        seeker.pathCallback += OnPathComplete;

    //        StartCoroutine(RepeatTrySearchPath());
    //    }
    //}

    public void Init()
    {
        lastRepath = -9999;
        canSearchAgain = true;
        seeker.pathCallback += OnPathComplete;
        //StartCoroutine(RepeatTrySearchPath());
    }

    public void SetTag()
    {

        seeker.traversableTags = 1 << 0;

    }

    public void OnDisable()
    {
        // Abort calculation of path
        if (seeker != null && !seeker.IsDone()) seeker.GetCurrentPath().Error();

        // Release current path
        if (path != null) path.Release(this);
        path = null;

        // Make sure we receive callbacks when paths complete
        seeker.pathCallback -= OnPathComplete;

        bInit = false;
    }

    /** Tries to search for a path every #repathRate seconds.
	 * \see TrySearchPath
	 */
    protected IEnumerator RepeatTrySearchPath()
    {
        while (true)
        {
            float v = TrySearchPath();
            yield return new WaitForSeconds(v);
        }
    }

    /** Tries to search for a path.
	 * Will search for a new path if there was a sufficient time since the last repath and both
	 * #canSearchAgain and #canSearch are true and there is a target.
	 *
	 * \returns The time to wait until calling this function again (based on #repathRate)
	 */
    public float TrySearchPath()
    {
        if (Time.time - lastRepath >= repathRate && canSearchAgain && canSearch && target != null)
        {
            SearchPath();
            return repathRate;
        }
        else
        {
            float v = repathRate - (Time.time - lastRepath);
            return v < 0 ? 0 : v;
        }
    }

    /** Requests a path to the target.
	 * Some inheriting classes will prevent the path from being requested immediately when
	 * this function is called, for example when the AI is currently traversing a special path segment
	 * in which case it is usually a bad idea to search for a new path.
	 */
    public virtual void SearchPath()
    {
        //ForceSearchPath();
    }

    bool bInit = false;

    /// <summary>
    /// 타겟트랜스폼을 받아서 패스파인딩 수행.
    /// </summary>
    /// <param name="targetTr"></param> 
    public void SearchPath(Vector3 targetpos)
    {
        if (bInit == false)
        {
            Init();
            bInit = true;
        }

        if (target == null)
        {
            ForceSearchPath(targetpos);
        }
        else
        {
            target.position = targetpos;
            ForceSearchPath(targetpos);
        }

    }

    // float time = 0;
    // bool bSearchStart = false;

    /** Requests a path to the target.
	 * Bypasses 'is-it-a-good-time-to-request-a-path' checks.
	 */
    public virtual void ForceSearchPath(Vector3 pos)
    {
        //TODO: [0320-건웅임시] target 트랜스폼은 일단 사용하지 않는다. postion을 직접 넣는다.
        //target 트랜스폼은 멀티타겟에서만 쓰고있음.
        //  if (target == null) throw new System.InvalidOperationException("Target is null");

        //time = 0;
        //bSearchStart = true;

        lastRepath = Time.time;
        // This is where we should search to
        var targetPosition = pos;
        var currentPosition = GetFeetPosition();

        // 타겟 위치가 바뀌면 이동한다.
        //if (!targetPosition.Equals(currentPosition))
        //{
        //    canMove = true;
        //    canSearchAgain = true;
        //    this.lastRepath = -9999;
        //    seeker.startEndModifier.adjustStartPoint = () =>
        //    {
        //        return tr.position;
        //    };
        //}

        // If we are following a path, start searching from the node we will reach next
        // this can prevent odd turns right at the start of the path
        if (path != null && path.vectorPath.Count > 1)
        {
            currentPosition = path.vectorPath[currentWaypointIndex];
            path = null;
        }

        canSearchAgain = false;

        //Alternative way of requesting the path
        //ABPath p = ABPath.Construct (currentPosition, targetPosition, null);
        //seeker.StartPath (p);

        // We should search from the current position
        if (isUseMultiTargetPath == false)      //단일 타겟 패쓰.
        {
            seeker.StartPath(currentPosition, targetPosition);
        }
        else
        {
            Vector3[] endPoints = getChildTransformPositions(target);       //멀티 타겟 패쓰.
            seeker.StartMultiTargetPath(transform.position, endPoints, true, OnMultiPathComplete);
        }

    }
    public void OnMultiPathComplete(Path p)
    {
        //Debug.Log("Multipath searching Completed");

        if (p.error)
        {
            Debug.LogWarning("the multipath returned error:" + p.errorLog);
            return;
        }

        MultiTargetPath path = p as MultiTargetPath;
        if (path == null)
        {
            Debug.LogWarning("Can not find multi Target Path");
            return;
        }
        drawPaths = path.vectorPaths;

        //
    }

    List<Vector3>[] drawPaths = new List<Vector3>[0];
    private void OnDrawGizmos()
    {
        List<Vector3>[] paths = drawPaths;

        for (int i = 0; i < paths.Length; i++)
        {
            List<Vector3> pt = paths[i];

            if (pt == null)
            {
                Debug.LogWarning("Path Number " + i + " cound not be found");
                continue;
            }
            for (int j = 0; j < pt.Count - 1; j++)
            {
                Debug.DrawLine(pt[j], pt[j + 1], AstarMath.IntToColor(i, 0.5f));
            }

        }
    }

    /** The end of the path has been reached.
	 * If you want custom logic for when the AI has reached it's destination
	 * add it here
	 * You can also create a new script which inherits from this one
	 * and override the function in that script.
	 */
    public virtual void OnTargetReached()
    {
        //도착하면 호출되는 곳.
        if (GetComponent<RVO2DController>() != null)
        {
            this.canMove = false;
            if (path != null) path.Release(this);
            path = null;
        }
    }

    /** Called when a requested path has finished calculation.
	 * A path is first requested by #SearchPath, it is then calculated, probably in the same or the next frame.
	 * Finally it is returned to the seeker which forwards it to this function.\n
	 */
    public virtual void OnPathComplete(Path _p)
    {
        //  bSearchStart = false;

        ABPath p = _p as ABPath;

        if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special path types");

        canSearchAgain = true;

        // Claim the new path
        // This is used for path pooling
        p.Claim(this);

        // Path couldn't be calculated of some reason.
        // More info in p.errorLog (debug string)
        if (p.error)
        {
            p.Release(this);
            //unit
            if (unitController != null)
            {
                unitController.TargetFindandPathSearch(EntityType.Defense);
            }
            return;
        }

        if (interpolatePathSwitches)
        {
            ConfigurePathSwitchInterpolation();
        }

        // Release the previous path
        // This is used for path pooling
        if (path != null) path.Release(this);

        // Replace the old path
        path = p;

        // Just for the rest of the code to work, if there is only one waypoint in the path
        // add another one
        if (path.vectorPath != null && path.vectorPath.Count == 1)
        {
            path.vectorPath.Insert(0, GetFeetPosition());
        }

        targetReached = false;
        //unit
        if(unitController != null)
        {
            unitController.OnTargetFind();
        }

        // Reset some variables
        ConfigureNewPath();
    }

    protected virtual void ConfigurePathSwitchInterpolation()
    {
        bool previousPathWasValid = path != null && path.vectorPath != null && path.vectorPath.Count > 1;

        bool reachedEndOfPreviousPath = false;

        if (previousPathWasValid)
        {
            reachedEndOfPreviousPath = currentWaypointIndex == path.vectorPath.Count - 1 && distanceAlongSegment >= (path.vectorPath[path.vectorPath.Count - 1] - path.vectorPath[path.vectorPath.Count - 2]).magnitude;
        }

        if (previousPathWasValid && !reachedEndOfPreviousPath)
        {
            List<Vector3> vPath = path.vectorPath;

            // Make sure we stay inside valid ranges
            currentWaypointIndex = Mathf.Clamp(currentWaypointIndex, 1, vPath.Count - 1);

            // Current segment vector
            Vector3 segment = vPath[currentWaypointIndex] - vPath[currentWaypointIndex - 1];
            float segmentLength = segment.magnitude;

            // Find the approximate length of the path that is left on the current path
            float approximateLengthLeft = segmentLength * Mathf.Clamp01(1 - distanceAlongSegment);
            for (int i = currentWaypointIndex; i < vPath.Count - 1; i++)
            {
                approximateLengthLeft += (vPath[i + 1] - vPath[i]).magnitude;
            }

            previousMovementOrigin = GetFeetPosition();
            previousMovementDirection = segment.normalized * approximateLengthLeft;
            previousMovementStartTime = Time.time;
        }
        else
        {
            previousMovementOrigin = Vector3.zero;
            previousMovementDirection = Vector3.zero;
            previousMovementStartTime = -9999;
        }
    }

    public virtual Vector3 GetFeetPosition()
    {
        return tr.position;
    }

    /** Finds the closest point on the current path.
	 * Sets #currentWaypointIndex and #lerpTime to the appropriate values.
	 */
    protected virtual void ConfigureNewPath()
    {
        var points = path.vectorPath;

        var currentPosition = GetFeetPosition();

        // Find the closest point on the new path
        // to our current position
        // and initialize the path following variables
        // to start following the path from that point
        float bestDistanceAlongSegment = 0;
        float bestDist = float.PositiveInfinity;
        Vector3 bestDirection = Vector3.zero;
        int bestIndex = 1;

        for (int i = 0; i < points.Count - 1; i++)
        {
            float factor = VectorMath.ClosestPointOnLineFactor(points[i], points[i + 1], currentPosition);
            factor = Mathf.Clamp01(factor);
            Vector3 point = Vector3.Lerp(points[i], points[i + 1], factor);
            float dist = (currentPosition - point).sqrMagnitude;

            if (dist < bestDist)
            {
                bestDist = dist;
                bestDirection = points[i + 1] - points[i];
                bestDistanceAlongSegment = factor * bestDirection.magnitude;
                bestIndex = i + 1;
            }
        }

        currentWaypointIndex = bestIndex;
        distanceAlongSegment = bestDistanceAlongSegment;

        if (interpolatePathSwitches && switchPathInterpolationSpeed > 0.01f)
        {
            var correctionFactor = Mathf.Max(-Vector3.Dot(previousMovementDirection.normalized, bestDirection.normalized), 0);
            distanceAlongSegment -= speed * correctionFactor * (1f / switchPathInterpolationSpeed);
        }
    }

    public Vector3 AIdirection = Vector3.forward;

    protected virtual void Update()
    {

        if (canMove)
        {
            Vector3 nextPos = CalculateNextPosition();
            tr.position = nextPos;
        }

    }

    /** Calculate the AI's next position (one frame in the future).
	 * \param direction The direction of the segment the AI is currently traversing. Not normalized.
	 */
    protected virtual Vector3 CalculateNextPosition()
    {
        if (path == null || path.vectorPath == null || path.vectorPath.Count == 0)
        {
            AIdirection = Vector3.zero;
            return tr.position;
        }

        List<Vector3> vPath = path.vectorPath;

        // Make sure we stay inside valid ranges
        currentWaypointIndex = Mathf.Clamp(currentWaypointIndex, 1, vPath.Count - 1);

        // Current segment vector
        Vector3 segment = vPath[currentWaypointIndex] - vPath[currentWaypointIndex - 1];
        float segmentLength = segment.magnitude;

        AIdirection = segment.normalized;

        // Move forwards
        distanceAlongSegment += Time.deltaTime * speed;

        //   Debug.Log(Time.deltaTime);

        // Pick the next segment if we have traversed the current one completely
        if (distanceAlongSegment >= segmentLength && currentWaypointIndex < vPath.Count - 1)
        {
            float overshootDistance = distanceAlongSegment - segmentLength;

            while (true)
            {
                currentWaypointIndex++;

                // Next segment vector
                Vector3 nextSegment = vPath[currentWaypointIndex] - vPath[currentWaypointIndex - 1];
                float nextSegmentLength = nextSegment.magnitude;

                if (overshootDistance <= nextSegmentLength || currentWaypointIndex == vPath.Count - 1)
                {
                    segment = nextSegment;
                    segmentLength = nextSegmentLength;
                    distanceAlongSegment = overshootDistance;
                    break;
                }
                else
                {
                    overshootDistance -= nextSegmentLength;
                }
            }
        }

        if (distanceAlongSegment >= segmentLength && currentWaypointIndex == vPath.Count - 1)
        {
            if (!targetReached)
            {
                OnTargetReached();
            }
            targetReached = true;
        }

        if (path == null)
        {
            //Debug.Log("path is null");
            return tr.position;
        }
        // Find our position along the path using a simple linear interpolation
        Vector3 positionAlongCurrentPath = segment * Mathf.Clamp01(segmentLength > 0 ? distanceAlongSegment / segmentLength : 1) + vPath[currentWaypointIndex - 1];

        if (interpolatePathSwitches)
        {
            // Find the approximate position we would be at if we
            // would have continued to follow the previous path
            Vector3 positionAlongPreviousPath = previousMovementOrigin + Vector3.ClampMagnitude(previousMovementDirection, speed * (Time.time - previousMovementStartTime));

            // Use this to debug
            //Debug.DrawLine (previousMovementOrigin, positionAlongPreviousPath, Color.yellow);

            return Vector3.Lerp(positionAlongPreviousPath, positionAlongCurrentPath, switchPathInterpolationSpeed * (Time.time - previousMovementStartTime));
        }
        else
        {
            return positionAlongCurrentPath;
        }
    }

    Vector3[] getChildTransformPositions(Transform parent)
    {
        Vector3[] ret = new Vector3[parent.childCount];
        int idx = 0;
        foreach (Transform child in parent)
        {
            ret[idx] = child.position;
            idx++;
        }
        return ret;
    }

    public void PathRelease()
    {
        if (this.path != null)
        {
            if (path != null) path.Release(this);
            path = null;
        }
    }
}
