using ConsoleApp1;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void TestNumberExtracter()
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
    }
}