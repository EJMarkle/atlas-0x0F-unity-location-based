using UnityEngine;
using System.Collections;
using TMPro;

public class GPSUI : MonoBehaviour
{
    public TMP_Text longitudeData;
    public TMP_Text latitudeData;
    public TMP_Text altitudeData;
    public TMP_Text GPSStatus;
    public float desiredAccuracyInMeters = 1f;
    public float updateDistanceInMeters = 1f;
    float longitude;
    float latitude;
    float altitude;


    IEnumerator Start()
    {
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
            yield break; 
        }

        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location not enabled on device or app does not have permission to access location");
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
            GPSStatus.text = "Timed out";
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            GPSStatus.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            GPSStatus.text = "Success!";
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // longitudeData.text = "test";
        // latitudeData.text = "test";
        // altitudeData.text = "test";

        // Stops the location service if there is no need to query location updates continuously.
        // Input.location.Stop();
    }

    void Update()
    {
        UpdateGPS();
        longitudeData.text = longitude.ToString();
        latitudeData.text = latitude.ToString();
        altitudeData.text = altitude.ToString();
    }

    void UpdateGPS()
    {
        longitude = Input.location.lastData.longitude;
        latitude = Input.location.lastData.latitude;
        altitude = Input.location.lastData.altitude;
    }
}
