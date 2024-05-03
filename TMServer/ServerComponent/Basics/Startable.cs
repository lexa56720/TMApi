namespace TMServer.ServerComponent.Basics
{
    internal abstract class Startable
    {
        public virtual bool IsRunning { get; protected set; }

        public virtual async Task Start()
        {
            if (IsRunning)
                return;
            IsRunning = true;
        }
        public virtual async Task Stop()
        {
            if (!IsRunning)
                return;
            IsRunning = false;
        }
    }
}
