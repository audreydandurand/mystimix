using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chaudron : MonoBehaviour
{
    [Header("Ingredient Settings")]
    public int totalIngredients = 9; // Total number of ingredients available
    public Transform hiddenStorage;  // A hidden area to store "eaten" ingredients

    public List<int> set1Ingredients = new List<int> { 1, 2, 3 };  // Ingredients for set 1
    public List<int> set2Ingredients = new List<int> { 4, 5, 6 };  // Ingredients for set 2
    public List<int> set3Ingredients = new List<int> { 7, 8, 9 };  // Ingredients for set 3

    private List<int> currentRecipe; // The active recipe
    private List<int> currentIngredients; // Ingredients currently in the cauldron

    private Dictionary<int, GameObject> ingredientObjects; // Tracks ingredient IDs and their corresponding GameObjects
    private Dictionary<int, Vector3> originalPositions; // Tracks original positions of ingredients
    private HashSet<int> ingredientsInHiddenStorage; // Keeps track of ingredients that have been moved to hidden storage

    private void Start()
    {
        ingredientObjects = new Dictionary<int, GameObject>();
        ingredientsInHiddenStorage = new HashSet<int>(); // Keeps track of ingredients in hidden storage
        originalPositions = new Dictionary<int, Vector3>(); // Keeps track of original ingredient positions

        Ingredient[] allIngredients = FindObjectsOfType<Ingredient>();
        foreach (var ingredient in allIngredients)
        {
            ingredientObjects[ingredient.id] = ingredient.gameObject;
            originalPositions[ingredient.id] = ingredient.transform.position; // Save original position
        }

        currentIngredients = new List<int>();
        GenerateNewRecipe();
    }

    private void GenerateNewRecipe()
    {
        if (set1Ingredients.Count == 0 || set2Ingredients.Count == 0 || set3Ingredients.Count == 0)
        {
            Debug.Log("Not enough ingredients left for a new recipe. Game over!");
            EndGame();
            return;
        }

        currentRecipe = new List<int>();

        // Randomly select one ingredient from each set
        currentRecipe.Add(SelectIngredientFromSet(set1Ingredients));
        currentRecipe.Add(SelectIngredientFromSet(set2Ingredients));
        currentRecipe.Add(SelectIngredientFromSet(set3Ingredients));

        Debug.Log("New recipe generated: " + string.Join(", ", currentRecipe));
    }

    private int SelectIngredientFromSet(List<int> ingredientSet)
    {
        if (ingredientSet.Count == 0)
        {
            Debug.LogWarning("Set is empty, no ingredients available!");
            return -1;
        }

        int randomIndex = Random.Range(0, ingredientSet.Count);
        int selectedIngredient = ingredientSet[randomIndex];
        ingredientSet.RemoveAt(randomIndex); // Remove the ingredient from the set to ensure uniqueness
        return selectedIngredient;
    }

    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (ingredient != null && !currentIngredients.Contains(ingredient.id) && !ingredientsInHiddenStorage.Contains(ingredient.id))
        {
            Debug.Log($"Ingredient {ingredient.id} entered the cauldron.");
            AddIngredient(ingredient);
        }
    }

    public void AddIngredient(Ingredient ingredient)
    {
        if (currentIngredients.Contains(ingredient.id))
        {
            Debug.Log($"Ingredient {ingredient.id} is already in the cauldron.");
            return;
        }

        currentIngredients.Add(ingredient.id);
        Debug.Log($"Ingredient {ingredient.id} added to the cauldron.");

        // Move the ingredient to hidden storage
        MoveIngredientToHiddenStorage(ingredient);

        if (currentIngredients.Count == 3)
        {
            CheckIngredients();
        }
    }

    private void MoveIngredientToHiddenStorage(Ingredient ingredient)
    {
        if (hiddenStorage == null)
        {
            Debug.LogError("Hidden storage Transform is not assigned! Please assign it in the Inspector.");
            return;
        }

        // Move the ingredient to the hidden storage area
        ingredient.transform.position = hiddenStorage.position;

        // Add the ingredient ID to the set of ingredients in hidden storage
        ingredientsInHiddenStorage.Add(ingredient.id);

        // Disable the ingredient's renderer to make it invisible
        Renderer renderer = ingredient.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;  // Hide the ingredient visually
        }

        // Disable the ingredient's collider to prevent interaction with it
        Collider collider = ingredient.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;  // Disable interaction
        }

        // Optionally disable the ingredient's interaction (e.g., XR interaction)
        var grabInteractable = ingredient.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

        Debug.Log($"Ingredient {ingredient.id} moved to hidden storage.");
    }

    private void CheckIngredients()
    {
        // Sort the lists to ensure order doesn't matter
        currentIngredients.Sort();
        currentRecipe.Sort();

        if (currentIngredients.SequenceEqual(currentRecipe))
        {
            Debug.Log("Correct ingredients! Moving to the next recipe.");
            currentIngredients.Clear(); // Clear the cauldron
            PlaySuccessAnimation();
            DisableConsumedIngredients(); // Disable consumed ingredients
            GenerateNewRecipe(); // Generate a new recipe
        }
        else
        {
            Debug.Log("Incorrect ingredients. Resetting the cauldron.");
            ResetIngredients(); // Reset the incorrect ingredients
        }
    }

    private void DisableConsumedIngredients()
    {
        // Disable ingredients that are part of the correct recipe
        foreach (var ingredientID in currentRecipe)
        {
            if (ingredientObjects.TryGetValue(ingredientID, out var ingredientObject))
            {
                // Disable XR interactivity so they can't be used again
                var grabInteractable = ingredientObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
                if (grabInteractable != null)
                {
                    grabInteractable.enabled = false;
                }

                // Disable the ingredient's renderer to make it invisible
                Renderer renderer = ingredientObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }

                // Disable the ingredient's collider to prevent interaction with it
                Collider collider = ingredientObject.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = false;
                }

                Debug.Log($"Ingredient {ingredientID} has been disabled after being consumed.");
            }
        }
    }

    private void ResetIngredients()
    {
        // Reset ingredients that are in the cauldron but not in hidden storage
        foreach (var ingredientID in currentIngredients)
        {
            if (ingredientObjects.TryGetValue(ingredientID, out var ingredientObject) && ingredientsInHiddenStorage.Contains(ingredientID))
            {
                // Reset the ingredient to its original position
                ingredientObject.transform.position = originalPositions[ingredientID];

                // Re-enable interaction for the ingredient
                var grabInteractable = ingredientObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
                if (grabInteractable != null)
                {
                    grabInteractable.enabled = true;
                }

                // Make the ingredient visible again
                Renderer renderer = ingredientObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = true;  // Make the ingredient visible again
                }

                // Re-enable the ingredient's collider for interaction
                Collider collider = ingredientObject.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = true;  // Re-enable interaction
                }

                Debug.Log($"Ingredient {ingredientID} has been reset.");
            }
        }

        currentIngredients.Clear(); // Clear the cauldron
        ingredientsInHiddenStorage.Clear(); // Clear the hidden storage tracking
    }

    private Vector3 GetRandomResetPosition()
    {
        // Customize this method to define where the reset ingredients should go
        return new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));
    }

    private void PlaySuccessAnimation()
    {
        Debug.Log("Success animation plays!");
        // Add animation logic here
    }

    private void EndGame()
    {
        Debug.Log("Congratulations! You've completed all the recipes!");
        // Add end-game animation or logic here
    }
}
