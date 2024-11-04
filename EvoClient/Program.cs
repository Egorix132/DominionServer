using EvoClient;
using EvoClient.Evo;

/*var strategy = StrategyGenomeV2.FromConsole();
File.WriteAllText(EvolutionV2.BaseSavePath + strategy!.Name + "_console.txt", string.Join(" ", strategy.ToIntArray().Select(t => (int)t!)));
*/
//var strategy = StrategyGenomeV2.FromFile(Evolution.BaseSavePath + "gazovayaEblya2_console.txt");

var evolution = new EvolutionV2("EvoTurn-399 parent-EvoTurn-398 parent-EvoTurn-397  V1.0.txt", 20);
//evolution.AddStrategy(strategy);

await evolution.StartEducation();

while (true)
{
    Thread.Sleep(1000);
}

