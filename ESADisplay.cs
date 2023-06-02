using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESADisplay : MonoBehaviour
{
    public ESARadar Radar;
    public VRInteractable vrInteractable;
    public Actor AttachedPlane;
    [Serializable]
    public struct DisplayedObject
    {
        public GameObject Self;
        public Actor Target;
        public Actor AttachedPlane;
        public int RadarIndex;
        public float Scale;
        public enum Team
        {
            Neutral = 0,
            Allied = 1,
            Enemy = 2,
            Self = 3
        }
        public Team team;
        public Material NeutralMaterial;
        public Material AlliedMaterial;
        public Material EnemyMaterial;
        public Material SelfMaterial;
        public void ResolveRadarIndex(ESARadar Radar)
        {
            for (int i = 0; i < Radar.DetectedObjects.Count; i++)
            {
                if (Radar.DetectedObjects[i].Actor == Target)
                {
                    this.RadarIndex = i;
                }
            }

        }
        public void ResolveObjectTeam()
        {
            if(this.Target == AttachedPlane)
            {
                this.team = Team.Self;
                return;
            }
            if(this.Target.team == AttachedPlane.team)
            {
                this.team = Team.Allied;
                return;
            }
            if(this.Target.team != AttachedPlane.team)
            {
                this.team = Team.Enemy;
                return;
            }

        }
        public void ResolveObjectMaterial()
        {
            Renderer[] MeshRenderers = Self.GetComponentsInChildren<Renderer>();
            foreach (var i in MeshRenderers)
            {
            switch(this.team)
            {
                case Team.Neutral:
                {
                i.material = NeutralMaterial;
                break;
                }
                case Team.Allied:
                {
                i.material = AlliedMaterial;
                break;
                }
                case Team.Enemy:
                {
                i.material = EnemyMaterial;
                break;
                }
                case Team.Self:
                {
                i.material = SelfMaterial;
                break;
                }
            }
            }

        }
        public Vector3 ResolveRelativePosition(ESARadar Radar, ESADisplay Display, float DisplayRadius)
        {
            //Radar Side
            var RadarPosition = Radar.transform.position;

            var RadarDelta = RadarPosition - Radar.DetectedObjects[RadarIndex].DetectedPosition;

            //Display Side
            var DisplayPosition = Display.transform.position;
            var DisplayedObjectPosition = this.Self.transform.position;
            var DeltaScale = DisplayRadius / Radar.DetectionZoneRadius;
            var ScaledDelta = RadarDelta * DeltaScale;
            var output = DisplayPosition + (ScaledDelta * -1);
            return output;
        }
        public void Update(ESARadar Radar, ESADisplay Display, float DisplayRadius)
        {
            ResolveRadarIndex(Radar);
            this.Self.transform.position = ResolveRelativePosition(Radar, Display, DisplayRadius);
            this.Self.transform.rotation = Radar.DetectedObjects[RadarIndex].DetectedRotation;
            this.Self.transform.localScale = new Vector3(Scale, Scale, Scale);
            ResolveObjectTeam();
            ResolveObjectMaterial();
        }
    }
    public List<DisplayedObject> DisplayedObjects = new List<DisplayedObject>();
    public List<Actor> DetectedObjects = new List<Actor>();
    public GameObject DisplayObjectPrefab;
    public Material DisplayObjectNeutralMaterial;
    public Material DisplayObjectAlliedMaterial;
    public Material DisplayObjectEnemyMaterial;
    public Material DisplayObjectSelfMaterial;
    public float DisplayObjectScale = 1f;
    public float DisplayRadius = 5f;
    public bool DebugDisplay;
    public bool BeingInteractedWith = false;

    // Start is called before the first frame update
    void Start()
    {
        AttachedPlane = base.GetComponentInParent<Actor>();
    }

    // Update is called once per frame
    void Update()
    {
        DetectedObjects.Clear();
        foreach (var i in Radar.DetectedObjects)
        {
            DetectedObjects.Add(i.Actor);
        }
        for (int i = 0; i < DisplayedObjects.Count; i++)
        {
            if (DetectedObjects.Contains(DisplayedObjects[i].Target) == false)
            {
                DestroyDisplayObject(DisplayedObjects[i]);
            }
        }
        foreach (var i in DetectedObjects)
        {
            if (PreCheck(i) == false)
            {
                CreateDisplayObject(i);
            }

        }
        for (int i = 0; i < DisplayedObjects.Count; i++)
        {
            DisplayedObjects[i].Update(Radar, this, DisplayRadius);
        }
    }

    void OnDrawGizmos()
    {
        if (DebugDisplay)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, DisplayRadius);
        }
    }

    void CreateDisplayObject(Actor GameObject)
    {
        DisplayedObject contact = new DisplayedObject();
        contact.Self = Instantiate(DisplayObjectPrefab, Vector3.zero, Quaternion.identity);
        contact.Target = GameObject;
        contact.RadarIndex = -1;
        contact.Scale = DisplayObjectScale;
        contact.AttachedPlane = AttachedPlane;
        contact.NeutralMaterial = DisplayObjectNeutralMaterial;
        contact.AlliedMaterial = DisplayObjectAlliedMaterial;
        contact.EnemyMaterial = DisplayObjectEnemyMaterial;
        contact.SelfMaterial = DisplayObjectSelfMaterial;
        DisplayedObjects.Add(contact);
    }

    void DestroyDisplayObject(DisplayedObject DisplayObject)
    {
        var self = DisplayObject.Self;
        DisplayedObjects.Remove(DisplayObject);
        Destroy(self);
    }

    public bool PreCheck(Actor GameObject)
    {
        for (int i = 0; i < DisplayedObjects.Count; i++)
        {
            if (DisplayedObjects[i].Target == GameObject)
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator MoveInteraction(VRHandController Controller)
    {
        while(BeingInteractedWith == true)
        {
            var controllerPosition = Controller.transform.position;
            var displayPosition = this.transform.position;
            var worldspacedelta = controllerPosition - displayPosition;
            this.transform.localPosition = this.transform.localPosition + worldspacedelta;
            yield return null;
        }
        yield return null;
    }

    public void StartInteraction()
    {
        BeingInteractedWith = true;
        StartCoroutine(MoveInteraction(vrInteractable.nearbyController));
    }

    public void EndInteraction()
    {
        BeingInteractedWith = false;
    }
}
test