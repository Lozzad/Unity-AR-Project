using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

public class FoodController : MonoBehaviour {
    private DetectedPlane detectedPlane;
    private GameObject foodInstance;
    private float foodAge;
    private readonly float maxAge = 10f;

    public GameObject[] foodModels;

    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        if (detectedPlane == null) {
            return;
        }
        if (detectedPlane.TrackingState != TrackingState.Tracking) {
            return;
        }
        //check for subsumation
        while (detectedPlane.SubsumedBy != null) {
            detectedPlane = detectedPlane.SubsumedBy;
        }
        if (foodInstance == null || foodInstance.activeSelf == false) {
            SpawnFoodInstance ();
            return;
        }
        foodAge += Time.deltaTime;
        if (foodAge >= maxAge) {
            Destroy (foodInstance);
            foodInstance = null;
        }
    }

    public void SetSelectedPlane (DetectedPlane selectedPlane) {
        detectedPlane = selectedPlane;
    }

    void SpawnFoodInstance () {
        GameObject foodItem = foodModels[Random.Range (0, foodModels.Length)];

        //pick random vertex and select a random point between it and the center of the plane 
        List<Vector3> vertices = new List<Vector3> ();
        detectedPlane.GetBoundaryPolygon (vertices);
        Vector3 pt = vertices[Random.Range (0, vertices.Count)];
        float dist = Random.Range (0.05f, 1f);
        Vector3 position = Vector3.Lerp (pt, detectedPlane.CenterPose.position, dist);
        //move the object above the plane
        position.y += .05f;

        Anchor anchor = detectedPlane.CreateAnchor (new Pose (position, Quaternion.identity));

        foodInstance = Instantiate (foodItem, position, Quaternion.identity, anchor.transform);

        //set the tag
        foodInstance.tag = "food";

        foodInstance.transform.localScale = new Vector3 (.025f, .025f, .025f);
        foodInstance.transform.SetParent (anchor.transform);
        foodAge = 0;

        foodInstance.AddComponent<FoodMotion> ();
    }
}