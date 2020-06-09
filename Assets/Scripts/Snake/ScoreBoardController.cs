using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

public class ScoreBoardController : MonoBehaviour {
    public Camera firstPersonCamera;
    private Anchor anchor;
    private DetectedPlane detectedPlane;
    private float yOffset;
    private int score;

    // Start is called before the first frame update
    void Start () {
        foreach (Renderer r in GetComponentsInChildren<Renderer> ()) {
            r.enabled = false;
        }
    }

    // Update is called once per frame
    void Update () {
        //the tracking must be framtrackingstate . tracking in order to access the frame
        if (Session.Status != SessionStatus.Tracking) {
            return;
        }
        //if no plane then return 
        if (detectedPlane == null) {
            return;
        }
        //checkfor the plane being subsumed
        //if the plane has been subsumed switch attachment
        while (detectedPlane.SubsumedBy != null) {
            detectedPlane = detectedPlane.SubsumedBy;
        }
        //make scoreboard face user
        transform.LookAt (firstPersonCamera.transform);
        //move the position to stay consistent with the plane 
        transform.position = new Vector3 (transform.position.x, detectedPlane.CenterPose.position.y + yOffset, transform.position.z);

    }

    public void SetSelectedPlane (DetectedPlane detectedPlane) {
        this.detectedPlane = detectedPlane;
        CreateAnchor ();
    }

    void CreateAnchor () {
        //create the position of the anchor by raycasting a point upwards
        Vector2 pos = new Vector2 (Screen.width * .5f, Screen.height * .90f);
        Ray ray = firstPersonCamera.ScreenPointToRay (pos);
        Vector3 anchorPosition = ray.GetPoint (5f);

        //create the anchor at that point
        if (anchor != null) {
            Destroy (anchor);
        }
        anchor = detectedPlane.CreateAnchor (
            new Pose (anchorPosition, Quaternion.identity)
        );

        //attach the scoreboard to the anchor
        transform.position = anchorPosition;
        transform.SetParent (anchor.transform);

        //record the y offset from the plane
        yOffset = transform.position.y - detectedPlane.CenterPose.position.y;

        //finally enable the renderers
        foreach (Renderer r in GetComponentsInChildren<Renderer> ()) {
            r.enabled = true;
        }
    }

    public void SetScore (int score) {
        if (this.score != score) {
            GetComponentInChildren<TextMesh> ().text = "Score: " + score;
            this.score = score;
        }
    }
}