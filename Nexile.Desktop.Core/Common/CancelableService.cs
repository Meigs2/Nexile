namespace Nexile.Desktop.Core.Common;

public class CancelableService : IDisposable
{
    private readonly IDisposable _service;
    private readonly CancellationTokenSource _cts;
    private bool _started = false;
    public Guid ServiceId { get; }
    private Func<CancellationToken, Task> _startServiceAction;

    public CancelableService(IDisposable service, Func<CancellationToken, Task> startServiceAction,
        CancellationTokenSource cts)
    {
        _service = service;
        _cts = cts;
        ServiceId = Guid.NewGuid();
        _startServiceAction = startServiceAction;
    }

    public Task StartServiceAsync()
    {
        if (_started)
        {
            throw new Exception("Service is already started.");
        }

        _started = true;
        return _startServiceAction.Invoke(_cts.Token);
    }

    public void StartService()
    {
        if (_started)
        {
            throw new Exception("Service is already started.");
        }

        _started = true;
        _startServiceAction.Invoke(_cts.Token);
    }

    public Task Wait(TimeSpan timeout = default)
    {
        if (!_started)
        {
            throw new Exception("Service not started.");
        }

        if (timeout == default) timeout = TimeSpan.MaxValue;
        return Task.Delay(timeout, _cts.Token);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        _service.Dispose();
    }
}