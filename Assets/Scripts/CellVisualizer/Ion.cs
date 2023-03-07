using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class Ion : MonoBehaviour
{
    private Rigidbody _rigidbody;
    
    public Bounds IonBounds {
        set;
        get;
    }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IonBounds.Contains(transform.position)) { // TODO: Update to allow multiple bounds
            _rigidbody.AddForce(Utilities.RandomPointInBounds(new Bounds(
                Vector3.zero, Vector3.one * 2 // magnitude of 2, might move to variable
            )));
        } else {
            // Add force to get ion back to proper position as defined by closestpoint, sqrdistance
            Vector3 returnForce = (IonBounds.ClosestPoint(transform.position) - transform.position).normalized;
            returnForce *= returnForce.sqrMagnitude;
            _rigidbody.AddForce(returnForce); 
        }
    }
}
