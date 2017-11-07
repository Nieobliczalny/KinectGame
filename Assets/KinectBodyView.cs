using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

public class KinectBodyView : MonoBehaviour
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private KinectBodyManager _BodyManager;

    public const string LEFT_HAND_OBJ = "LeftHandObj";
    public const string RIGHT_HAND_OBJ = "RightHandObj";

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (BodySourceManager == null)
        {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<KinectBodyManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }
    }

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.material = BoneMaterial;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.GetComponent<Collider>().enabled = false;

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.GetComponent<Collider>().enabled = false;
            jointObj.transform.parent = body.transform;
        }

        GameObject leftHandObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leftHandObj.transform.localScale = new Vector3(2f, 2f, 2f);
        leftHandObj.name = LEFT_HAND_OBJ;
        leftHandObj.GetComponent<Renderer>().material.color = Color.red;
        //Destroy(leftHandObj.GetComponent<SphereCollider>());
        //CircleCollider2D lcc = leftHandObj.AddComponent<CircleCollider2D>();
        SphereCollider lcc = leftHandObj.GetComponent<SphereCollider>();
        lcc.center = new Vector3(0f, 0f, 0f);
        //lcc.offset = new Vector2(0f, 0f);
        lcc.radius = 1f;
        leftHandObj.transform.parent = body.transform;

        GameObject rightHandObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightHandObj.transform.localScale = new Vector3(2f, 2f, 2f);
        rightHandObj.name = RIGHT_HAND_OBJ;
        rightHandObj.GetComponent<Renderer>().material.color = Color.blue;
        //Destroy(rightHandObj.GetComponent<SphereCollider>());
        //CircleCollider2D rcc = rightHandObj.AddComponent<CircleCollider2D>();
        SphereCollider rcc = rightHandObj.GetComponent<SphereCollider>();
        rcc.center = new Vector3(0f, 0f, 0f);
        //rcc.offset = new Vector2(0f, 0f);
        rcc.radius = 1f;
        rightHandObj.transform.parent = body.transform;

        return body;
    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        Vector3 leftHandPosition = Vector3.zero, rightHandPosition = Vector3.zero, leftShoulderPosition = Vector3.zero, rightShoulderPosition = Vector3.zero;
        double leftHandSize = 0, rightHandSize = 0;
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            Vector3? targetObjPosition = null;
            if (targetJoint.HasValue) targetObjPosition = GetVector3FromJoint(targetJoint.Value);

            //Calculate hand sizes to determine how far from shoulder hand tip is

            //Calculating leftHandSize
            switch (jt)
            {
                case Kinect.JointType.HandTipLeft:
                case Kinect.JointType.HandLeft:
                case Kinect.JointType.WristLeft:
                case Kinect.JointType.ElbowLeft:
                    //Add to size
                    if (targetObjPosition.HasValue) leftHandSize += (targetObjPosition.Value - jointObj.localPosition).magnitude;
                    break;
                default:
                    break;
            }

            //Calculating rightHandSize
            switch (jt)
            {
                case Kinect.JointType.HandTipRight:
                case Kinect.JointType.HandRight:
                case Kinect.JointType.WristLeft:
                case Kinect.JointType.ElbowLeft:
                    //Add to size
                    if (targetObjPosition.HasValue) rightHandSize += (targetObjPosition.Value - jointObj.localPosition).magnitude;
                    break;
                default:
                    break;
            }

            //Saving joint positions
            switch (jt)
            {
                case Kinect.JointType.HandTipLeft:
                    leftHandPosition = jointObj.localPosition;
                    Transform leftHandObj = bodyObject.transform.Find(LEFT_HAND_OBJ);
                    leftHandObj.localPosition = GetVector3FromJoint(sourceJoint);
                    break;
                case Kinect.JointType.ShoulderLeft:
                    leftShoulderPosition = jointObj.localPosition;
                    break;
                case Kinect.JointType.HandTipRight:
                    rightHandPosition = jointObj.localPosition;
                    Transform rightHandObj = bodyObject.transform.Find(RIGHT_HAND_OBJ);
                    rightHandObj.localPosition = GetVector3FromJoint(sourceJoint);
                    break;
                case Kinect.JointType.ShoulderRight:
                    rightShoulderPosition = jointObj.localPosition;
                    break;
            }

            //Draw line between joints
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if (targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, targetObjPosition.Value);
                lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }
        }
        double left = leftShoulderPosition.x - leftHandSize;
        double right = rightShoulderPosition.x + rightHandSize;
        double top = Math.Max(leftShoulderPosition.y + leftHandSize, rightShoulderPosition.y + rightHandSize);
        double bottom = Math.Min(leftShoulderPosition.y - leftHandSize, rightShoulderPosition.y - rightHandSize);
        //Przesunięcie układu o wektor [left,bottom] 
        leftHandPosition.x -= (float)left;
        leftHandPosition.y -= (float)bottom;
        rightHandPosition.x -= (float)left;
        rightHandPosition.y -= (float)bottom;
        right -= left;
        top -= bottom;
        left = 0;
        top = 0;
        //Przy okazji w right i top znajdują się rozmiary ekranu, przez co można to przeskalować na piksele:
        //x_pos_in_px = x_pos_in_kinect / right * x_size_of_screen_in_px;
        //y_pos_in_px = y_pos_in_kinect / top * y_size_of_screen_in_px;
        //Zostaje jeszcze kwestia proporcji ekranu, ale to już do późniejszej dyskusji zostawiam :-)
    }

    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green;

            case Kinect.TrackingState.Inferred:
                return Color.red;

            default:
                return Color.black;
        }
    }

    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, 0);// joint.Position.Z * 10);
    }
}
