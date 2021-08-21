using DapperDll;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Telegram.Bot;

namespace BotReceiver
{
    public class Program
    {
        private static ITelegramBotClient client;

        private static Lazy<List<Admin>> registeringAccounts = new Lazy<List<Admin>>();
        private static Lazy<List<Query>> requestingModerators = new Lazy<List<Query>>();

        private static Lazy<List<Chat>> chats = new Lazy<List<Chat>>(() => Chat_Repository.Select());
        private static Lazy<List<Admin>> admins = new Lazy<List<Admin>>(() => Admin_Repository.Select());
        private static Lazy<List<Moderator>> moderators = new Lazy<List<Moderator>>(() => Moderator_Repository.Select());
        private static Lazy<List<Query>> queries = new Lazy<List<Query>>(() => Query_Repository.Select());

        private static void Main(string[] args)
        {
            client = new TelegramBotClient(ConfigurationManager.ConnectionStrings["bot_key"].ConnectionString);

            client.OnMessage += Client_OnMessage;
            client.StartReceiving();
            Console.Read();
        }

        private static bool RegisterProcess(Telegram.Bot.Args.MessageEventArgs e)
        {
            foreach (Admin item in registeringAccounts.Value)
            {
                if (item.Admin_TelegramId == e.Message.From.Id)
                {
                    if (e.Message.Chat.Type != Telegram.Bot.Types.Enums.ChatType.Private)
                        return false;

                    if (item.Admin_Login == null)
                    {
                        item.Admin_Login = e.Message.Text;
                        client.SendTextMessageAsync(e.Message.From.Id, "Введите ваш пароль: ");

                        return false;
                    }
                    else
                    {
                        item.Admin_Password = e.Message.Text;
                        //Console.WriteLine($"{item.Admin_Login}, {item.Admin_Password}, {item.Admin_TelegramId}");

                        Admin_Repository.Insert(item);
                        admins = new Lazy<List<Admin>>(() => Admin_Repository.Select());
                        registeringAccounts.Value.Remove(item);

                        break;
                    }
                }
            }

            return true;
        }

        private static bool RequestProcess(Telegram.Bot.Args.MessageEventArgs e)
        {
            foreach (Query item in requestingModerators.Value)
            {
                if (item.Query_TelegramId == e.Message.From.Id)
                {
                    if (e.Message.Chat.Type != Telegram.Bot.Types.Enums.ChatType.Private)
                        return false;

                    if (item.Query_Text == null)
                    {
                        item.Query_Text = e.Message.Text;

                        Query_Repository.Insert(item);
                        queries = new Lazy<List<Query>>(() => Query_Repository.Select());
                        requestingModerators.Value.Remove(item);

                        break;
                    }
                }
            }

            return true;
        }

        private static async void Client_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            //Console.WriteLine($"{e.Message.From.Id}, {e.Message.Text}, {e.Message.Chat.Type}");

            if (e.Message.Chat.Type != Telegram.Bot.Types.Enums.ChatType.Private && !chats.Value.Exists(x => x.Chat_TelegramId == e.Message.Chat.Id))
            {
                Chat_Repository.Insert(new Chat() { Chat_TelegramId = e.Message.Chat.Id });
                chats = new Lazy<List<Chat>>(() => Chat_Repository.Select());
            }
                
            if (RegisterProcess(e) && RequestProcess(e))
            {
                try
                {
                    switch (e.Message.Text)
                    {
                        case "/admin":
                            {
                                if (e.Message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private)
                                    throw new Exception("Вы должны быть в беседе для выполнения данной команды.");

                                if (!client.GetChatAdministratorsAsync(e.Message.Chat.Id).Result.ToList().Exists(x => x.User.Id == e.Message.From.Id))
                                    throw new Exception($"Вы не являетесь администратором в беседе {e.Message.Chat.Title}.");

                                if (admins.Value.Exists(x => x.Admin_TelegramId == e.Message.From.Id))
                                    throw new Exception($"Вы уже являетесь администратором в беседе {e.Message.Chat.Title}.");
                                
                                client.PinChatMessageAsync(e.Message.Chat.Id, client.SendTextMessageAsync(e.Message.Chat.Id, $"{e.Message.From.FirstName} собирается стать админом!").Result.MessageId);

                                registeringAccounts.Value.Add(new Admin { Admin_TelegramId = e.Message.From.Id, Admin_ChatId = chats.Value.FirstOrDefault(x => x.Chat_TelegramId == e.Message.Chat.Id).Chat_Id });

                                client.SendTextMessageAsync(e.Message.From.Id, "Введите ваш логин: ");
                                    
                                break;
                            }
                        case "/request":
                            {
                                if (e.Message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private)
                                    throw new Exception("Вы должны быть в беседе для выполнения данной команды.");

                                if (moderators.Value.Exists(x => x.Moderator_TelegramId == e.Message.From.Id) || queries.Value.Exists(x => x.Query_TelegramId == e.Message.From.Id))
                                    throw new Exception($"Вы уже являетесь модератором или отправили заявку на модерацию в беседе {e.Message.Chat.Title}.");

                                client.PinChatMessageAsync(e.Message.Chat.Id, client.SendTextMessageAsync(e.Message.Chat.Id, $"{e.Message.From.FirstName} подаёт заявку на модерацию!").Result.MessageId);
                           
                                requestingModerators.Value.Add(new Query() { Query_TelegramId = e.Message.From.Id, Query_ChatId = chats.Value.FirstOrDefault(x => x.Chat_TelegramId == e.Message.Chat.Id).Chat_Id });

                                client.SendTextMessageAsync(e.Message.From.Id, "Введите информацию о себе к запросу на модерацию (и желаемые данные для входа): ");

                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    client.SendTextMessageAsync(e.Message.From.Id, ex.Message);
                }
            }
        }
    }
}
