using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallClimber : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] public float hor;
    [SerializeField] public float ver;

    [Header("Plugins")]
    [SerializeField] private PlayerCon pc;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform handTrans;
    [SerializeField] private Climbingsort currentSort;

    [Header("Settings")]
    [SerializeField] private float climbForce;
    [SerializeField] private float climbRange;
    [SerializeField] private float smallerEdge = 0.25f;
    [SerializeField] private float maxAngle = 30f; //keep equal to or below 45
    [SerializeField] private float minDist;
    [SerializeField] private float coolDown = 0.15f;
    [SerializeField] private float jumpForce;

    [Header("Offsets")]
    [SerializeField] private Vector3 verHandOffset;
    [SerializeField] private Vector3 horHandOffset;
    [SerializeField] private Vector3 fallHandOffset;

    [Header("Position")]
    [SerializeField] private Vector3 raycastPos;
    [SerializeField] private Vector3 targetPoint;
    [SerializeField] private Vector3 targetNorm;
    
    [Header("Layer Masks")]
    [SerializeField] private LayerMask spotLayer;
    [SerializeField] private LayerMask curSpotLayer;
    [SerializeField] private LayerMask checkLayersObstacle;
    [SerializeField] private LayerMask checkLayersReachable;

    [SerializeField] private float lastTime;
    [SerializeField] private float beginDist;

    private RaycastHit hit;
    private Quaternion oldRot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerCon>();
    }
    public void Move(float h, float v)
    {
        hor = h;
        ver = v;
    }

    private void Update()
    {
        if(currentSort == Climbingsort.Walking && ver > 0)
        {
            StartClimbing();
        }
        if(currentSort == Climbingsort.Climbing)
        {
            Climb();
        }

        UpdateStats();

        if(currentSort == Climbingsort.ClimbingTowardPoint || currentSort == Climbingsort.ClimbingTowardPlateu)
        {
            MoveTowardsPoint();
        }
        if(currentSort == Climbingsort.Jumping || currentSort == Climbingsort.Falling)
        {
            Jumping();
        }
    }

    private void UpdateStats()
    {
        if(currentSort != Climbingsort.Walking && pc.isGrounded && currentSort != Climbingsort.ClimbingTowardPoint)
        {
            currentSort = Climbingsort.Walking;
            //ENABLE PC MOVEMENT
            rb.isKinematic = false;
            pc.enabled = true;
        }

        if(currentSort == Climbingsort.Walking && !pc.isGrounded)
        {
            currentSort = Climbingsort.Jumping;
        }

        if(currentSort == Climbingsort.Walking && hor != 0 || ver != 0)
        {
            CheckForClimbStart();
        }
    }
    
    private void StartClimbing()
    {
        if(Physics.Raycast(transform.position + transform.rotation * raycastPos, transform.forward, 1f)
        && Time.time - lastTime > coolDown && currentSort == Climbingsort.Walking)
        {
            if(currentSort == Climbingsort.Walking)
            {
                rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
            }
            lastTime = Time.time;
        }
    }

    private void Jumping()
    {
        if(rb.velocity.y < 0 && currentSort != Climbingsort.Falling)
        {
            currentSort = Climbingsort.Falling;
            oldRot = transform.rotation;
        }
        if(rb.velocity.y > 0 && currentSort != Climbingsort.Jumping)
        {
            currentSort = Climbingsort.Jumping;
        }

        if (currentSort == Climbingsort.Jumping)
        {
            CheckForSpots(handTrans.position + fallHandOffset, -transform.up, 0.1f, CheckingSort.normal);
        }

        if (currentSort == Climbingsort.Falling)
        {
            /* CHANGE THE VECTOR3 VALUE IF FALLING TO QUICK AND MISSING */
            CheckForSpots(handTrans.position + fallHandOffset + transform.rotation * new Vector3(0.02f, -0.6f,0f),
            -transform.up, 0.4f, CheckingSort.normal);
            transform.rotation = oldRot;
        }
    }

    private void Climb()
    {
        if(Time.time - lastTime > coolDown && currentSort == Climbingsort.Climbing)
        {
            /* IF MOVE UP */
            if (ver > 0)
            {
                CheckForSpots(handTrans.position + transform.rotation * verHandOffset + transform.up * climbRange, -transform.up, climbRange, CheckingSort.normal);
                
                if(currentSort != Climbingsort.ClimbingTowardPoint)
                {
                    CheckForPlateu();
                }
            }
            /* IF MOVE DOWN */
            if (ver < 0)
            {
                CheckForSpots(handTrans.position - transform.rotation * (verHandOffset + new Vector3(0,0.3f,0)), -transform.up, climbRange, CheckingSort.normal);
                
                if(currentSort != Climbingsort.ClimbingTowardPoint)
                {
                    rb.isKinematic = false;
                    pc.enabled = true;

                    //ENABLE CHAR CON
                    currentSort = Climbingsort.Falling;
                    oldRot = transform.rotation;
                }
            }

            if(hor != 0)
            {
                CheckForSpots(handTrans.position + transform.rotation * horHandOffset,
                transform.right * hor - transform.up /3.5f, climbRange/2, CheckingSort.normal);
                if (currentSort != Climbingsort.ClimbingTowardPoint)
                {
                    CheckForSpots(handTrans.position + transform.rotation * horHandOffset,
                    transform.right * hor - transform.up / 1.5f, climbRange / 3f, CheckingSort.normal);
                }
                if (currentSort != Climbingsort.ClimbingTowardPoint)
                {
                    CheckForSpots(handTrans.position + transform.rotation * horHandOffset,
                    transform.right * hor - transform.up / 6f, climbRange / 1.5f, CheckingSort.normal);
                }

                if(currentSort != Climbingsort.ClimbingTowardPoint)
                {
                    int hort = 0;
                    if(hor < 0)
                    {
                        hort = -1;
                    }
                    if (hor > 0)
                    {
                        hort = 1;
                    }
                    CheckForSpots(handTrans.position + transform.rotation * horHandOffset + transform.right * hort * smallerEdge/4,
                    transform.forward - transform.up *2, climbRange / 3, CheckingSort.turning);

                    if (currentSort != Climbingsort.ClimbingTowardPoint)
                    {
                        CheckForSpots(handTrans.position + transform.rotation * horHandOffset + transform.right * 0.2f,
                        transform.forward - transform.up * 2 + transform.right * hort/1.5f, climbRange / 3, CheckingSort.turning);
                    }
                }
            }
        }
    }

    private void CheckForSpots(Vector3 spotLoc, Vector3 dir, float range, CheckingSort sort )
    {
        bool foundSpot = false;
        //check if to the left
        if(Physics.Raycast(spotLoc - transform.right * smallerEdge / 2, dir, out hit, range, spotLayer))
        {
            if(Vector3.Distance(handTrans.position, hit.point) > minDist)
            {
                foundSpot = true;
                FindSpot(hit, sort);
            }
        }
        //check if to the right if not hte left
        if (!foundSpot)
        {
            if (Physics.Raycast(spotLoc + transform.right * smallerEdge / 2, dir, out hit, range, spotLayer))
            {
                if (Vector3.Distance(handTrans.position, hit.point) > minDist)
                {
                    foundSpot = true;
                    FindSpot(hit, sort);
                }
            }
        }
        //check to the right closer to the wall if not to the left
        if (!foundSpot)
        {
            if (Physics.Raycast(spotLoc + transform.right * smallerEdge / 2 + transform.forward * smallerEdge, dir, out hit, range, spotLayer))
            {
                if (Vector3.Distance(handTrans.position, hit.point) - smallerEdge/1.5f > minDist)
                {
                    foundSpot = true;
                    FindSpot(hit, sort);
                }
            }
        }
        //check to the left closer to the wall if not to the right and close to the wall
        if (!foundSpot)
        {
            if (Physics.Raycast(spotLoc - transform.right * smallerEdge / 2 + transform.forward * smallerEdge, dir, out hit, range, spotLayer))
            {
                if (Vector3.Distance(handTrans.position, hit.point) - smallerEdge / 1.5f > minDist)
                {
                    foundSpot = true;
                    FindSpot(hit, sort);
                }
            }
        }
    }

    private void FindSpot(RaycastHit hit3, CheckingSort sort)
    {
        if(Vector3.Angle(hit3.normal, Vector3.up) < maxAngle)
        {
            RayInfo rayIn = new RayInfo();
            /* optimise with else if to not go off if the first one goes off */
            if(sort == CheckingSort.normal)
            {
                rayIn = GetClosestPoint(hit3.transform, hit3.point + new Vector3(0, -0.01f, 0), transform.forward / 2.5f);
            }
            else if (sort == CheckingSort.turning)
            {
                rayIn = GetClosestPoint(hit3.transform, hit3.point + new Vector3(0, -0.01f, 0), transform.forward / 2.5f - transform.right * pc.hor);
            }
            else if (sort == CheckingSort.falling)
            {
                rayIn = GetClosestPoint(hit3.transform, hit3.point + new Vector3(0, -0.01f, 0), transform.forward / 2.5f);
            }

            targetPoint = rayIn.point;
            targetNorm = rayIn.normal;

            if (rayIn.canGoToPoint)
            {
                if(currentSort != Climbingsort.Climbing && currentSort != Climbingsort.ClimbingTowardPoint)
                {
                    //DISABLE MOVEMENT IN CONTROLLER
                    rb.isKinematic = true;
                    pc.isGrounded = false;
                    pc.enabled = false;
                }
                currentSort = Climbingsort.ClimbingTowardPoint;
                beginDist = Vector3.Distance(transform.position, (targetPoint - transform.rotation * handTrans.localPosition));
            }
        }
    }

    private RayInfo GetClosestPoint(Transform trans, Vector3 pos, Vector3 dir)
    {
        RayInfo curRay = new RayInfo();
        RaycastHit hit2;
        int oldLayer = trans.gameObject.layer;
        /* CHANGE THE LAYER TO THE ONE NEEDED */
        trans.gameObject.layer = 14;

        if(Physics.Raycast(pos - dir, dir, out hit2, dir.magnitude * 2, curSpotLayer))
        {
            curRay.point = hit2.point;
            curRay.normal = hit2.normal;

            if (!Physics.Linecast(handTrans.position + transform.rotation * new Vector3(0,0.05f, -0.05f),
            curRay.point + new Vector3(0,0.5f,0), out hit2, checkLayersReachable))
            {
                /* CHECKING THROUGH THE WALL FOR A POINT - checking both sides of the normal*/
                if(!Physics.Linecast(curRay.point - Quaternion.Euler(new Vector3(0,90,0))
                * curRay.normal * 0.35f + 0.1f * curRay.point, curRay.point + Quaternion.Euler(new Vector3(0, 90, 0))
                * curRay.normal * 0.35f + 0.1f * curRay.normal, out hit2, checkLayersObstacle))
                {
                    if (!Physics.Linecast(curRay.point + Quaternion.Euler(new Vector3(0, 90, 0))
                    * curRay.normal * 0.35f + 0.1f * curRay.normal, curRay.point - Quaternion.Euler(new Vector3(0, 90, 0))
                    * curRay.normal * 0.35f + 0.1f * curRay.point, out hit2, checkLayersObstacle))
                    {
                        curRay.canGoToPoint = true;
                    }
                    else
                    {
                        curRay.canGoToPoint = false;
                    }
                }
                else
                {
                    curRay.canGoToPoint = false;
                }
            }
            else
            {
                curRay.canGoToPoint = false;
            }
            
            trans.gameObject.layer = oldLayer;
            return curRay;

        }
        else
        {
            trans.gameObject.layer = oldLayer;
            return curRay;
        }
    }

    private void CheckForClimbStart()
    {
        RaycastHit hit2;
        Vector3 dir = transform.forward - transform.up / 0.8f;

        /* Looking for the edge that we will hang off */
        if (!Physics.Raycast(transform.position + transform.rotation * raycastPos, dir, 1.6f)  && pc.isGrounded) 
        {
            currentSort = Climbingsort.CheckingForClimbStart;
            if(Physics.Raycast(transform.position + new Vector3(0, 1.1f, 0), -transform.up, out hit2, 1.6f, spotLayer))
            {
                FindSpot(hit2, CheckingSort.falling);
            }
        }
    }

    private void CheckForPlateu()
    {
        RaycastHit hit2;
        Vector3 dir = transform.up + transform.forward/ 2;
        if (!Physics.Raycast(handTrans.position + transform.rotation * verHandOffset, dir, out hit2, 1.5f, spotLayer))
        {
            currentSort = Climbingsort.ClimbingTowardPlateu;
            if (Physics.Raycast(handTrans.position + dir * 1.5f, -Vector3.up, out hit2, 1.7f, spotLayer))
            {
                targetPoint = handTrans.position + dir * 1.5f;
            }
            else
            {
                targetPoint = handTrans.position + dir * 1.5f - transform.rotation * new Vector3(0, -0.2f, 0.25f);
            }

            targetNorm = -transform.forward;
            //ANIMATOR - SET CROUCH TRUE AND SET ON GROUND - whateves i want
        }
    }

    /*last thing we are going to do */
    private void MoveTowardsPoint()
    {
        transform.position = Vector3.Lerp(transform.position,
        //if going to rotate around a point we need to change the rotation of the player
        (targetPoint - transform.rotation * handTrans.localPosition),
        Time.deltaTime * climbForce);

        //face toward - inverting the value of the normal to look toward the wall
        Quaternion lookRot = Quaternion.LookRotation(-targetNorm);

        //Rotate toward the wall - re use the climbforce value to stay in line with movement
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot,
        Time.deltaTime * climbForce);

        //PUT IN ANIMATION NEEDS

        float dist = Vector3.Distance(transform.position,
        (targetPoint - transform.rotation * handTrans.localPosition));

        float percent = -9 * (beginDist - dist) / beginDist;
        //PUT PERCENT INTO ANIMATOR FLOAT -> anim.SetFloat ("clim", percent);

        //if we are at a spot w/ room for failure
        if (dist <= 0.01f && currentSort == Climbingsort.ClimbingTowardPoint)
        {
            transform.position = targetPoint - transform.rotation * handTrans.localPosition;
            transform.rotation = lookRot;

            //reset system if it overworks and confuses itself
            lastTime = Time.time;
            currentSort = Climbingsort.Climbing;
        }

        //ALLOW A WALK ONTO SURFACE ANIMATION AS A DIFFERENCE
        if (dist <= 0.01f && currentSort == Climbingsort.ClimbingTowardPlateu)
        {
            transform.position = targetPoint - transform.rotation * handTrans.localPosition;
            transform.rotation = lookRot;

            //reset system if it overworks and confuses itself - BUT STANDING AT PLATEU
            lastTime = Time.time;
            currentSort = Climbingsort.Walking;

            rb.isKinematic = false;
            pc.enabled = true;
        }
    }
}
[Serializable]
public enum Climbingsort
{
    Walking,
    Jumping,
    Falling,
    Climbing, //on an edge hanging
    ClimbingTowardPoint, //like in the function
    ClimbingTowardPlateu, //climbing toward a surface
    CheckingForClimbStart, //on a plateu and checking for edges
}
/* get info from the point */
[Serializable]
public class RayInfo
{
    public Vector3 point;
    public Vector3 normal;
    public bool canGoToPoint;
}
[Serializable]
public enum CheckingSort
{
    normal,
    turning,
    falling,
}