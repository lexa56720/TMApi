using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMServer.Logger;
using TMServer.ServerComponent.Basics;

namespace TMServer.Services
{
    internal abstract class Service : Startable
    {
        public TimeSpan Period { get; private set; }
        private ILogger Logger { get; }

        public Service(TimeSpan period,ILogger logger)
        {
            Period = period;
            Logger = logger;
        }

        public override void Start()
        {
            if (IsRunning)
                return;
            base.Start();
            Task.Run(async () =>
            {
                while (IsRunning)
                {
                    var timer = Task.Delay(Period);
                    await Work(Logger);
                    await timer;
                }
            });
        }

        protected abstract Task Work(ILogger logger);
    }
}
