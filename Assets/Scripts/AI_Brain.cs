using UnityEditor;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class AI_Brain : MonoBehaviour
{
    private NavMeshAgent Agent;

    //Movement States
    public Type movementType = Type.Stationary;

    public enum Type
    {
        Stationary,
        Patrol,
        Roaming,
        MoveToPosition
    }

    //Action State
    public State actionState = State.Idle;

    public enum State
    {
        Idle,
        Chasing,
    }

    //Stationary
    public bool returnToStartAfterTargetLoss = true;
    private Vector3 startingPos;

    //Patrol
    public Transform[] patrolPath;
    public bool loopToStartOfPatrol = false;
    public bool returnToClosestPointAfterTargetLoss = true;

    public int pointOnPath = -1;
    private bool returningToStartOfPatrol = false;

    //Random Patrol
    public float patrolRange = 10f;
    public bool staticRange = true;     //In Random Patrol, does the range move with the player

    public Vector3 staticRangeOffset;

    //Move To Position
    public Transform transformTarget;
    public bool updateTransformPosition = true;

    //Searching Parameters
    public float sightRange = 10f;
    public float sightAngle = 45f;
    public float absoluteSightRange = 2f;
    public float absoluteSightAngle = 45f;

    private LayerMask TargetMask;
    private LayerMask IgnoreTargetMask = -1;

    public float targetDeadzone = 4f;
    public bool enableAbsoluteSightRange = true;

    //Movement Vectors
    public Transform Target;
    public Vector3 movingToPoint;
    private Vector3 targetDestination;

    //Speed Parameters
    private float characterSpeed;
    private float baseCharacterSpeed;

    public float caughtSpeedFactor = 1.25f;

    //Settings
    public bool visionGizmo = true;
    public bool movementGizmo = true;

    public float gizmoScale = 1f;

    private readonly float POINT_DISTANCE_MIN = 0.3f;   //how close does the character have to be to have reached the point

    private void Awake()
    {
        //Editor Check to ensure patrolPath array is filled, or if on MoveToPos has a valid transform
#if UNITY_EDITOR
        if (movementType == Type.Patrol && patrolPath.Length == 0)
        {
            Debug.LogError($"[{gameObject.name} {gameObject.GetInstanceID()}] is set to patrol but has no points to patrol!");
            enabled = false;
            return;
        }

        if (movementType == Type.MoveToPosition && transformTarget == null)
        {
            Debug.LogError($"[{gameObject.name} {gameObject.GetInstanceID()}] is set to move towards a transform but has no transfrom attached!");
            enabled = false;
            return;
        }
#endif

        IgnoreTargetMask &= ~(1 << LayerMask.NameToLayer("Target"));           //sets layer to ignore "Target" layer
        IgnoreTargetMask &= ~(1 << LayerMask.NameToLayer("Ignore Raycast"));

        TargetMask |= 1 << LayerMask.NameToLayer("Target");                    //sets layer to only "Target" layer

        Agent = GetComponent<NavMeshAgent>();

        characterSpeed = Agent.speed;
        baseCharacterSpeed = characterSpeed;
    }

    private void OnEnable()
    {
        //Reseting values
        startingPos = transform.position;
        movingToPoint = startingPos;

        SetCharacterSpeed(baseCharacterSpeed);

        Target = null;
        actionState = State.Idle;

        if (movementType == Type.Patrol)
            StartPatrolling();
    }

    private void FixedUpdate()
    {
        SearchingForTarget();

        switch (actionState)
        {
            case State.Idle:
                SetMovement();
                break;
            case State.Chasing:
                ChasingTarget();
                break;
        }
    }

    #region Patroling

    private void StartPatrolling()
    {
        FindClosestPatrolPoint();

        Move(patrolPath[pointOnPath].position);
    }

    private void Patrol()
    {
        if (pointOnPath == patrolPath.Length - 1)
        {
            if (loopToStartOfPatrol)
                pointOnPath = 0;
            else
            {
                returningToStartOfPatrol = true;
                pointOnPath--;
            }
        }
        else
        {
            if (returningToStartOfPatrol)
                pointOnPath--;
            else
                pointOnPath++;
        }

        if (pointOnPath == 0 && returningToStartOfPatrol)
            returningToStartOfPatrol = false;

        Move(patrolPath[pointOnPath].position);
    }

    private void RandomPatrol()
    {
        int iterations = 1;

        while (true)
        {
            Vector3 randomPoint;

            if (staticRange)
                randomPoint = FindRandomPointWithin(startingPos + staticRangeOffset, patrolRange);
            else
                randomPoint = FindRandomPointWithin(transform.position + transform.forward * (patrolRange / 2), patrolRange);

            if (CanReachPoint(randomPoint))
            {
                Move(randomPoint);
                break;
            }

            iterations++;

            if (iterations > 10)
            {
                Debug.LogWarning("Cannot find suitable point to head to!!!");
                break;
            }
        }
    }

    #endregion

    #region Moving

    private void SetMovement()
    {
        if (movementType != Type.Stationary && !DistanceCheck(movingToPoint, POINT_DISTANCE_MIN))
            return;
    
        switch (movementType)
        {
            case Type.Patrol:
                Patrol();
                break;
            case Type.Roaming:
                RandomPatrol();
                break;
            case Type.MoveToPosition:
                if (updateTransformPosition)
                    Move(transformTarget.position);
                break;
        }
    }

    private void Move(Vector3 destination)
    {
        targetDestination = destination;

        if (movingToPoint == targetDestination)
            return;

        movingToPoint = FindGroundPoint(destination);

        if (!CanReachPoint(movingToPoint))
        {
            Debug.Log($"[{gameObject.name}] cannot reach destination! {movingToPoint}");

            movingToPoint = FindRandomPointWithin(movingToPoint, 2f);
        }

        Agent.SetDestination(movingToPoint);
    }

    #endregion

    #region AI State

    private void SearchingForTarget()
    {
        Collider[] hitColliders = new Collider[1];
        int hit = Physics.OverlapSphereNonAlloc(transform.position, sightRange, hitColliders, TargetMask);

        if (hit != 0)
        {
            Vector3 foundTarget = hitColliders[0].transform.position;

            Vector3 dir = (foundTarget - transform.position).normalized;

            // finds target if it is witin the angle. or if it is within the absolute range (if enabled), checking if both within the minimum range and if it is not too high/low angle to the character
            if (Vector3.Angle(transform.forward, dir) <= sightAngle ||
                enableAbsoluteSightRange && DistanceCheck(foundTarget, absoluteSightRange) && Vector3.Angle(transform.up, dir) >= absoluteSightAngle)
            {
                if (AreaClearToPoint(foundTarget, dir))
                {
                    Target = hitColliders[0].transform;
                    actionState = State.Chasing;

                    return;
                }
            }
        }

        //Target not found
        if (actionState == State.Chasing)
            ReturnToIdle();
    }

    private void ChasingTarget()
    {
        //Moves character towards target, if too close only rotates in targets direction

        if (!DistanceCheck(Target.position, targetDeadzone))
        {
            ChangeCharacterSpeed(characterSpeed * caughtSpeedFactor);
            Move(Target.position);
        }
        else
        {
            ChangeCharacterSpeed(0f);

            Quaternion targetRotation = Quaternion.LookRotation((Target.position - transform.position).normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, characterSpeed * 10 * Time.deltaTime);
        }
    }


    public void SetSearchingArea(Vector3 point, float size)
    {
        Vector3 searchPoint = FindRandomPointWithin(point, size);

        Move(searchPoint);
    }

    private void ReturnToIdle()
    {
        Vector3 returnPoint;

        if (movementType == Type.Patrol)
        {
            if (returnToClosestPointAfterTargetLoss)
                FindClosestPatrolPoint();

            returnPoint = patrolPath[pointOnPath].position;
        }
        else
            returnPoint = startingPos;

        actionState = State.Idle;

        ChangeCharacterSpeed(characterSpeed);

        Target = null;

        if (movementType == Type.Stationary && !returnToStartAfterTargetLoss)
            return;

        Move(returnPoint);
    }

    #endregion

    #region Point Checks

    public bool DistanceCheck(Vector3 point, float distance)
    {
        return (point - transform.position).sqrMagnitude < distance;
    }

    public bool AreaClearToPoint(Vector3 target, Vector3 dir)
    {
        RaycastHit[] rayhit = new RaycastHit[1];
        Ray ray = new(transform.position, dir);

        int hit = Physics.RaycastNonAlloc(ray, rayhit, Vector3.Distance(transform.position, target), IgnoreTargetMask, QueryTriggerInteraction.Ignore);

        return hit == 0;
    }

    public Vector3 FindGroundPoint(Vector3 origin)
    {
        RaycastHit[] rayhit = new RaycastHit[1];
        Ray ray = new(origin, Vector3.down);

        Physics.RaycastNonAlloc(ray, rayhit, 100f, IgnoreTargetMask, QueryTriggerInteraction.Ignore);

        return rayhit[0].point;
    }

    public bool CanReachPoint(Vector3 target)
    {
        NavMeshPath path = new();

        return Agent.CalculatePath(target, path) && path.status == NavMeshPathStatus.PathComplete;
    }

    public bool FacingPoint(Vector3 point)
    {
        return Vector3.Dot(transform.forward, (point - transform.position).normalized) < 0.9f;
    }

    private void FindClosestPatrolPoint()
    {
        float closestPoint = (patrolPath[0].position - transform.position).sqrMagnitude;
        int closestPointPosition = 0;

        for (int i = 1; i < patrolPath.Length; i++)
        {
            float distance = (patrolPath[i].position - transform.position).sqrMagnitude;

            if (closestPoint > distance)
            {
                closestPoint = distance;
                closestPointPosition = i;
            }
        }

        pointOnPath = closestPointPosition;
    }

    private Vector3 FindRandomPointWithin(Vector3 origin, float radius)
    {
        //rework so radii greater than twice the Agent.height are split up for performance

        Vector3 randomDirection = Random.insideUnitSphere * radius + origin;
        Vector3 finalPosition = Vector3.zero;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, 1))
            finalPosition = hit.position;

        return finalPosition;
    }

    #endregion

    #region Speed

    public bool IsMoving()
    {
        return Agent.velocity.magnitude > 0f;
    }

    public void SetCharacterSpeed(float speed)
    {
        characterSpeed = speed;

        ChangeCharacterSpeed(characterSpeed);
    }

    private void ChangeCharacterSpeed(float speed)
    {
        Agent.speed = speed;
    }

    #endregion

    #region Gizmo

    private void OnDrawGizmosSelected()
    {
        if (visionGizmo)
        {
            switch (actionState)
            {
                case State.Idle:
                    Gizmos.color = Color.blue;
                    break;
                case State.Chasing:
                    Gizmos.color = Color.red;
                    break;
            }

            Gizmos.DrawWireSphere(transform.position, sightRange);

            Gizmos.color = Color.black;

            Gizmos.DrawWireSphere(transform.position, targetDeadzone);

            if (!Mathf.Approximately(sightAngle, 180f))
            {
                Gizmos.color = Color.magenta;
                float lineAngle = sightAngle + 90f - transform.eulerAngles.y;

                Vector3[] sightLines = new Vector3[2];

                for (int i = 0; i < sightLines.Length; i++)
                {
                    sightLines[i] = new(
                        sightRange * Mathf.Cos(Mathf.PI * lineAngle / 180f),
                        0f,
                        sightRange * Mathf.Sin(Mathf.PI * lineAngle / 180f)
                    );

                    lineAngle -= sightAngle + sightAngle;
                }


                Gizmos.DrawLine(sightLines[0] + transform.position, transform.position);
                Gizmos.DrawLine(sightLines[1] + transform.position, transform.position);
            }
        }

        if (movementGizmo)
        {
            Gizmos.color = Color.white;

            if (movementType == Type.Patrol && patrolPath != null && patrolPath.Length != 0)
            {
                for (int i = 1; i < patrolPath.Length; i++)
                {
                    Gizmos.DrawLine(patrolPath[i - 1].position, patrolPath[i].position);
                    Gizmos.DrawWireCube(patrolPath[i].position, Vector3.one * gizmoScale);
                }

                if (loopToStartOfPatrol)
                {
                    Gizmos.DrawWireCube(patrolPath[0].position, Vector3.one * gizmoScale);
                    Gizmos.DrawLine(patrolPath[0].position, patrolPath[^1].position);
                }
                else
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireCube(patrolPath[0].position, Vector3.one * gizmoScale);
                    Gizmos.DrawWireCube(patrolPath[^1].position, Vector3.one * gizmoScale);
                }

                if (pointOnPath != -1)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(patrolPath[pointOnPath].position, Vector3.one * gizmoScale);
                }
            }
            else if (movementType == Type.Roaming)
            {
                if (staticRange)
                {
                    if (Application.isPlaying)
                        Gizmos.DrawWireSphere(startingPos + staticRangeOffset, patrolRange);
                    else
                        Gizmos.DrawWireSphere(transform.position + staticRangeOffset, patrolRange);
                }
                else
                    Gizmos.DrawWireSphere(transform.position + transform.forward * (patrolRange / 2), patrolRange);

                if (actionState == State.Idle)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(movingToPoint, 0.5f * gizmoScale);
                }
            }
            else if (movementType == Type.MoveToPosition && transformTarget != null)
                Gizmos.DrawWireSphere(transformTarget.position, 1f * gizmoScale);
        }
    }

    #endregion
}

#region Editor
#if UNITY_EDITOR

[CustomEditor(typeof(AI_Brain)), CanEditMultipleObjects]
public class AI_Brain_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (AI_Brain)target;

        EditorUtility.SetDirty(script);

        script.movementType = (AI_Brain.Type)EditorGUILayout.EnumPopup("Type", script.movementType);
        EditorGUILayout.Space(10);

        switch (script.movementType)
        {
            case AI_Brain.Type.Stationary:

                script.returnToStartAfterTargetLoss = EditorGUILayout.Toggle("Return To Start After Target Loss", script.returnToStartAfterTargetLoss);

                break;
            case AI_Brain.Type.Patrol:

                SerializedProperty pathArray = serializedObject.FindProperty("patrolPath");
                EditorGUILayout.PropertyField(pathArray, includeChildren: true);
                if (pathArray.hasChildren)
                    serializedObject.ApplyModifiedProperties();

                script.loopToStartOfPatrol = EditorGUILayout.Toggle("Loop Back To Start Of Patrol", script.loopToStartOfPatrol);
                script.returnToClosestPointAfterTargetLoss = EditorGUILayout.Toggle("Return To Closest Point", script.returnToClosestPointAfterTargetLoss);

                break;
            case AI_Brain.Type.Roaming:

                script.patrolRange = EditorGUILayout.Slider("Patrol Range", script.patrolRange, 1f, 30f);
                script.staticRange = EditorGUILayout.Toggle("Static Range", script.staticRange);

                if (script.staticRange)
                    script.staticRangeOffset = EditorGUILayout.Vector3Field("Static Range Offset", script.staticRangeOffset);

                break;
            case AI_Brain.Type.MoveToPosition:

                script.transformTarget = EditorGUILayout.ObjectField("Target", script.transformTarget, typeof(Transform), true) as Transform;
                script.updateTransformPosition = EditorGUILayout.Toggle("Update Position", script.updateTransformPosition);

                break;
        }


        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Searching Parameters", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        script.sightRange = EditorGUILayout.Slider("Sight Range", script.sightRange, 0.1f, 15f);
        script.sightAngle = EditorGUILayout.Slider("Sight Angle", script.sightAngle, 1f, 180f);

        script.targetDeadzone = EditorGUILayout.Slider("Deadzone size", script.targetDeadzone, 0f, 10f);

        EditorGUILayout.Space(10);

        if (script.sightAngle != 180f)
            script.enableAbsoluteSightRange = EditorGUILayout.Toggle("Absolute Sight", script.enableAbsoluteSightRange);

        if (script.enableAbsoluteSightRange)
        {
            script.absoluteSightRange = EditorGUILayout.Slider("Absolute Range", script.absoluteSightRange, 0f, script.sightRange - 1f);
            script.absoluteSightAngle = EditorGUILayout.Slider("Above Angle Minimum", script.absoluteSightAngle, 1f, 180f);
        }
        

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Speed Multiplier", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        script.caughtSpeedFactor = EditorGUILayout.Slider("Caught Speed", script.caughtSpeedFactor, 1f, 2f);


        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        script.visionGizmo = EditorGUILayout.Toggle("Vision Gizmos", script.visionGizmo);
        script.movementGizmo = EditorGUILayout.Toggle("Movement Gizmos", script.movementGizmo);
        
        if (script.visionGizmo || script.movementGizmo)
            script.gizmoScale = EditorGUILayout.Slider("Gizmo Scale", script.gizmoScale, 0f, 1f);

        EditorGUILayout.Space(10);
        GUI.enabled = false;

        script.actionState = (AI_Brain.State)EditorGUILayout.EnumPopup("State", script.actionState);

        if (script.movementType == AI_Brain.Type.Patrol)
            EditorGUILayout.IntField("Point On Path", script.pointOnPath);

        EditorGUILayout.Vector3Field("Moving To Position", script.movingToPoint);

        GUI.enabled = true;
    }
}

#endif
    #endregion