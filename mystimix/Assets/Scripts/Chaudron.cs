using System.Collections.Generic;
using System.Linq;
using System.Collections;
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

    public GameObject magicBuffPink;
    public GameObject magicBuffWhite;
    public GameObject magicBuffGreen;
    public GameObject Canvas_end;
    public GameObject Chronometre;

    // Predefined recipes
    private List<int> recipe1;
    private List<int> recipe2;
    private List<int> recipe3;
   
    private int randomRecipe = 1;


    // Ingredients for each group
    private List<int> group1 = new List<int> { 1, 2, 3 };
    private List<int> group2 = new List<int> { 4, 5, 6 };
    private List<int> group3 = new List<int> { 7, 8, 9 };


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

        // Generate fixed recipes
        GenerateFixedRecipes();

        // Start with the first recipe
        GenerateNewRecipe();
    }

    private void GenerateFixedRecipes()
    {
     
        int random1 = Random.Range(0, group1.Count);
        int random2 = Random.Range(0, group2.Count);
        int random3 = Random.Range(0, group3.Count);

        // Randomly select one ingredient from each group for each recipe
        recipe1 = new List<int> { group1[random1], group2[random2], group3[random3] };
        group1.RemoveAt(random1);
        group2.RemoveAt(random2);
        group3.RemoveAt(random3);
   
      
        random1 = Random.Range(0, group1.Count);
        random2 = Random.Range(0, group2.Count);
        random3 = Random.Range(0, group3.Count);

        recipe2 = new List<int> { group1[random1], group2[random2], group3[random3] };
        group1.RemoveAt(random1);
        group2.RemoveAt(random2);
        group3.RemoveAt(random3);


        recipe3 = new List<int> { group1[0], group2[0], group3[0] };

        Debug.Log($"Recipe 1: {string.Join(", ", recipe1)}");
        Debug.Log($"Recipe 2: {string.Join(", ", recipe2)}");
        Debug.Log($"Recipe 3: {string.Join(", ", recipe3)}");
    }

    private void GenerateNewRecipe()
    {
        // Randomly choose one of the predefined recipes
        

        switch (randomRecipe)
        {
            case 1:
                currentRecipe = new List<int>(recipe1);
                randomRecipe += 1;
                break;
            case 2:
                currentRecipe = new List<int>(recipe2);
                randomRecipe += 1;
                break;
            case 3:
                currentRecipe = new List<int>(recipe3);
                randomRecipe += 1;
                break;
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

        StartCoroutine("reussite");
       
    }

    public IEnumerator reussite()
    {
        // D�sactiver tous les buffs au d�but
        magicBuffWhite.SetActive(false);
        magicBuffGreen.SetActive(false);
        magicBuffPink.SetActive(false);
        Canvas_end.SetActive(false);

        // V�rifier la recette et activer le bon buff
        if (currentRecipe.SequenceEqual(recipe1))
        {
            magicBuffWhite.SetActive(true);  // Active le buff correspondant � la recette 1
            yield return new WaitForSeconds(5f);
            magicBuffWhite.SetActive(false);
            Debug.Log("Playing Recipe 1 Success Animation (MagicBuffWhite)");
        }
        else if (currentRecipe.SequenceEqual(recipe2))
        {
            magicBuffGreen.SetActive(true);  // Active le buff correspondant � la recette 2
            yield return new WaitForSeconds(5f);
            magicBuffGreen.SetActive(false);
            Debug.Log("Playing Recipe 2 Success Animation (MagicBuffGreen)");
        }
        else if (currentRecipe.SequenceEqual(recipe3))
        {
            magicBuffPink.SetActive(true);   // Active le buff correspondant � la recette 3
            yield return new WaitForSeconds(5f);
            magicBuffPink.SetActive(false);
            Destroy(Chronometre);
            Canvas_end.SetActive(true);
            Debug.Log("Playing Recipe 3 Success Animation (MagicBuffBlue)");
        }
        else
        {
            Debug.LogWarning("Unknown recipe or already played, no animation played.");
        }
        yield break;
    }
}
