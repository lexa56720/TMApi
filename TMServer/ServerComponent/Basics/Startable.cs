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
