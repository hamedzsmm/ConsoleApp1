namespace ConsoleApp1;

public class TradeItem
{
    public TradeItem(
        string symbol,
        string type,
        decimal tradeVolume,
        DateTime openingDateAndTime,
        decimal entryPoint,
        decimal? currentLevel,
        decimal? stopLoss,
        decimal? takeProfit,
        decimal? profit)
    {
        Symbol = symbol;
        Type = type;
        TradeVolume = tradeVolume;

        if (openingDateAndTime < DateTime.Now.AddMonths(-1))
            openingDateAndTime = DateTime.Now;

        OpeningDateAndTime = openingDateAndTime;
        EntryPoint = entryPoint;
        CurrentLevel = currentLevel;
        StopLoss = stopLoss;
        TakeProfit = takeProfit;
        Profit = profit;
    }
    public string Symbol { get; private set; }
    public string Type { get; private set; }
    public decimal TradeVolume { get; private set; }
    public DateTime OpeningDateAndTime { get; private set; }
    public decimal EntryPoint { get; private set; }
    public decimal? CurrentLevel { get; private set; }
    public decimal? StopLoss { get; private set; }
    public decimal? TakeProfit { get; private set; }
    public decimal? Profit { get; private set; }
    public string Id => $"{Type.ToLower().Trim()}-{Symbol.ToLower().Trim()}-{OpeningDateAndTime}";
}