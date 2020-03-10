using System;
using csharpsnipcode;

public abstract class AbstractPubSubConsumer<T>
{
    public AbstractPubSubConsumer()
    {
    }

    public void Start()
    {
        var subcribeName = $"{typeof(T).FullName}_{this.GetType().FullName}";
        
        MemoryMessageBus.Instance.Subscribe<T>(typeof(T).FullName, subcribeName, Do);
    }

    public void Stop()
    {
          var subcribeName = $"{typeof(T).FullName}_{this.GetType().FullName}";
        MemoryMessageBus.Instance.Unsubscribe(typeof(T).FullName, subcribeName);
    }

    public abstract void Do(T data);
}