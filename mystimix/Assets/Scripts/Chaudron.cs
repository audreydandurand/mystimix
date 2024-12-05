using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chaudron : MonoBehaviour
{
    [Header("Ingredient Settings")]
    public Transform hiddenStorage;  // Hidden area to store "eaten" ingredients
    public List<GameObject> ingredientCanvasGameObjects; // UI GameObjects for ingredients
    public Animator[] recipeAnimators; // Animators for recipes 1, 2, and 3

    private List<int> unusedIngredients; // All unused ingredient IDs
    private List<int> currentRecipe; // Current active recipe
    private List<int> currentIngredients; // Ingredients in the cauldron
    private List<List<int>> ingredientGroups; // Groups of ingredients

    private Dictionary<int, GameObject> ingredientObjects; // Tracks ingredient IDs to GameObjects

    private void Start()
    {
        InitializeIngredients();
        GenerateNewRecipe();
    }

    private void InitializeIngredients()
    {
        unusedIngredients = new List<int>();
        ingredientObjects = new Dictionary<int, GameObject>();
        ingredientGroups = new List<List<int>> { new List<int>(), new List<int>(), new List<int>() };

        Ingredient[] allIngredients = FindObjectsOfType<Ingredient>();
        foreach (var ingredient in allIngredients)
        {
            unusedIngredients.Add(ingredient.id);
            ingredientObjects[ingredient.id] = ingredient.gameObject;

            // Assign each ingredient to its group (e.g., 1-3 => group 0, 4-6 => group 1, etc.)
            int groupIndex = (ingredient.id - 1) / 3;
            if (groupIndex < ingredientGroups.Count)
                ingredientGroups[groupIndex].Add(ingredient.id);
        }

        currentIngredients = new List<int>();
    }

    private void GenerateNewRecipe()
    {
        if (ingredientGroups.Any(group => group.Count == 0))
        {
            Debug.Log("Not enough ingredients left for a new recipe. Game over!");
            EndGame();
            return;
        }

        currentRecipe = new List<int>();

        for (int i = 0; i < ingredientGroups.Count; i++)
        {
            int randomIndex = Random.Range(0, ingredientGroups[i].Count);
            int selectedIngredient = ingredientGroups[i][randomIndex];

            currentRecipe.Add(selectedIngredient);
            ingredientGroups[i].Remove(selectedIngredient); // Remove from the group
        }

        Debug.Log("New recipe generated: " + string.Join(", ", currentRecipe));
        DisplayCurrentRecipe();
    }

    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (ingredient != null && !currentIngredients.Contains(ingredient.id))
        {
            AddIngredient(ingredient);
        }
    }

    private void AddIngredient(Ingredient ingredient)
    {
        currentIngredients.Add(ingredient.id);
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
            Debug.LogError("Hidden storage Transform is not assigned!");
            return;
        }

        ingredient.transform.position = hiddenStorage.position;

        var grabInteractable = ingredient.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
        if (grabInteractable != null)
            grabInteractable.enabled = false;
    }

    private void CheckIngredients()
    {
        currentIngredients.Sort();
        currentRecipe.Sort();

        if (currentIngredients.SequenceEqual(currentRecipe))
        {
            Debug.Log("Correct ingredients! Recipe completed.");
            PlaySuccessAnimation();
            currentIngredients.Clear();
            GenerateNewRecipe();
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
                ingredientObject.transform.position = GetRandomResetPosition();

                var grabInteractable = ingredientObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
                if (grabInteractable != null)
                    grabInteractable.enabled = true;
            }
        }

        currentIngredients.Clear();
    }

    private Vector3 GetRandomResetPosition()
    {
        return new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));
    }

    private void DisplayCurrentRecipe()
    {
        foreach (GameObject ingredientObject in ingredientCanvasGameObjects)
        {
            ingredientObject.SetActive(false);
        }

        foreach (int ingredientID in currentRecipe)
        {
            if (ingredientID >= 1 && ingredientID <= ingredientCanvasGameObjects.Count)
            {
                ingredientCanvasGameObjects[ingredientID - 1].SetActive(true);
            }
        }
    }

    private void PlaySuccessAnimation()
    {
        int recipeIndex = unusedIngredients.Count / 3; // Determine which recipe was completed
        if (recipeIndex >= 0 && recipeIndex < recipeAnimators.Length)
        {
            recipeAnimators[recipeIndex].SetTrigger("PlaySuccess");
            Debug.Log($"Playing success animation for recipe {recipeIndex + 1}");
        }
    }

    private void EndGame()
    {
        Debug.Log("All recipes completed!");
    }
}

