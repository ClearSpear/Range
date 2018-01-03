using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LaserPointer : MonoBehaviour {

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
    private SteamVR_TrackedObject trackedObj;
    private GameObject reticle;
    private Transform teleportReticleTransform;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

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

    void Start ()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;

        //reticle = Instantiate(teleportReticlePrefab);
        //teleportReticleTransform = reticle.transform;
    }
	
	void Update () {

        if (Controller.GetHairTriggerDown())
        {
            RaycastHit hit;
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                hitPoint = hit.point;
                //ShowLaser(hit);

                //reticle.SetActive(true);
                //teleportReticleTransform.position = hitPoint + teleportReticleOffset;

                Teleport();
                Controller.TriggerHapticPulse(100);
            }
        }
        else 
        {
            //laser.SetActive(false);
            //reticle.SetActive(false);
        }
        
    }
}
