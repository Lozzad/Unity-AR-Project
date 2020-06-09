using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

public class SceneController : MonoBehaviour {
    public Camera firstPersonCamera;
    public ScoreBoardController scoreBoard;
    public SnakeController snakeController;
    // Start is called before the first frame update
    void Start () {
        QuitOnConnectionErrors ();
    }

    // Update is called once per frame
    void Update () {
        //session must be tracking in order to access the frame
        if (Session.Status != SessionStatus.Tracking) {
            int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        scoreBoard.SetScore (snakeController.GetLength ());

        //at end
        ProcessTouches ();
    }

    void QuitOnConnectionErrors () {
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted) {
            StartCoroutine (CodelabUtils.ToastAndExit (
                "Camera permission is needed to run this application.", 5
            ));
        } else if (Session.Status.IsError ()) {
            //this covers a variety of errors
            StartCoroutine (CodelabUtils.ToastAndExit (
                "Arcore encountered a problem connecting. Please restart the app.", 5
            ));
        }
    }

    void ProcessTouches () {
        Touch touch;
        if (Input.touchCount != 1 ||
            (touch = Input.GetTouch (0)).phase != TouchPhase.Began) {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags rayCastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast (touch.position.x, touch.position.y, rayCastFilter, out hit)) {
            SetSelectedPlane (hit.Trackable as DetectedPlane);
        }
    }

    void SetSelectedPlane (DetectedPlane selectedPlane) {
        Debug.Log ("selected plane centered at " + selectedPlane.CenterPose.position);
        scoreBoard.SetSelectedPlane (selectedPlane);
        snakeController.SetPlane (selectedPlane);
        GetComponent<FoodController> ().SetSelectedPlane (selectedPlane);
    }
}