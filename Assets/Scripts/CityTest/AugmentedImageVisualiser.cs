using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

public class AugmentedImageVisualiser : MonoBehaviour {
    #region singleton
    public static AugmentedImageVisualiser Instance;
    void Awake () {
        Instance = this;
    }
    #endregion

    //the image to augment
    public AugmentedImage image;
    //the model to use
    public GameObject fullCity;
    public GameObject city;
    GameObject activeObject;

    // Start is called before the first frame update
    void Start () {
        activeObject = city;
    }

    // Update is called once per frame
    void Update () {
        if (image == null || image.TrackingState != TrackingState.Tracking) {
            activeObject.SetActive (false);
            return;
        }

        activeObject.transform.localPosition = Vector3.zero;
        activeObject.SetActive (true);
    }
}