using System;

namespace ConnectBuySellToday.Domain.Common;

public class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string CityName { get; set; } = string.Empty;

    public Location() { }

    public Location(double latitude, double longitude, string cityName)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90");
        
        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180");

        Latitude = latitude;
        Longitude = longitude;
        CityName = cityName ?? string.Empty;
    }

    public bool IsValid => Latitude >= -90 && Latitude <= 90 && Longitude >= -180 && Longitude <= 180;
}
