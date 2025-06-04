using UnityEngine;


// NamedLocation struct, name. gps, and discovered flag
[System.Serializable]
public class NamedLocation
{
    public string name;
    public float latitude;
    public float longitude;
    public float altitude;
    public bool discovered;

    public NamedLocation(string name, float latitude, float longitude, float altitude)
    {
        this.name = name;
        this.latitude = latitude;
        this.longitude = longitude;
        this.altitude = altitude;
        discovered = false;
    }
}
