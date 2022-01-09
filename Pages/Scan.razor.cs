


using Blazm.Bluetooth;
using Microsoft.AspNetCore.Components;

namespace VariaLogger.Pages
{
    public partial class Scan
    {
        [Inject] IBluetoothNavigator navigator { get; set; }

        protected async Task StartScanning()
        {
            var q = new RequestDeviceQuery();
            q.AcceptAllDevices = true;
            var device = await navigator.RequestDeviceAsync(q);

        }
    }
}