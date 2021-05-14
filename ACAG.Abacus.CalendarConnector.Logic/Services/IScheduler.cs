namespace ACAG.Abacus.CalendarConnector.Logic.Services
{
  public interface IScheduler
  {
    void StartAll();
    void StopAll();

    void Start(int tenantId);

    void Stop(int tenantId);
  }
}
