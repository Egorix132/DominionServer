using EvoClient;
using EvoClient.Evo;
/*
var strategy = StrategyGenome.FromConsole();
File.WriteAllText(strategy!.Name + "_console.txt", string.Join(" ", strategy.ToIntArray().Select(t => (int)t!)));
*/
//var strategy = StrategyGenome.FromFile("market-reconstruction_console.txt");

var evolution = new Evolution(20);
//evolution.AddStrategy(strategy);

await evolution.StartEducation();

while (true)
{
    Thread.Sleep(1000);
}

