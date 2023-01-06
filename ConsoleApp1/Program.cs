using Flurl.Http;
using System.Diagnostics;

var requests = Enumerable.Range(1, 20).ToList();

var sw1 = new Stopwatch();
sw1.Start();

foreach (var request in requests) await BanksService.GetBanks();

sw1.Stop();

BanksService.i = 0;

var sw2 = new Stopwatch();
sw2.Start();

await Parallel.ForEachAsync(requests, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, 
    async (_, __) => await BanksService.GetBanks());

sw2.Stop();

Console.WriteLine($"\n\n... {sw1.ElapsedMilliseconds / 1000} seg foreach / {sw2.ElapsedMilliseconds / 1000} seg Parallel.ForEach");

static class BanksService
{
    public static int i = 0;

    public static async Task<IEnumerable<Bank>> GetBanks()
    {
        var banks = await "https://brasilapi.com.br/api/banks/v1".GetJsonAsync<IEnumerable<Bank>>();
        // Console.WriteLine($"** banks: \n{string.Join(',', banks.Select(x => x.Name).ToList())}\n\n");
        Console.WriteLine($"{++i} {banks.Count()} banks");
        return banks;
    }
}

record Bank(string Name, string Code);