using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        // Ingredients for each group
        List<int> group1 = new List<int> { 1, 2, 3 };
        List<int> group2 = new List<int> { 4, 5, 6 };
        List<int> group3 = new List<int> { 7, 8, 9 };

        // Pick one ingredient from each group
        currentRecipe.Add(group1[Random.Range(0, group1.Count)]);
        currentRecipe.Add(group2[Random.Range(0, group2.Count)]);
        currentRecipe.Add(group3[Random.Range(0, group3.Count)]);

        // Remove the selected ingredients from the unusedIngredients pool
        foreach (var ingredientID in currentRecipe)
        {
            unusedIngredients.Remove(ingredientID);
        }

        Debug.Log("New recipe generated: " + string.Join(", ", currentRecipe));
        DisplayCurrentRecipe();  // Show the current recipe on the canvas
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
        Debug.Log($"Current Ingredients in Cauldron: {string.Join(", ", currentIngredients)}");
        Debug.Log($"Expected Recipe: {string.Join(", ", currentRecipe)}");

        List<int> sortedCurrentIngredients = new List<int>(currentIngredients);
        List<int> sortedCurrentRecipe = new List<int>(currentRecipe);

        sortedCurrentIngredients.Sort();  // Sort both the lists
        sortedCurrentRecipe.Sort();

        if (sortedCurrentIngredients.SequenceEqual(sortedCurrentRecipe))
        {
            Debug.Log("Correct ingredients! Recipe completed.");
            PlaySuccessAnimation();
            currentIngredients.Clear();  // Clear the ingredients after success
            GenerateNewRecipe();  // Generate a new recipe
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
                // Reset the ingredient to its original position
                Ingredient ingredientScript = ingredientObject.GetComponent<Ingredient>();
                if (ingredientScript != null)
                {
                    ingredientObject.transform.position = ingredientScript.originalPosition;
                }

                // Re-enable interactivity for reuse (if needed)
                var grabInteractable = ingredientObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
                if (grabInteractable != null)
                {
                    grabInteractable.enabled = true;
                }

                Debug.Log($"Ingredient {ingredientID} has been reset.");
            }
        }

        currentIngredients.Clear();  // Clear the cauldron
    }

    private void DisplayCurrentRecipe()
    {
        // Disable all ingredient GameObjects initially
        foreach (GameObject ingredientObject in ingredientCanvasGameObjects)
        {
            ingredientObject.SetActive(false);  // Disable all ingredients initially
        }

        // Now enable only the GameObjects for the ingredients in the current recipe
        foreach (int ingredientID in currentRecipe)
        {
            // Map ingredient ID to the correct list index (ID starts at 1, so subtract 1 for zero-indexing)
            int index = ingredientID - 1;

            if (index >= 0 && index < ingredientCanvasGameObjects.Count)
            {
                ingredientCanvasGameObjects[index].SetActive(true);  // Enable the ingredient's GameObject
                Debug.Log($"Displaying ingredient {ingredientID} at index {index}");
            }
            else
            {
                Debug.LogWarning($"Ingredient ID {ingredientID} is out of bounds for the ingredientCanvasGameObjects list.");
            }
        }
    }

private void PlaySuccessAnimation()
{
    string tagToFind = "";

    if (currentRecipe.SequenceEqual(new List<int> { 1, 4, 7 }))
    {
        tagToFind = "MagicBuffPink";  
        Debug.Log("Playing Recipe 1 Success Animation (MagicBuffPink)");
    }
    else if (currentRecipe.SequenceEqual(new List<int> { 2, 5, 8 }))
    {
        tagToFind = "MagicBuffWhite";  
        Debug.Log("Playing Recipe 2 Success Animation (MagicBuffWhite)");
    }
    else if (currentRecipe.SequenceEqual(new List<int> { 3, 6, 9 }))
    {
        tagToFind = "MagicBuffPurple"; 
        Debug.Log("Playing Recipe 3 Success Animation (MagicBuffPurple)");
    }
    else
    {
        Debug.LogWarning("Unknown recipe, no animation played.");
        return;  
    }

    GameObject particleObject = GameObject.FindWithTag(tagToFind);
    Debug.Log("Tag to Find: " + tagToFind);
    Debug.Log("Particle Object Found: " + (particleObject != null));

    if (particleObject != null)
    {
        particleObject.SetActive(true); 
        ParticleSystem ps = particleObject.GetComponent<ParticleSystem>();
        
        if (ps != null)
        {
            ps.Play(); 
            Debug.Log("Particle system playing: " + tagToFind);
        }
        else
        {
            Debug.LogWarning($"No ParticleSystem component found on {particleObject.name}");
        }
    }
    else
    {
        Debug.LogWarning($"No GameObject found with tag {tagToFind}");
    }
}



    private void EndGame()
    {
        Debug.Log("Congratulations! You've completed all the recipes!");
    }
}
