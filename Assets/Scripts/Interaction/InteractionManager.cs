using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    void Update() {
        if (Input.GetButton("Fire1")) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo)) {
                IInteractable obj = hitInfo.transform.GetComponentInParent<IInteractable>();
                if (obj != null) {
                    obj.OnInteract(hitInfo.collider);
                }
            }
        }
    }
}
