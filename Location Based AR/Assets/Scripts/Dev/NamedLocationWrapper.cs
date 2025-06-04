using System.Collections.Generic;

// Wrapper for json serialization
[System.Serializable]
public class NamedLocationWrapper
{
    public List<NamedLocation> locations;

    public NamedLocationWrapper(List<NamedLocation> locations)
    {
        this.locations = locations;
    }
}
