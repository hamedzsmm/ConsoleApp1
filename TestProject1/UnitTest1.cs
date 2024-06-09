using ConsoleApp1;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void TestNumberExtractor()
        {
            var a = "1 205.6 USD".GetNumber();
            var b = "-306.7 USDT".GetNumber();
            var c = "254.8 USD".GetNumber();
            var d = "1 057 USD".GetNumber();
            var e = "-11.2".GetNumber();

            Assert.True(a == (decimal)1205.6 &&
                        b == (decimal)-306.7 &&
                        c == (decimal)254.8 &&
                        d == (decimal)1057 &&
                        e == (decimal)-11.2);
        }


        [Fact]
        public void DiffExtractorCanDetectAdded()
        {
            var oldList = new List<TradeItem>
            {
                new("AB", "BUY", 1, DateTime.Now, 1, 1, 1, 1,1)
            };

            var newList = new List<TradeItem>
            {
                new("Added1", "BUY", 1, DateTime.Now, 1, 1, 1, 1,1),
                new("AB", "BUY", 1, DateTime.Now, 1, 1, 1, 1,1),
                new("Added2", "BUY", 1, DateTime.Now, 1, 1, 1, 1,1)
            };

            var diff = ListComparer.FindDifferences(oldList, newList, q => q.Id);

            Assert.True(diff.Added != null &&
                        diff.Added.Count() == 2 &&
                        diff.Added.All(q => q.Symbol.StartsWith("Added")));
        }

        [Fact]
        public void DiffExtractorCanDetectDelete()
        {
            var oldList = new List<TradeItem>
            {
                new("Del1", "BUY", 1, DateTime.Now, 1, 1, 1, 1,1),
                new("AB", "BUY", 1, DateTime.Now, 1, 1, 1, 1,1),
                new("Del2", "BUY", 1, DateTime.Now, 1, 1, 1, 1,1)
            };

            var newList = new List<TradeItem>
            {
                new("AB", "BUY", 1, DateTime.Now, 1, 1, 1, 1,1),
            };

            var diff = ListComparer.FindDifferences(oldList, newList, q => q.Id);

            Assert.True(diff.Deleted != null &&
                        diff.Deleted.Count() == 2 &&
                        diff.Deleted.All(q => q.Symbol.StartsWith("Del")));
        }
    }
}