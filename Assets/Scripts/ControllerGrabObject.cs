using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    private GameObject collidingObject;
    private GameObject objectInHand;

    public GameObject gunHit;
    public GameObject laserPrefab;
    public Transform cameraRigTransform;
    public Transform headTransform;
    public Vector3 teleportReticleOffset;
    public LayerMask teleportMask;
    public GameObject teleportReticlePrefab;

    public AudioClip gunShot;
    public Transform shotSpawn;

    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    //private SteamVR_TrackedObject trackedObj;
    private GameObject reticle;
    private Transform teleportReticleTransform;

    public Vector3 gunShift; 

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = col.gameObject;
    }

    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }
        collidingObject = null;
    }

    private void GrabObject()
    {
        objectInHand = collidingObject;
        collidingObject = null;

        // Set gun's position to the same as the controller 

        Quaternion gunRotation = Controller.transform.rot;
        objectInHand.transform.rotation = gunRotation;
        objectInHand.transform.Rotate(90, 0, 0, Space.Self); 
        
        //objectInHand.transform.position = Controller.transform.pos; 
        //objectInHand.transform.Translate(gunShift, Space.Self);
        
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
        
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        objectInHand = null;
        
    }

    // Laser Pointer


    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }

    // Don't teleport, instantiate a hit at the proper location 
    private void Teleport()
    {
        //reticle.SetActive(false);
        //Vector3 difference = cameraRigTransform.position - headTransform.position;
        //difference.y = 0;

        //cameraRigTransform.position = hitPoint + difference;

        // Show gun shot location 
        Quaternion shotRotation = new Quaternion(0, 90, 90, 0);
        Instantiate(gunHit, hitPoint, shotRotation);

        // Play gun noise
        AudioSource.PlayClipAtPoint(gunShot, shotSpawn.position);
    }

    void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;

        //reticle = Instantiate(teleportReticlePrefab);
        //teleportReticleTransform = reticle.transform;
    }

    void Update () {

        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (!objectInHand && collidingObject && collidingObject.tag == "Gun")
            {
                GrabObject();
            }
            else if(objectInHand)
            {
                ReleaseObject(); 
            }
        }

        if (Controller.GetHairTriggerDown() && objectInHand && objectInHand.tag == "Gun")
        {
            RaycastHit hit;
            if (Physics.Raycast(trackedObj.transform.position, -1 * transform.up, out hit, 100, teleportMask))
            {
                hitPoint = hit.point;
                ShowLaser(hit);

                //reticle.SetActive(true);
                //teleportReticleTransform.position = hitPoint + teleportReticleOffset;

                Teleport();
                Controller.TriggerHapticPulse(100);
            }
        }
        else
        {
            laser.SetActive(false);
            //reticle.SetActive(false);
        }

        /*
        if (objectInHand && objectInHand.tag == "Gun")
        {
            objectInHand.transform.position = Controller.transform.pos;
            objectInHand.transform.rotation = Controller.transform.rot;
        }
        */
    }
}

