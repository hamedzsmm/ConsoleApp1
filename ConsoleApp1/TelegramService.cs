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
            106291548   //Hamed
        };
        private static DateTime? LastErrorMessageSentTime = null;

        #endregion

        public static async Task SendMessageByDifference(ListDifference<TradeItem> difference, Person person)
        {
            var sb = new StringBuilder();
            sb.Append($"$$$$$$ <b>{person.Handle}</b> $$$$$$\n");

            sb.Append($"<b>Added Into Portfolio</b> :\n");
            var index = 0;
            if (difference.Added != null)
                foreach (var tradeItem in difference.Added)
                {
                    sb.Append($"{++index}) <b>{tradeItem.Type}</b> <i>{tradeItem.Symbol}</i> at <i>{tradeItem.EntryPoint}</i> and tp: <i>{tradeItem.TakeProfit}</i> [{tradeItem.OpeningDateAndTime}]\n");
                }

            sb.Append($"<b>Deleted From Portfolio</b> :\n");
            index = 0;
            if (difference.Deleted != null)
                foreach (var tradeItem in difference.Deleted)
                {
                    sb.Append($"{++index}) <b>{tradeItem.Type}</b> <i>{tradeItem.Symbol}</i> at <i>{tradeItem.EntryPoint}</i> and tp: <i>{tradeItem.TakeProfit}</i> [{tradeItem.OpeningDateAndTime}]\n");
                }

            await SendMessageToAllParticipantsAsync(sb.ToString());
            Console.Title = $"{person.Handle} at {DateTime.Now}";
        }

        public static async Task SendErrorToAllParticipantsAsync(string errorMessage)
        {
            if (LastErrorMessageSentTime == null ||
               (DateTime.Now - LastErrorMessageSentTime.Value).TotalMinutes > 60)
                await SendMessageToAllParticipantsAsync(errorMessage);
        }

        private static async Task SendMessageToAllParticipantsAsync(string message)
        {
            var botClient = new TelegramBotClient($"{TelegramToken}");

            foreach (var participant in Participants)
            {
                await botClient.SendTextMessageAsync(new ChatId(participant),
                    message, parseMode: ParseMode.Html);
            }
        }
    }
}
