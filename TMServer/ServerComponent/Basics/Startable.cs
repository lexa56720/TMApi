using CSDTP.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.ServerComponent.Basics
{
    internal abstract class Startable
    {
        public virtual bool IsRunning { get; private set; }

        public virtual void Start()
        {
            if (IsRunning)
                return;

            IsRunning = true;
        }
        public virtual void Stop()
        {
            if (!IsRunning)
                return;
            IsRunning = false;
        }
    }
}
