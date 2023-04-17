using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableSpace : MonoBehaviour
{
    public void Rotate(Quaternion rotation) {
        transform.rotation = rotation;
    }
}
