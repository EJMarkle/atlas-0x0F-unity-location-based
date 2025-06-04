using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System;
using UnityEngine.SceneManagement;


/// <summary>
/// LocationManager class, handles gps services and provided dev mode functionality for registering new locations
/// </summary>
public class LocationManager : MonoBehaviour
{
    public TMP_InputField nameInputField;
    [SerializeField]
    public List<NamedLocation> savedLocations = new List<NamedLocation>();
    public float desiredAccuracyInMeters = 1f;
    public float updateDistanceInMeters = 1f;
    public string LocationStatus;
    public float latitude;
    public float longitude;
    public float altitude;


    void Start()
    {
        StartCoroutine(StartLocServices());
    }

    void Update()
    {
        GetGPSData();
    }

    void GetGPSData()
    {
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        altitude = Input.location.lastData.altitude;
    }

    public void SaveLocation()
    {
        string name = nameInputField.text;

        if (Input.location.status == LocationServiceStatus.Running)
        {
            savedLocations.Add(new NamedLocation(name, latitude, longitude, altitude));
        }
    }

    public void SaveLocationsToJson()
    {
        NamedLocationWrapper wrapper = new NamedLocationWrapper(savedLocations);
        string json = JsonUtility.ToJson(wrapper, true);
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filename = $"locations_{timestamp}.json";
        string path = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllText(path, json);
    }

    IEnumerator StartLocServices()
    {
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
            yield break;
        }

        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location not enabled on device or app does not have permission to access location");
            LocationStatus = "Location not enabled on device or app does not have permission to access location";
        }

        Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            LocationStatus = "Timed out";
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            LocationStatus = "Unable to determine device location";
            yield break;
        }
        else
        {
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            LocationStatus = "Success!";
            // Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }
    }

    public void SwitchToUserScene()
    {
        SceneManager.LoadScene("LocBasedAR");
    }
}
