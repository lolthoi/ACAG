using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACAG.Abacus.CalendarConnector.Scheduler.UnitTests
{

  [TestClass()]
  public class AbacusListener_Test
  {

    [TestMethod()]
    public void Start_Success()
    {
      var timer = new AbacusListener(100);
      int count = 0;
      timer.OnTimerAbacusJob += () =>
      {
        count++;
      };
      timer.Start();

      Thread.Sleep(1);
      Assert.IsTrue(timer.IsRunning);

      Thread.Sleep(340);
      timer.Stop();

      Thread.Sleep(600);

      var expectNo = 3 + 1;
      Assert.AreEqual(expectNo, count);
      Assert.IsFalse(timer.IsRunning);
    }

    [TestMethod()]
    public void Stop_Success()
    {
      var timer = new AbacusListener(100);
      int count = 0;
      timer.OnTimerAbacusJob += () =>
      {
        count++;
      };
      timer.Start();

      Thread.Sleep(340);
      timer.Stop();

      Thread.Sleep(600);

      var expectNo = 3+1;
      Assert.AreEqual(expectNo, count);
      Assert.IsFalse(timer.IsRunning);
    }

    [TestMethod()]
    public void Restart_Success()
    {
      var timer = new AbacusListener(500);
      int count = 0;
      timer.OnTimerAbacusJob += () =>
      {
        count++;
      };
      timer.Start();

      Thread.Sleep(1200);
      timer.Stop();

      timer.Start();
      Thread.Sleep(1200);
      timer.Stop();

      Thread.Sleep(600);

      var expectNo = 3 + 3;
      Assert.AreEqual(expectNo, count);
      Assert.IsFalse(timer.IsRunning);
    }
  }
}
