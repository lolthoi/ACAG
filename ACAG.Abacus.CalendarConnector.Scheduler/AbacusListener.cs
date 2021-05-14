using System;
using System.Threading.Tasks;

namespace ACAG.Abacus.CalendarConnector.Scheduler
{
  public delegate void TimerAbacusJob();
  public class AbacusListener : IDisposable
  {
    public int TimerInterval { get; set; }

    public event TimerAbacusJob OnTimerAbacusJob;

    public bool IsRunning { get; private set; }
    private bool _cancelToken = false;
    private bool _disposed = false;
    private const int _waitMiniSeconds = 500;

    public AbacusListener(int timeInterval)
    {
      TimerInterval = timeInterval;
    }

    public void Stop()
    {
      if (_disposed) return;

      _cancelToken = true;
    }

    public void Start()
    {
      if (_disposed) return;

      WaitStop();
      
      Task.Factory.StartNew(() =>
      {
        IsRunning = true;
        while (!_cancelToken)
        {
          try
          {
            OnTimerAbacusJob?.Invoke();
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
          }

          WaitInterval();
        }
        IsRunning = false;
      });
    }

    private void WaitStop()
    {
      _cancelToken = true;
      while (_cancelToken && IsRunning)
      {
        System.Threading.Thread.Sleep(100);
      }
      _cancelToken = false;
    }

    private void WaitInterval()
    {
      var loop = TimerInterval / _waitMiniSeconds;
      for (var i = 0; i < loop; i++)
      {
        if (_cancelToken)
          return;
        System.Threading.Thread.Sleep(_waitMiniSeconds);
      }

      var leftTime = TimerInterval % _waitMiniSeconds;
      if (leftTime > 0 && !_cancelToken)
      {
        System.Threading.Thread.Sleep(leftTime);
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!_disposed)
      {
        if (disposing)
        {
          _cancelToken = true;
        }
        _disposed = true;
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    ~AbacusListener()
    {
      Dispose(false);
    }
  }
}
