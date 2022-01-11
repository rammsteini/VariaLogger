using BrowserInterop.Geolocation;

namespace VariaLogger
{
    public class StateContainer
    {

        public event SomethingHappendHandler SomethingHappend;
        public delegate void SomethingHappendHandler(int i);

        private Task _task = null;


        public void StartBackgroundWorker()
		{
            if (_task != null)
                return;

             _task = Task.Run(new Action(Method));
        }

        bool _stop = false;

        public void StopBackgroundWorker()
		{
            _stop = true;

            while (true)
            {
                if (_task.IsCanceled)
                {
                    _task = null;
                    return;
                }
                Thread.Sleep(500);
            }
		}


        int cnt = 0;

        private void Method()
        {
            Console.WriteLine("Method() started");

            while(true)
			{
                SomethingHappend(cnt);
                Console.WriteLine($"SomethingHappend={cnt++}");

                if (_stop)
				{
                    Console.WriteLine("Method() stopped");
                    return;
                }

                Thread.Sleep(1000);
            }
        }

        public List<GeolocationPosition> PositionHistory { get; set; } = new List<GeolocationPosition>();


    }
}
