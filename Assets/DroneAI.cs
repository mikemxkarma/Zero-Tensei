using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DroneAI : MonoBehaviour {

    #region Variables
    public Transform target;
    Vector3 storeTarget;
    Vector3 newTargetPos;
    bool savePos;
    bool overrideTarget;

    Vector3 acceleration;
    Vector3 velocity;
    public float maxSpeed;
    public float sideSpan;
    float storeMaxSpeed;
    float targetSpeed;

    Rigidbody rigidBody;
    Transform obstacle;

    public List<Vector3> EscapeDirections = new List<Vector3>();
    #endregion

    void Start ()
    {
        storeMaxSpeed = maxSpeed;
        targetSpeed = storeMaxSpeed;

        rigidBody = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate ()
    {
        Debug.DrawLine(transform.position, target.position);

        Vector3 forces = MoveTowardsTarget(target.position);

        acceleration = forces;
        velocity += 2 * acceleration * Time.deltaTime;

        if(velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }

        rigidBody.velocity = velocity;

        Quaternion desiredRotation = Quaternion.LookRotation(velocity);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * 3);

        ObstacleAvoidance(transform.forward, 0);

        /*if (overrideTarget)
        {
            target.position = newTargetPos;
        }
        */
	}

    Vector3 MoveTowardsTarget(Vector3 target)
    {
        Vector3 distance = target - transform.position;

        if(distance.magnitude < 3)
        {
            return distance.normalized * -maxSpeed;
        }
        else
        {
            return distance.normalized * maxSpeed;
        }
    }

    void ObstacleAvoidance(Vector3 direction, float offsetX)
    {
        RaycastHit[] hit = Rays(direction, offsetX);

        for (int i = 0; i < hit.Length - 1; i++)
        {
            if(hit[i].transform.root.gameObject != this.gameObject)
            {
                if (!savePos)
                {
                    storeTarget = target.position;
                    obstacle = hit[i].transform;
                    savePos = true;
                }
                FindEscapeDirections(hit[i].collider);
            }
        }
        if(EscapeDirections.Count > 0)
        {
            if (!overrideTarget)
            {
                newTargetPos = getClosests();
                overrideTarget = true;
            }
        }
        float distance = Vector3.Distance(transform.position, target.position);

        if(distance < 5 + sideSpan)
        {
            if (savePos)
            {
                target.position = storeTarget;
                savePos = false;
            }

            overrideTarget = false;

            EscapeDirections.Clear();
        }
    }

    Vector3 getClosests()
    {
        Vector3 clos = EscapeDirections[0];
        float distance = Vector3.Distance(transform.position, EscapeDirections[0]);

        foreach(Vector3 dir in EscapeDirections)
        {
            float tempDistance = Vector3.Distance(transform.position, dir);

            if(tempDistance < distance)
            {
                distance = tempDistance;
                clos = dir;
            }
        }
        return clos;
    }

    void FindEscapeDirections(Collider col)
    {
        RaycastHit hitUp;
        RaycastHit hitDown;
        RaycastHit hitRight;
        RaycastHit hitLeft;

        if(Physics.Raycast(col.transform.position, col.transform.up, out hitUp, col.bounds.extents.y * 2 * sideSpan))
        {
        }
        else
        {
            Vector3 dir = col.transform.position + new Vector3(0, col.bounds.extents.y * 2 + sideSpan, 0);

            if (!EscapeDirections.Contains(dir))
            {
                EscapeDirections.Add(dir);
            }
        }

        if (Physics.Raycast(col.transform.position, -col.transform.up, out hitDown, col.bounds.extents.y * 2 * sideSpan))
        {
        }
        else
        {
            Vector3 dir = col.transform.position + new Vector3(0, -col.bounds.extents.y * 2 - sideSpan, 0);

            if (!EscapeDirections.Contains(dir))
            {
                EscapeDirections.Add(dir);
            }
        }

        if (Physics.Raycast(col.transform.position, col.transform.right, out hitRight, col.bounds.extents.x * 2 * sideSpan))
        {
        }
        else
        {
            Vector3 dir = col.transform.position + new Vector3(col.bounds.extents.x * 2 + sideSpan,0 , 0);

            if (!EscapeDirections.Contains(dir))
            {
                EscapeDirections.Add(dir);
            }
        }

        if (Physics.Raycast(col.transform.position, -col.transform.right, out hitLeft, col.bounds.extents.x * 2 * sideSpan))
        {
        }
        else
        {
            Vector3 dir = col.transform.position + new Vector3(-col.bounds.extents.x * 2 - sideSpan, 0, 0);

            if (!EscapeDirections.Contains(dir))
            {
                EscapeDirections.Add(dir);
            }
        }

    }

    RaycastHit[] Rays(Vector3 direction, float offsetX)
    {
        Ray ray = new Ray(transform.position + new Vector3(offsetX, 0, 0), direction);
        Debug.DrawRay(transform.position + new Vector3(offsetX, 0, 0), direction * 10 * maxSpeed, Color.red);

        float distanceToLookAhead = maxSpeed * 10;

        RaycastHit[] hits = Physics.SphereCastAll(ray, 1, distanceToLookAhead);

        return hits; 
    }


}
