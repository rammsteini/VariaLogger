using BrowserInterop.Geolocation;

namespace VariaLogger
{
    public class StateContainer
    {

        public List<GeolocationPosition> PositionHistory { get; set; } = new List<GeolocationPosition>();


    }
}
