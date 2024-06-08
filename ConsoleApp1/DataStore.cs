namespace ConsoleApp1
{
    public static class DataStore
    {
        public static List<Person> Persons = GetPersons();
        private static List<Person> GetPersons()
        {
            return new List<Person>
            {
                new(1,1120932, "@demaq"),
                new(2,1428766, "@Titans Group"),
                new(3,1143035, "@TradingSCapital")
            };
        }
    }

    public class Person
    {
        public Person(int id, long userId, string handle)
        {
            UserId = userId;
            Handle = handle;
            Id = id;
            PortfolioTradeItems = null;
            UpdateDate = null;
        }

        public int Id { get; private set; }
        public long UserId { get; private set; }
        public string Handle { get; private set; }
        public DateTime? UpdateDate { get; private set; }
        public List<TradeItem>? PortfolioTradeItems { get; private set; }

        public ListDifference<TradeItem> AddPortfolioAndGetDiff(List<TradeItem> tradeItems)
        {
            if (PortfolioTradeItems is null)
            {
                UpdateDate = DateTime.Now;
                PortfolioTradeItems = tradeItems;
                return new ListDifference<TradeItem>();
            }

            var diff = ListComparer.FindDifferences(tradeItems, PortfolioTradeItems, q => q.OpeningDateAndTime);
            UpdateDate = DateTime.Now;
            PortfolioTradeItems = tradeItems;
            return diff;
        }


        #region ReadOnly Properties

        public string PortfolioPage => $"https://my.litefinance.org/traders/trades?id={UserId}";
        public string TradeHistoryPage => $"https://my.litefinance.org/traders/trades-history?id={UserId}";
        public string InfoPage => $"https://my.litefinance.org/traders/info?id={UserId}";

        #endregion
    }
}
