using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


/// <summary>
/// GameManager class, initalizes penguin data and enables win state
/// </summary>
public class GameManager : MonoBehaviour
{
    public List<NamedLocation> MasterLocations = new List<NamedLocation>();
    public NamedLocation selectedLocation;
    public LocationManager locationManager;
    public GameObject discoveredFalse;
    public GameObject discoveredTrue;
    public float winDistance = 5f;
    public GameObject PenguinSpawn;



    void Start()
    {
        PopulateMasterLocations();
        //LoadListFromJson();
    }

    private void PopulateMasterLocations()
    {
        MasterLocations = new List<NamedLocation>
        {
            new NamedLocation("Dotty", 36.1212399f, -95.9764414f, 200f),
            new NamedLocation("Willie P., the Wild One", 36.0938625f, -95.9758428f, 200f),
            new NamedLocation("Stella the Stewardess", 36.1405355f, -95.9692526f, 184f),
            new NamedLocation("Hildegaard the Happy Camper", 36.1409808f, -95.9650798f, 200f),
            new NamedLocation("Winona", 36.118457f, -95.9389468f, 200f),
            new NamedLocation("Dr. Zoolittle", 36.0811999f, -95.9587884f, 200f),
            new NamedLocation("Haute Fun", 36.082375f, -96.1190607f, 200f),
            new NamedLocation("Waddlesworth", 36.1029541f, -95.9059073f, 200f),
            new NamedLocation("Boomer", 36.1043534f, -95.9117864f, 200f),
            new NamedLocation("Sergeant P. Guin", 36.0906243f, -95.8969827f, 200f),
            new NamedLocation("Handy", 36.0944109f, -95.8681656f, 200f),
            new NamedLocation("Express Success", 36.0602944f, -95.8560606f, 200f),
            new NamedLocation("A Coat of a Different Color", 36.1475883f, -95.9854197f, 200f),
            new NamedLocation("Joy", 36.1469027f, -95.9885632f, 200f),
            new NamedLocation("Buddy the Penguin", 36.1335912f, -95.989344f, 200f),
            new NamedLocation("Illustrious Sir", 36.1217089f, -95.9052667f, 200f),
            new NamedLocation("Triztec the Office Penguin", 36.1538983f, -95.9922719f, 200f),
            new NamedLocation("P.M.", 36.0839603f, -95.8867956f, 200f),
            new NamedLocation("Ameriguin", 36.0662978f, -95.8858421f, 200f),
            new NamedLocation("Charity", 36.206257f, -95.906595f, 200f),
            new NamedLocation("Two-Faced Penguin", 36.15276f, -95.9889899f, 200f),
            new NamedLocation("Keeping the Wonder Alive", 36.1630069f, -95.7962982f, 200f),
            new NamedLocation("Girl from Patagonia", 36.0922421f, -95.8858581f, 200f),
            new NamedLocation("Happy", 36.0861225f, -95.9406493f, 200f),
            new NamedLocation("Home", 36.06489f, -95.92639f, 200f)
        };
    }

    void Update()
    {
        PenguinCheck();
    }

    private void PenguinCheck()
    {
        if (selectedLocation.discovered)
        {
            discoveredTrue.SetActive(true);
            discoveredFalse.SetActive(false);
            PenguinSpawn.SetActive(true);
        }
        else
        {
            discoveredTrue.SetActive(false);
            discoveredFalse.SetActive(true);
            PenguinSpawn.SetActive(false);
        }

        float userLat = locationManager.latitude;
        float userLon = locationManager.longitude;
        float targetLat = selectedLocation.latitude;
        float targetLon = selectedLocation.longitude;

        float distance = GetDistance(userLat, userLon, targetLat, targetLon);

        if (distance <= winDistance)
        {
            Debug.Log("Location Discovered");
            selectedLocation.discovered = true;
        }
    }

    // Haversine formula to calculate ditance between 2 gps coordinates
    private float GetDistance(float lat1, float lon1, float lat2, float lon2)
    {
        float R = 6371000f; // Earth's radius in meters

        // Convert degrees to radians
        float dLat = Mathf.Deg2Rad * (lat2 - lat1);
        float dLon = Mathf.Deg2Rad * (lon2 - lon1);

        float radLat1 = Mathf.Deg2Rad * lat1;
        float radLat2 = Mathf.Deg2Rad * lat2;

        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                Mathf.Cos(radLat1) * Mathf.Cos(radLat2) *
                Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        float distance = R * c;

        return distance;
    }


    private void LoadListFromJson()
    {
        TextAsset jsonMasterFile = Resources.Load<TextAsset>("MasterLocations");
        NamedLocationWrapper wrapper = JsonUtility.FromJson<NamedLocationWrapper>(jsonMasterFile.text);
        MasterLocations = wrapper.locations;
    }

    public void SwitchToDevScene()
    {
        SceneManager.LoadScene("Dev");
    }

    // Grab location data based on name of button calling this method
    public void SelectLocation()
    {
        GameObject sender = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        string buttonName = sender.name;
        Debug.Log("Button clicked: " + buttonName);
        NamedLocation found = MasterLocations.Find(loc => loc.name == buttonName);

        if (found != null)
        {
            selectedLocation = found;
        }
        else
        {
            Debug.LogWarning($"{buttonName} not found");
        }
    }

    public void NavigateToLocation()
    {
        if (selectedLocation == null)
        {
            Debug.LogWarning("No location selected!");
            return;
        }

        float lat = selectedLocation.latitude;
        float lon = selectedLocation.longitude;

        string url = "";

#if UNITY_ANDROID
        // Opens in Google Maps
        url = $"geo:0,0?q={lat},{lon}({UnityWebRequest.EscapeURL(selectedLocation.name)})";
#elif UNITY_IOS
        // Apple Maps via HTTPS
        url = $"http://maps.apple.com/?daddr={lat},{lon}&dirflg=d";
#else
        // Fallback to Google Maps web on other platforms
        url = $"https://www.google.com/maps/dir/?api=1&destination={lat},{lon}";
#endif

        Application.OpenURL(url);
    }

}
