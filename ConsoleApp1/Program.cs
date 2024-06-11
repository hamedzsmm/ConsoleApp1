using AngleSharp;
using AngleSharp.Dom;
using ConsoleApp1;
using Serilog;
using System;
int nextPersonId = 1;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

mainLoop:
{
    try
    {
        while (true)
        {
            await CheckPortfolioByPersonIdAsync(nextPersonId);
            if (nextPersonId == DataStore.Persons.Max(q => q.Id))
            {
                nextPersonId = DataStore.Persons.Min(q => q.Id);
            }
            else
            {
                nextPersonId++;
            }

            var random = new Random();
            var delaySeconds = random.Next(9, 16);
            Log.Information($"Waiting {delaySeconds} seconds\n");
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
    }
    catch (Exception ex)
    {
        Log.Error($"{ex.Message}");
        await TelegramService.SendMessageAsync(ex.Message, true);
        goto mainLoop;
    }
}


async Task CheckPortfolioByPersonIdAsync(int id)
{
    Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$ BEGIN CHECKING $$$$$$$$$$$$$$$$$$$$$");
    var person = DataStore.Persons.First(q => q.Id == id);
    Log.Information($"Checking person : {person.Handle}");

    var portfolio = new List<TradeItem>();
    #region Crawling

    var config = Configuration.Default.WithDefaultLoader();
    var context = BrowsingContext.New(config);
    var document = await context.OpenAsync(person.PortfolioPage);
    var rowSelector = "div.content_row";
    var rows = document.QuerySelectorAll(rowSelector);

    if (rows.Any())
        foreach (var element in rows)
        {
            var symbol = element.Children[0].GetElementsByClassName("title").First().Text();
            var type = element.Children[1].GetElementsByClassName("label").First().Text();
            var volume = Convert.ToDecimal(element.Children[2].GetElementsByClassName("data_value").First().Text().GetNumber());
            var openingDateTimeString = element.Children[3].GetElementsByClassName("data_value").First().Text();
            DateTime.TryParse(openingDateTimeString, out var openingDateTime);
            var entryPoint = Convert.ToDecimal(element.Children[4].GetElementsByClassName("data_value").First().Text().GetNumber());

            decimal? currentLevel = element.Children[5].GetElementsByClassName("data_value").FirstOrDefault() == null ?
                null :
                Convert.ToDecimal(element.Children[5].GetElementsByClassName("data_value").First().Text().GetNumber());

            decimal? stopLoss = element.Children[6].GetElementsByClassName("data_value").FirstOrDefault() == null ?
                null :
                Convert.ToDecimal(element.Children[6].GetElementsByClassName("data_value").First().Text().GetNumber());

            decimal? takeProfit = element.Children[7].GetElementsByClassName("data_value").FirstOrDefault() == null ?
                null :
                Convert.ToDecimal(element.Children[7].GetElementsByClassName("data_value").First().Text().GetNumber());

            decimal? profit = element.Children[8].GetElementsByClassName("data_value").FirstOrDefault() == null ?
                null :
                Convert.ToDecimal(element.Children[8].GetElementsByClassName("data_value").First().Text().GetNumber());

            var approvedTypes = new List<string>
            {
                "buy",
                "sell"
            };
            if (approvedTypes.Contains(type.ToLower().Trim()))
                portfolio.Add(new TradeItem(symbol,
                    type,
                    volume,
                    openingDateTime,
                    entryPoint,
                    currentLevel,
                    stopLoss,
                    takeProfit,
                    profit));
        }
    else
    {
        var sectionSelector = "main.page_content";
        var sections = document.QuerySelectorAll(sectionSelector);
        if (!sections.Any())
        {
            Log.Information("Problem in loading content");
            await TelegramService.SendErrorToAdminAsync("Problem in loading content");
        }
    }

    #endregion

    Log.Information($"{portfolio.Count} open trades was exist in portfolio");
    var diff = person.AddPortfolioAndGetDiff(portfolio);
    if ((diff.Added != null && diff.Added.Any()) ||
        (diff.Deleted != null && diff.Deleted.Any()))
    {
        //store in redis
        var key = $"CopyTrading:{person.Username}:{DateTime.Now:dd/HH-mm-ss}";
        await RedisDataService.SetAsync(key, diff, TimeSpan.FromHours(1));
        Log.Information($"Sending Founded Changes From {person.Handle}");
        await TelegramService.SendMessageByDifference(diff, person);
    }

    Log.Information($"Checking completed person : {person.Handle}");
    Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$ END CHECKING $$$$$$$$$$$$$$$$$$$$$$$\n");
}