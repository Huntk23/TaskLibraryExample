using HuntSoft.TaskLibraryExample;

var worker = new Worker();

Console.WriteLine("Starting library background work");

await worker.Start();

Console.WriteLine("Retrieving data every 15 seconds in the console or press 'Esc' key to end the retrieval and stop the library from working in the background");

await Task.Delay(250); // Lets give worker.Start() 250 milliseconds to retrieve the first result

do
{
    while (!Console.KeyAvailable)
    {
        Console.WriteLine("Result received at {0}: {1}", DateTime.Now, await worker.GetData());
        await Task.Delay(TimeSpan.FromSeconds(15)); // When we press Esc key, we will get stuck here until the delay finishes - outside the scope of the question
    }
} while (Console.ReadKey(true).Key != ConsoleKey.Escape);

Console.WriteLine("Calling Stop() that will cancel the task retrieval process within the library");

await worker.Stop();

Console.WriteLine("The library has shutdown the retrieval process, press any key to end");

worker.Dispose();

Console.ReadKey();