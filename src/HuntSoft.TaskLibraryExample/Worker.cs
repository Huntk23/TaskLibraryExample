namespace HuntSoft.TaskLibraryExample;

public class Worker : IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly HttpClient _httpClient = new();

    private string? _result;

    public async Task Start(TimeSpan? refreshInterval = null)
    {
        var refreshIntervalLocal = refreshInterval ?? TimeSpan.FromMinutes(10); // Default is 10 minutes or pass in a shorter TimeSpan - be warned, by making this something shorter than a second will hit rate limits and other issues

        // TODO: TimeSpan validation

        // Start the work in a Task - depending on need and longevity, Task.Run() can also be used with minimal issue
        await Task.Factory.StartNew(async () =>
        {
            try
            {
                while (true)
                {
                    // Retrieve data from a website per requester asked
                    _result = await _httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/posts", _cancellationTokenSource.Token); // If the request to the server takes too long, pass in the token so Stop() will cancel this

                    // If more work is done after the GetStringAsync call, consider using the following when needed to break out of this while loop quicker than GetString or Task.Delay cancels

                    //if (_cancellationTokenSource.IsCancellationRequested)
                    //{
                    //    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    //}

                    // while (!_cancellationTokenSource.IsCancellationRequested) works too, it really depends on how the internal logic of the loop works
                    // {
                    //    // Work
                    // }

                    await Task.Delay(refreshIntervalLocal, _cancellationTokenSource.Token); // Cancellation token is passed into the delay in case Stop() is called during the delay
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Task has been canceled");
            }
        }, _cancellationTokenSource.Token);
    }

    public Task Stop()
    {
        _cancellationTokenSource.Cancel(); // Tell all the methods utilizing our token sources token to cancel

        return Task.CompletedTask;
    }

    public Task<string?> GetData()
    {
        return Task.FromResult(_result); // Not entirely safe if you have many threads running the same Worker instance and calling this method
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
        _httpClient.Dispose();
    }
}