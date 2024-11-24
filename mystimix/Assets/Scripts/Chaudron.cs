using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CauldronManager : MonoBehaviour
{
    [Header("Cauldron Settings")]
    public int requiredIngredients = 3; // Number of ingredients per set
    public Animator cauldronAnimator; // Animator for success animation
    public Transform ingredientEjectPoint; // Point to eject wrong ingredients

    private List<int> currentIngredients = new List<int>(); // IDs of ingredients currently in the cauldron
    private List<int> correctIngredients = new List<int>(); // Current correct ingredient set
    private List<List<int>> allIngredientSets = new List<List<int>>(); // All sets of random ingredients
    private int currentSetIndex = 0; // Tracks the current set
    private System.Random random = new System.Random(); // Random number generator

    private void Start()
    {
        GenerateAllIngredientSets();
        Debug.Log("Game Started! Solve all sets.");
        LoadNextSet();
    }

    private void GenerateAllIngredientSets()
    {
        for (int i = 0; i < 3; i++) // Generate 3 random sets
        {
            var newSet = Enumerable.Range(1, 9)
                                   .OrderBy(_ => random.Next())
                                   .Take(requiredIngredients)
                                   .ToList();
            allIngredientSets.Add(newSet);
        }

        for (int i = 0; i < allIngredientSets.Count; i++)
        {
            Debug.Log($"Set {i + 1}: {string.Join(", ", allIngredientSets[i])}");
        }
    }

    private void LoadNextSet()
    {
        if (currentSetIndex < allIngredientSets.Count)
        {
            correctIngredients = allIngredientSets[currentSetIndex];
            Debug.Log($"Current Set {currentSetIndex + 1}: {string.Join(", ", correctIngredients)}");
        }
        else
        {
            EndGame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has an Ingredient component
        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (ingredient != null)
        {
            AddIngredient(ingredient);
        }
    }

    private void AddIngredient(Ingredient ingredient)
    {
        // Add ingredient if it's not already in the cauldron
        if (currentIngredients.Count < requiredIngredients && !currentIngredients.Contains(ingredient.id))
        {
            currentIngredients.Add(ingredient.id);
            Destroy(ingredient.gameObject); // Consume the ingredient visually
        }

        if (currentIngredients.Count == requiredIngredients)
        {
            CheckIngredients();
        }
    }

    private void CheckIngredients()
    {
        // Check if the ingredients match the current set
        currentIngredients.Sort();
        correctIngredients.Sort();

        if (currentIngredients.SequenceEqual(correctIngredients))
        {
            Debug.Log($"Set {currentSetIndex + 1} Completed!");
            cauldronAnimator.SetTrigger("Success");

            currentSetIndex++; // Move to the next set
            LoadNextSet();
        }
        else
        {
            Debug.Log("Incorrect Ingredients! Try again.");
            EjectIngredients();
        }

        // Clear the cauldron for the next attempt
        currentIngredients.Clear();
    }

    private void EjectIngredients()
{
    foreach (var ingredientID in currentIngredients)
    {
        // Find the ingredient in the cauldron by ID (assuming it was instantiated)
        GameObject ingredientToEject = FindIngredientObjectByID(ingredientID);

        if (ingredientToEject != null)
        {
            // Move the ingredient to the eject point
            ingredientToEject.transform.position = ingredientEjectPoint.position;

            // Add Rigidbody and apply a force to simulate ejection
            Rigidbody rb = ingredientToEject.GetComponent<Rigidbody>();
            if (rb == null) rb = ingredientToEject.AddComponent<Rigidbody>();
            rb.velocity = Vector3.zero; // Reset any existing velocity
            rb.AddForce(Random.onUnitSphere * 2f, ForceMode.Impulse); // Eject in random direction

            // Optional: Make the ingredient grabbable again if needed
            EnableGrabbable(ingredientToEject);
        }
    }

    Debug.Log("Incorrect ingredients ejected!");
}

// Helper function to find the ingredient GameObject by its ID
private GameObject FindIngredientObjectByID(int id)
{
    Ingredient[] allIngredients = FindObjectsOfType<Ingredient>(); // Get all active ingredients in the scene
    foreach (Ingredient ingredient in allIngredients)
    {
        if (ingredient.id == id)
        {
            return ingredient.gameObject;
        }
    }
    Debug.LogWarning($"Ingredient with ID {id} not found!");
    return null;
}

// Helper function to make the ingredient grabbable again (specific to XR Grab Interactable)
private void EnableGrabbable(GameObject ingredient)
{
    var grabInteractable = ingredient.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
    if (grabInteractable != null)
    {
        grabInteractable.enabled = true; // Ensure it's enabled for interaction
    }
}


// Helper function to find the ingredient prefab by ID
private GameObject FindIngredientPrefabByID(int id)
{
    // Replace this with a reference to your ingredient prefabs
    foreach (GameObject prefab in ingredientPrefabs)
    {
        Ingredient ingredient = prefab.GetComponent<Ingredient>();
        if (ingredient != null && ingredient.id == id)
        {
            return prefab;
        }
    }
    Debug.LogWarning($"No ingredient prefab found with ID {id}!");
    return null;
}


    private void EndGame()
    {
        Debug.Log("Game Over! All sets completed!");
        // Trigger any end-game logic here (e.g., load a new scene, show a UI screen, etc.)
    }
}
