using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

public class CityModel : MonoBehaviour {

    public GameObject prefab;
    public House[] houses;

    void Start () {
        for (int i = 0; i < houses.Length; i++) {
            var house = MakeHouse (houses[i].position, houses[i].dimensions);
            house.name = "house" + i;
        }
    }

    void Update () {

        if (AugmentedImageVisualiser.Instance.image == null || AugmentedImageVisualiser.Instance.image.TrackingState != TrackingState.Tracking) {
            gameObject.SetActive (false);
            return;
        }

        gameObject.transform.localPosition = Vector3.zero;
        gameObject.SetActive (true);

    }

    GameObject MakeHouse (Vector2 position, Vector3 dimensions) {
        var pos = new Vector3 (position.x, 0, position.y);
        var house = Instantiate (prefab, pos, Quaternion.identity);
        house.transform.localScale = dimensions;
        house.transform.SetParent (gameObject.transform);
        return house;
    }

}

[System.Serializable]
public class House {
    public Vector3 dimensions;
    public Vector2 position;
}