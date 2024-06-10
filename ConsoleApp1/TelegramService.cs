using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ConsoleApp1
{
    public static class TelegramService
    {
        #region Base Informations

        const string TelegramToken = "7247068109:AAHCrGWtht6PpCx8bGNgKqHsqNaYA7PERig";
        private static readonly List<long> Participants = new()
        {
            106291548,   //Hamed
            62429349,    //Hamid
            93308697     //Pedram
        };
        private static DateTime? LastErrorMessageSentTime = null;

        #endregion

        public static async Task SendMessageByDifference(ListDifference<TradeItem> difference, Person person)
        {
            var sb = new StringBuilder();
            sb.Append($"$$$$$$ <b><a href='{person.PortfolioPage}'>{person.Handle}</a></b> $$$$$$\n");

            sb.Append($"<b>Added (Open) Into Portfolio</b> :\n");
            var index = 0;
            if (difference.Added != null)
                foreach (var tradeItem in difference.Added)
                {
                    sb.Append($"{++index}) <b>{tradeItem.Type}</b> <i>{tradeItem.Symbol}</i> at <i>{tradeItem.EntryPoint}</i> and tp: <i>{tradeItem.TakeProfit}</i> [{tradeItem.OpeningDateAndTime}]\n");
                }

            sb.Append($"<b>Deleted (Close) From Portfolio</b> :\n");
            index = 0;
            if (difference.Deleted != null)
                foreach (var tradeItem in difference.Deleted)
                {
                    sb.Append($"{++index}) <b>{tradeItem.Type}</b> <i>{tradeItem.Symbol}</i> at <i>{tradeItem.EntryPoint}</i> and profit : <i>{tradeItem.Profit} USD</i>\n");
                }

            await SendMessageAsync(sb.ToString());
            Console.Title = $"{person.Handle} at {DateTime.Now}";
        }

        public static async Task SendErrorToAllParticipantsAsync(string errorMessage)
        {
            if (LastErrorMessageSentTime == null ||
               (DateTime.Now - LastErrorMessageSentTime.Value).TotalMinutes > 60)
                await SendMessageAsync(errorMessage);
        }

        public static async Task SendMessageAsync(string message, bool onlyToAdmin = false)
        {
            var botClient = new TelegramBotClient($"{TelegramToken}");
            if (onlyToAdmin)
            {
                await botClient.SendTextMessageAsync(new ChatId(106291548),
                    message, parseMode: ParseMode.Html);
            }
            else
            {
                foreach (var participant in Participants)
                {
                    await botClient.SendTextMessageAsync(new ChatId(participant),
                        message, parseMode: ParseMode.Html);
                }
            }
        }
    }
}
