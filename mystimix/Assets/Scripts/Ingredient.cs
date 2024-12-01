using UnityEngine;

public class Ingredient : MonoBehaviour {
    public int id;
    public Vector3 originalPosition; // Store the original position here

    private void Start() {
        originalPosition = transform.position;
    }
}
