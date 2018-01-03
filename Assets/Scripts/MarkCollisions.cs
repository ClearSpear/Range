using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkCollisions : MonoBehaviour {

    public GameObject hit;

    public float xAdjust;
    public float yAdjust;
    public float zAdjust;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Shot")
        {
            print("Something hit");
            print(gameObject.name);

            Quaternion shotRotation = other.attachedRigidbody.rotation;
            shotRotation.x += xAdjust;
            shotRotation.y += yAdjust;
            shotRotation.z += zAdjust;

            Instantiate(hit, other.attachedRigidbody.position, shotRotation);

            print(other.gameObject.name + " destroyed"); 
            Destroy(other.gameObject);

            return;
        }
    }

}
