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
            // Create a placeholder object to represent ejected ingredient (optional)
            GameObject dummyIngredient = new GameObject($"EjectedIngredient_{ingredientID}");
            Rigidbody rb = dummyIngredient.AddComponent<Rigidbody>();
            dummyIngredient.transform.position = ingredientEjectPoint.position;
            rb.AddForce(Random.insideUnitSphere * 2f, ForceMode.Impulse); // Random direction
            Destroy(dummyIngredient, 2f); // Clean up dummy objects
        }
    }

    private void EndGame()
    {
        Debug.Log("Fin des recettes!");
        // Trigger any end-game logic here (e.g., load a new scene, show a UI screen, etc.)
    }
}

