
using Blazm.Bluetooth;
using BrowserInterop.Geolocation;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharpGPX;
using SharpGPX.GPX1_1;
using System.Text;

namespace VariaLogger.Pages
{
	public partial class Index
	{

		int threats = 0;
		double speed = 0;
		int distance = 0;
		//int t0 = 0;
		//int t1 = 0;
		//int t2 = 0;
		//int t3 = 0;
		//int t4 = 0;
		//int t5 = 0;

		string firmware = "";
		string name = "";

		protected override void OnInitialized()
		{
			//Ticker();
			base.OnInitialized();
		}

		int cnt = 0;

		// use an async-void or a timer. An async-void needs no cleanup. 
		async void Ticker()
		{
			while (true)
			{
				await Task.Delay(1_000);
				cnt++;

				StateHasChanged();   // refresh everything
			}
		}


		private Device _device;


		private

		protected async Task Connect()
		{
			lastTimestamp = DateTime.UtcNow;
			lastDistance = 500;

			for (int i = 0; i < 10; i++)
			{
				Threats.Add(new Threat());
			}

			var serviceid = "6A4E3200-667B-11E3-949A-0800200C9A66".ToLower();
			var q = new RequestDeviceQuery();
			q.Filters.Add(new Filter() { Services = { serviceid } });
			q.OptionalServices.Add(serviceid);
			_device = await navigator.RequestDeviceAsync(q);


			var characteristics = "6A4E3203-667B-11E3-949A-0800200C9A66".ToLower();
			await _device.SetupNotifyAsync(serviceid, characteristics);
			_device.Notification += Value_Notification;


			WatchPosition();

		}

		protected async Task Disconnect()
		{
			_device.Notification -= Value_Notification;
		}


		List<Threat> Threats = new List<Threat>();

		DateTime lastTimestamp;
		double calcSpeed;
		double lastDistance;

		private void Value_Notification(object sender, CharacteristicEventArgs e)
		{
			var now = DateTime.UtcNow;

			var deltat = (now - lastTimestamp).Milliseconds / 1000.0;
			lastTimestamp = now;


			var data = e.Value.ToArray();

			byte b0 = 0;
			byte b1 = 0;
			byte b2 = 0;
			byte b3 = 0;
			byte b4 = 0;

			var l = data.Length;



			threats = (l - 1) / 3;




			for (int t = 0; t < 10; t++)
			{
				Threats[t].active = false;
			}



			StringBuilder sb = new StringBuilder();
			sb.Append($"Threats={threats}");

			for (int t = 0; t < threats; t++)
			{
				Threats[t].active = true;
				Threats[t].speed = data[3 + t * 3];
				Threats[t].distance = data[2 + t * 3];

				sb.Append($"Speed:{Threats[t].speed} D={Threats[t].distance} | ");

			}

			firstThreatSpeed = Threats[0].active ? Threats[0].speed : 0;
			firstThreatDistance= Threats[0].active ? Threats[0].distance : 0;





			//StringBuilder sb = new StringBuilder();
			//sb.Append($"Threats={threats}");
			//for (int i = 0; i < l; i++)
			//{
			//    sb.Append($"b{i}={data[i]}");
			//}



			//if (l >= 5)
			//{
			//	b4 = data[4];
			//}
			//if (l >= 4)
			//{
			//	b3 = data[3];
			//}
			//if (l >= 3)
			//{
			//	b2 = data[2];
			//}
			//if (l >= 2)
			//{
			//	b1 = data[1];
			//}
			//if (l >= 0)
			//{
			//	b0 = data[0];
			//}






			//System.Console.WriteLine($"Threats: {threats}. b0={b0} b1={b1} b2={b2} b3={b3} b4={b4}");
			System.Console.WriteLine(sb.ToString());

			distance = b2;
			speed = b3;


			var deltad = lastDistance - distance;

			lastDistance = distance;


			calcSpeed = Math.Round(deltad / deltat * 3.6);

			StateHasChanged();
		}


		double firstThreatSpeed = 0;
		double firstThreatDistance = 0;

		private class Threat
		{
			public bool active = false;
			public double speed = 0;
			public double distance = 0;
		}
		private WindowNavigatorGeolocation geolocationWrapper;
		private IAsyncDisposable geopositionWatcher;
		private double? currentSpeed;
		public async Task WatchPosition()
		{
			geopositionWatcher = await geolocationWrapper.WatchPosition(async (p) =>
			{
				StateContainer.PositionHistory.Add(p.Location);

				currentSpeed = p.Location.Coords.Speed == null ? 0 : p.Location.Coords.Speed * 3.6;
				//currentAltitude = p.Location.Coords.Altitude == null ? 0 : p.Location.Coords.Altitude;

				Console.WriteLine($"currentSpeed={currentSpeed}");

				StateHasChanged();
			}
			, new PositionOptions()
			{
				EnableHighAccuracy = true,
				MaximumAgeTimeSpan = TimeSpan.FromHours(100),
				TimeoutTimeSpan = TimeSpan.FromMinutes(100)
			}
				);
		}

	}
}