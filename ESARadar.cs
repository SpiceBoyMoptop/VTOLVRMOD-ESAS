using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESARadar : MonoBehaviour
{
    [Serializable]
    public struct DetectedObject
    {
        public Actor Actor;
        public Vector3 DetectedPosition;
        public Quaternion DetectedRotation;
    }
    public List<Actor> TrackedObjects = new List<Actor>();
    public List<DetectedObject> DetectedObjects = new List<DetectedObject>();
    public Transform DetectionZoneTF;
    public float DetectionZoneRadius = 10f;
    public float RadarRefreshRate = 0.5f;
    public bool DebugRadar;
    public float RefreshInterval;
    public bool Updating;
    public bool Bugged = false;
    // Start is called before the first frame update
    [ContextMenu("Start")]
    void Start()
    {
        StartCoroutine(RefreshRadar());
        if (DebugRadar)
        {
            Debug.Log("ESA Radar Controller Start");
            foreach (Actor i in TrackedObjects)
            {
                Debug.Log(i);
            }
        }
        if (TargetManager.instance == null)
        {
            Debug.Log("Null TargetManager on Entry");
            Bugged = true;
        }
        else
        {
            TrackedObjects = TargetManager.instance.allActors;
        }
    }


    void OnDrawGizmos()
    {
        if (DebugRadar)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(DetectionZoneTF.position, DetectionZoneRadius);
            Gizmos.color = Color.red;
            if (DetectedObjects.Count > 0)
            {
                foreach (var i in DetectedObjects)
                {
                    Gizmos.DrawSphere(i.DetectedPosition, 0.1f);
                }
            }
        }
    }

    IEnumerator RefreshRadar()
    {
        Debug.Log("ESA Radar Routine Started");
        while (true)
        {
            //Debug.Log("ESA RR Tick");
            var DetectedObjectsColliders = Physics.OverlapSphere(DetectionZoneTF.position, DetectionZoneRadius);
            DetectedObjects.Clear();
            foreach (var i in DetectedObjectsColliders)
            {
                DetectedObject contact = new DetectedObject();
                if (i.GetComponentInParent<Actor>() != null)
                {
                    contact.Actor = i.GetComponentInParent<Actor>();
                    if (i.GetComponentInParent<Actor>().flightInfo != null)
                    {
                        contact.DetectedPosition = contact.Actor.transform.position;
                        contact.DetectedRotation = contact.Actor.transform.rotation;
                        if (DetectedObjects.Contains(contact) == false && TrackedObjects.Contains(contact.Actor) == true)
                        {
                            DetectedObjects.Add(contact);
                            continue;
                        }

                    }
                }


            }
            yield return new WaitForSeconds(RadarRefreshRate);
        }

    }

}
