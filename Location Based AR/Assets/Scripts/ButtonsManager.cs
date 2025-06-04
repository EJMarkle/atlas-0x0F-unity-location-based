using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Class for loading icons from names based on discovered state
/// It's a mess
/// </summary>
public class ButtonsManager : MonoBehaviour
{
    [System.Serializable]
    public class ButtonBinding
    {
        public string locationName;
        public Button button;
    }

    public GameManager gameManager;
    public List<ButtonBinding> buttonBindings = new List<ButtonBinding>();

    private Dictionary<string, Sprite> iconLookup = new Dictionary<string, Sprite>();
    private Dictionary<string, bool> lastDiscoveredState = new Dictionary<string, bool>();

    void Start()
    {
        // Wait a frame to ensure GameManager has initialized
        StartCoroutine(InitializeAfterDelay());
    }

    System.Collections.IEnumerator InitializeAfterDelay()
    {
        yield return null; // Wait one frame

        // Verify GameManager reference
        if (gameManager == null)
        {
            Debug.LogError("GameManager reference is not set in ButtonsManager!");
            yield break;
        }

        // Check if MasterLocations is populated
        if (gameManager.MasterLocations == null || gameManager.MasterLocations.Count == 0)
        {
            Debug.LogError("GameManager.MasterLocations is empty or null!");
            yield break;
        }

        Debug.Log($"Found {gameManager.MasterLocations.Count} locations in MasterLocations");

        LoadIcons();
        InitializeLastStates();
        UpdateAllButtons();
    }

    void Update()
    {
        // Only update buttons when discovery state changes
        CheckForStateChanges();
    }

    void LoadIcons()
    {
        iconLookup.Clear();

        // Debug: Check if Resources/Icons folder exists and what's in it
        Debug.Log("Attempting to load icons from Resources/Icons...");

        // Try loading all sprites from Icons folder
        Sprite[] icons = Resources.LoadAll<Sprite>("Icons");

        if (icons == null || icons.Length == 0)
        {
            Debug.LogError("No sprites found in Resources/Icons! Make sure:");
            Debug.LogError("1. Folder exists at Assets/Resources/Icons/");
            Debug.LogError("2. Image files are imported as Sprites (not Texture2D)");
            Debug.LogError("3. Image files have .png, .jpg, or other supported extensions");

            // Try alternative loading methods
            Object[] allObjects = Resources.LoadAll("Icons");
            Debug.Log($"Found {allObjects.Length} objects of any type in Icons folder:");
            foreach (Object obj in allObjects)
            {
                Debug.Log($"  - {obj.name} (Type: {obj.GetType()})");
            }
            return;
        }

        foreach (Sprite sprite in icons)
        {
            iconLookup[sprite.name] = sprite;
            Debug.Log($"Loaded sprite: '{sprite.name}'");
        }

        Debug.Log($"Successfully loaded {iconLookup.Count} icons from Resources/Icons");
    }

    void InitializeLastStates()
    {
        lastDiscoveredState.Clear();
        foreach (var location in gameManager.MasterLocations)
        {
            lastDiscoveredState[location.name] = location.discovered;
        }
    }

    void CheckForStateChanges()
    {
        bool needsUpdate = false;

        foreach (var location in gameManager.MasterLocations)
        {
            if (lastDiscoveredState.ContainsKey(location.name))
            {
                if (lastDiscoveredState[location.name] != location.discovered)
                {
                    lastDiscoveredState[location.name] = location.discovered;
                    needsUpdate = true;
                }
            }
        }

        if (needsUpdate)
        {
            UpdateAllButtons();
        }
    }

    void UpdateAllButtons()
    {
        foreach (var binding in buttonBindings)
        {
            UpdateButton(binding);
        }
    }

    void UpdateButton(ButtonBinding binding)
    {
        if (binding.button == null)
        {
            Debug.LogWarning($"Button is null for location: {binding.locationName}");
            return;
        }

        // Debug: Show what we're looking for
        Debug.Log($"Looking for location: '{binding.locationName}'");
        Debug.Log($"Available locations in MasterLocations:");
        foreach (var loc in gameManager.MasterLocations)
        {
            Debug.Log($"  - '{loc.name}' (discovered: {loc.discovered})");
        }

        // Find the location in MasterLocations
        NamedLocation location = gameManager.MasterLocations.Find(loc => loc.name == binding.locationName);
        if (location == null)
        {
            Debug.LogError($"Location '{binding.locationName}' not found in MasterLocations");
            Debug.LogError("Make sure the locationName in ButtonBinding exactly matches the name in MasterLocations");
            return;
        }

        // Generate the icon key based on discovered state
        string normalizedName = NormalizeLocationName(binding.locationName);
        string suffix = location.discovered ? "_discovered" : "_undiscovered";
        string iconKey = normalizedName + suffix;

        Debug.Log($"Looking for icon with key: '{iconKey}'");

        // Try to find the icon
        if (iconLookup.TryGetValue(iconKey, out Sprite icon))
        {
            Image buttonImage = binding.button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = icon;
                Debug.Log($"✓ Updated button '{binding.locationName}' with icon '{iconKey}'");
            }
            else
            {
                Debug.LogWarning($"Button '{binding.button.name}' has no Image component");
            }
        }
        else
        {
            Debug.LogError($"Icon not found for key: '{iconKey}'");
            Debug.Log("Available icon keys:");
            foreach (var key in iconLookup.Keys)
            {
                Debug.Log($"  - '{key}'");
                if (key.ToLower().Contains(normalizedName.ToLower()))
                {
                    Debug.Log($"    ^ This might be a match for '{normalizedName}'");
                }
            }
        }
    }

    string NormalizeLocationName(string locationName)
    {
        // This normalization should match your actual file naming convention
        // Adjust this method based on how your icon files are actually named

        string normalized = locationName;

        // Remove common punctuation and spaces
        normalized = normalized.Replace(" ", "");
        normalized = normalized.Replace(".", "");
        normalized = normalized.Replace(",", "");
        normalized = normalized.Replace("'", "");
        normalized = normalized.Replace("-", "");

        // Handle special cases based on your file names
        // Add more replacements here if needed for specific location names

        return normalized;
    }

    // Public method to manually refresh all buttons (useful for testing)
    public void RefreshAllButtons()
    {
        LoadIcons();
        UpdateAllButtons();
    }

    // Public method to update a specific location's button
    public void UpdateLocationButton(string locationName)
    {
        ButtonBinding binding = buttonBindings.Find(b => b.locationName == locationName);
        if (binding != null)
        {
            UpdateButton(binding);
        }
    }
}