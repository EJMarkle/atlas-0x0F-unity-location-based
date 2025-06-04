using UnityEngine;
using System.Collections;
using TMPro;


// Dev mode UI
public class GPSUI : MonoBehaviour
{
    public LocationManager locationManager;
    public TMP_Text longitudeData;
    public TMP_Text latitudeData;
    public TMP_Text altitudeData;
    public TMP_Text GPSStatus;



    void Update()
    {
        longitudeData.text = locationManager.longitude.ToString();
        latitudeData.text = locationManager.latitude.ToString();
        altitudeData.text = locationManager.altitude.ToString();
        GPSStatus.text = locationManager.LocationStatus;
    }
}
