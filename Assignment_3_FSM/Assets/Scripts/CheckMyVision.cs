using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
public class CheckMyVision : MonoBehaviour
{

    //How sen we r about vision
    public enum enmSensitivity { HIGH, LOW };

    //var check senstivity
    public enmSensitivity sensitivity = enmSensitivity.HIGH;

    //R we able to see target now?
    public bool targetInSight = false;

    public float fieldOfVision = 90f;

    //need ref to our target here
    private Transform target = null;

    //ref to our eyes
    public Transform myEyes = null;

    //My transform comp
    public Transform npcTransform = null;

    //My sphere collider
    private SphereCollider sphereCollider = null;

    //last known sighting of objects?
    public Vector3 lastKnownSighting = Vector3.zero;

    private void Awake()
    {
        npcTransform = GetComponent<Transform>();
        sphereCollider = GetComponent<SphereCollider>();
        lastKnownSighting = npcTransform.position;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();


    }

    bool InMyFieldOfVision()
    {
        Vector3 dirToTarget = target.position - myEyes.position;
        //get angle btw forward and view direction
        float angle = Vector3.Angle(myEyes.forward, dirToTarget);

        //let us check if within field of view
        if (angle <= fieldOfVision)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    //We need a function to check line of sight
    bool ClearLineofSight()
    {
        RaycastHit hit;

        if (Physics.Raycast(myEyes.position, (target.position - myEyes.position).normalized,
            out hit, sphereCollider.radius))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    void UpdateSight()
    {
        switch (sensitivity)
        {
            case enmSensitivity.HIGH:
                targetInSight = InMyFieldOfVision() && ClearLineofSight();
                break;
            case enmSensitivity.LOW:
                targetInSight = InMyFieldOfVision() || ClearLineofSight();
                break;


        }
    }

    private void OnTriggerStay(Collider other)
    {
        UpdateSight();
        //update last known sighting
        if (targetInSight)
            lastKnownSighting = target.position;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        targetInSight = false;
    }
}
