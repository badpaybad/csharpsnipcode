using System.Threading;
public abstract class AbstractDequeueWorker
{
    Thread _thread;
    bool _isStop;
    public AbstractDequeueWorker(string queueName)
    {
        _thread = new Thread(() =>
        {
            while (!_isStop)
            {
                try
                {
                    Do();
                }
                finally
                {
                    Thread.Sleep(100);
                }
            }
        });
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
            _thread = new Thread(() =>
            {
                while (!_isStop)
                {
                    try
                    {
                        Do();
                    }
                    finally
                    {
                        Thread.Sleep(100);
                    }
                }
            });

            Start();
        }
    }
    public void Stop()
    {

        _isStop = true;
    }

    public abstract void Do();

}