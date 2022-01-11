


using Blazm.Bluetooth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharpGPX;
using SharpGPX.GPX1_1;
using System.Text;

namespace VariaLogger.Pages
{
	public partial class Counter
	{
		private int _cnt = 0;

		protected override void OnInitialized()
		{
			StateContainer.SomethingHappend += StateContainer_SomethingHappend;
			base.OnInitialized();
		}

		private void StateContainer_SomethingHappend(int i)
		{
			_cnt = i;
			StateHasChanged();
		}

		private void StartBackgroundWorker()
		{
			StateContainer?.StartBackgroundWorker();
		}

		private void StopBackgroundWorker()
		{
			StateContainer?.StopBackgroundWorker();
		}
	}
}