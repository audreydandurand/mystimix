using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Chaudron : MonoBehaviour
{
    [Header("Ingredient Settings")]
    public int totalIngredients = 9; // Total number of ingredients available
    public Transform hiddenStorage;  // A hidden area to store "eaten" ingredients
    public List<GameObject> ingredientCanvasGameObjects;  // List to hold ingredient GameObjects in the canvas

    private List<int> unusedIngredients; // Tracks ingredients that are still available
    private List<int> currentRecipe; // The active recipe
    private List<int> currentIngredients; // Ingredients currently in the cauldron

    private Dictionary<int, GameObject> ingredientObjects; // Tracks ingredient IDs and their corresponding GameObjects

    private void Start()
    {
        // Initialize the unused ingredient pool with all ingredient IDs
        unusedIngredients = new List<int>();
        ingredientObjects = new Dictionary<int, GameObject>();

        Ingredient[] allIngredients = FindObjectsOfType<Ingredient>();
        foreach (var ingredient in allIngredients)
        {
            unusedIngredients.Add(ingredient.id);
            ingredientObjects[ingredient.id] = ingredient.gameObject;
        }

        currentIngredients = new List<int>();
        GenerateNewRecipe();
    }

    private void GenerateNewRecipe()
    {
        if (unusedIngredients.Count < 3)
        {
            Debug.Log("Not enough ingredients left for a new recipe. Game over!");
            EndGame();
            return;
        }

        currentRecipe = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, unusedIngredients.Count);
            currentRecipe.Add(unusedIngredients[randomIndex]);
            unusedIngredients.RemoveAt(randomIndex); // Remove the selected ingredient
        }

        Debug.Log("New recipe generated: " + string.Join(", ", currentRecipe));
        DisplayCurrentRecipe(); // Show the current recipe on the canvas
    }

    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (ingredient != null && !currentIngredients.Contains(ingredient.id))
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

        // Simulate the cauldron "eating" the ingredient
        ConsumeIngredient(ingredient);

        if (currentIngredients.Count == 3)
        {
            CheckIngredients();
        }
    }

    private void ConsumeIngredient(Ingredient ingredient)
    {
        if (hiddenStorage == null)
        {
            Debug.LogError("Hidden storage Transform is not assigned! Please assign it in the Inspector.");
            return;
        }

        // Move the ingredient to the hidden storage area
        ingredient.transform.position = hiddenStorage.position;

        // Disable XR interactivity while "consumed"
        var grabInteractable = ingredient.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

        Debug.Log($"Ingredient {ingredient.id} has been consumed by the cauldron.");
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
            GenerateNewRecipe(); // Generate a new recipe
        }
        else
        {
            Debug.Log("Incorrect ingredients. Resetting the cauldron.");
            ResetIngredients();
        }
    }

    private void ResetIngredients()
    {
        foreach (var ingredientID in currentIngredients)
        {
            if (ingredientObjects.TryGetValue(ingredientID, out var ingredientObject))
            {
                // Return the ingredient to its original location or make it available again
                ingredientObject.transform.position = GetRandomResetPosition();

                // Re-enable interactivity for reuse
                var grabInteractable = ingredientObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
                if (grabInteractable != null)
                {
                    grabInteractable.enabled = true;
                }

                Debug.Log($"Ingredient {ingredientID} has been reset.");
            }
        }

        currentIngredients.Clear(); // Clear the cauldron
    }

    private Vector3 GetRandomResetPosition()
    {
        // Customize this method to define where the reset ingredients should go
        return new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));
    }

    private void DisplayCurrentRecipe()
    {
        // Disable all ingredient GameObjects initially to clear previous selections
        foreach (GameObject ingredientObject in ingredientCanvasGameObjects)
        {
            ingredientObject.SetActive(false);  // Disable the GameObject to hide the image
        }

        // Now enable only the GameObjects for the ingredients in the current recipe
        foreach (int ingredientID in currentRecipe)
        {
            // Ensure that the ingredientID is within the range of available GameObjects
            if (ingredientID >= 1 && ingredientID <= ingredientCanvasGameObjects.Count)
            {
                // Enable the corresponding GameObject for this ingredientID
                ingredientCanvasGameObjects[ingredientID - 1].SetActive(true);  // Adjusting for 0-indexing
                Debug.Log($"Displaying ingredient {ingredientID} in GameObject slot {ingredientID}");
            }
            else
            {
                Debug.LogWarning($"Ingredient ID {ingredientID} does not correspond to a GameObject slot.");
            }
        }
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
