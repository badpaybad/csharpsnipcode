using csharpsnipcode;

public abstract class AbstractMessageSender<T>
{
    public void PushToQueue(T data)
    {
        string queueName = typeof(T).FullName;
        MemoryMessageBus.Instance.Enqueue(queueName, data);
    }

    public void PushToStack(T data)
    {
        string stackName = typeof(T).FullName;
        MemoryMessageBus.Instance.Push(stackName, data);
    }

    public void PublishToChannel( T data)
    {
        var channel= typeof(T).FullName;
        MemoryMessageBus.Instance.Publish(channel, data);
    }
}