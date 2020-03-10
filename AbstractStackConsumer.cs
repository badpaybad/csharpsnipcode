using System.Threading;
using csharpsnipcode;

public abstract class AbstractStackConsumer<T>
{
    Thread _thread;
    bool _isStop;
    public AbstractStackConsumer()
    {
        _thread = new Thread(() =>
        {
            LoopDequeue();
        });
    }

    private void LoopDequeue()
    {
        while (!_isStop)
        {
            try
            {
                string stackName = typeof(T).FullName;

                var obj = MemoryMessageBus.Instance.Pop<T>(stackName);
                if (obj != null)
                {
                    Do(obj);
                }
            }
            finally
            {
                Thread.Sleep(10);
            }
        }
    }

    public void Start()
    {
        _isStop = false;

        _thread.Start();
    }

    public void Resume()
    {
        if (_isStop)
        {
            _isStop = false;
            _thread = new Thread(() =>
            {
                LoopDequeue();
            });
            _thread.Start();
        }
    }
    public void Stop()
    {
        _isStop = true;
    }

    public abstract void Do(T data);

}