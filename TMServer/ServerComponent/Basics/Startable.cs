namespace TMServer.ServerComponent.Basics
{
    internal abstract class Startable
    {
        public virtual bool IsRunning { get; protected set; }

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
