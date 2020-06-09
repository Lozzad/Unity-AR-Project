using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AugmentedImageSceneController : MonoBehaviour {

    public AugmentedImageVisualiser augmentedImageVisualiser;

    public GameObject FitToScanOverlay;

    private Dictionary<int, AugmentedImageVisualiser> m_Visualisers = new Dictionary<int, AugmentedImageVisualiser> ();

    private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage> ();

    void Awake () {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update () {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey (KeyCode.Escape)) {
            SceneManager.LoadScene ("MainMenu");
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking) {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        } else {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        //get updated augmented images for this frame
        Session.GetTrackables<AugmentedImage> (
            m_TempAugmentedImages, TrackableQueryFilter.Updated);

        //create visualisers and anchors for updated augmented images that are tracking
        //and dont have a visualiser. remove visualisers for stopped images
        foreach (var image in m_TempAugmentedImages) {
            AugmentedImageVisualiser visualiser = null;
            m_Visualisers.TryGetValue (image.DatabaseIndex, out visualiser);
            if (image.TrackingState == TrackingState.Tracking && visualiser == null) {
                //create anchor to make sure the image is tracked even off screen
                Anchor anchor = image.CreateAnchor (image.CenterPose);
                visualiser = (AugmentedImageVisualiser) Instantiate (augmentedImageVisualiser, anchor.transform);
                visualiser.image = image;
                m_Visualisers.Add (image.DatabaseIndex, visualiser);
            } else if (image.TrackingState == TrackingState.Stopped && visualiser != null) {
                m_Visualisers.Remove (image.DatabaseIndex);
                GameObject.Destroy (visualiser.gameObject);
            }
        }
        // Show the fit-to-scan overlay if there are no images that are Tracking.
        foreach (var visualiser in m_Visualisers.Values) {
            if (visualiser.image.TrackingState == TrackingState.Tracking) {
                FitToScanOverlay.SetActive (false);
                return;
            }
        }

        FitToScanOverlay.SetActive (true);
    }
}