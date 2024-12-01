using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public int id;
    public Vector3 originalPosition;

    private void Start() {
        originalPosition = transform.position;
    }
}


