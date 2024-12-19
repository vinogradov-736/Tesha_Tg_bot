using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    static string token = "7064244196:AAFO5ows5pFqCXVNfLSHcymzyTEZnDILfsU"; 
    static TelegramBotClient botClient;
    static Dictionary<long, string> userNames = new Dictionary<long, string>(); // Словарь для хранения имен пользователей
    static List<string> tips = new List<string>
    {
        "Не забудь зарядить телефон перед выходом из дома.",
        "Собака – не лучший советчик в финансах.",
        "Улыбайся – это бесплатный аксессуар.",
        "Сегодня лучше отдохнуть, чем делать ненужное.",
        "Не клади яйца в одну корзину. А лучше вообще их съешь!"
    };

    static string currentTip = ""; // Текущий совет
    static DateTime lastTipTime = DateTime.MinValue; // Время последнего обновления совета

    static async Task Main(string[] args)
    {
        botClient = new TelegramBotClient(token);
        Console.WriteLine("Бот запущен...");

        // Запуск таймера для обновления совета каждые 2 минуты
        _ = Task.Run(async () =>
        {
            while (true)
            {
                await UpdateTipIfNeeded();
                await Task.Delay(TimeSpan.FromMinutes(2)); // Ждем 2 минуты
            }
        });

        // Настройка для обработки обновлений
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // Получать все типы обновлений
        };

        var handler = new UpdateHandler();

        botClient.StartReceiving(
            updateHandler: handler,
            receiverOptions: receiverOptions,
            cancellationToken: CancellationToken.None
        );

        Console.ReadLine();
    }

    // Обновление совета, если прошло больше 2 минут
    static async Task UpdateTipIfNeeded()
    {
        var now = DateTime.Now;

        if ((now - lastTipTime).TotalMinutes >= 2)
        {
            var random = new Random();
            currentTip = tips[random.Next(tips.Count)];
            lastTipTime = now;

            // Отправляем новый совет всем пользователям
            foreach (var userId in userNames.Keys)
            {
                try
                {
                    await botClient.SendTextMessageAsync(userId, $"Новый совет дня: {currentTip}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка отправки сообщения: {ex.Message}");
                }
            }
        }
    }

    // Кастомный обработчик обновлений
    class UpdateHandler : IUpdateHandler
    {
        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message || update.Message?.Text == null)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            // Запрос имени пользователя
            if (!userNames.ContainsKey(chatId))
            {
                await bot.SendTextMessageAsync(chatId, "Как мне к вам обращаться?");
                userNames[chatId] = ""; // Временная запись
                return;
            }

            // Сохранение имени пользователя
            if (userNames[chatId] == "")
            {
                userNames[chatId] = messageText;
                await bot.SendTextMessageAsync(chatId, $"Приятно познакомиться, {messageText}!");

                return;
            }

            var userName = userNames[chatId];

            // Обработка команд
            switch (messageText.ToLower())
            {
                case "кнопки":
                    var buttons = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "Совет дня", "Привет" },
                        new KeyboardButton[] { "Картинка", "Видео", "Стикер" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                    await bot.SendTextMessageAsync(chatId, "Выберите действие:", replyMarkup: buttons);
                    break;

                case "привет":
                    await bot.SendTextMessageAsync(chatId, $"Здравствуй, {userName}!");
                    break;

                case "совет дня":
                    if (currentTip == "")
                        await UpdateTipIfNeeded(); // Если совет ещё не задан, обновляем
                    await bot.SendTextMessageAsync(chatId, $"Совет дня: {currentTip}");
                    break;

                case "картинка":
                    await bot.SendPhotoAsync(chatId, InputFile.FromUri("https://drive.google.com/uc?export=download&id=1HeXP2npezIoUl14h7t0q7ZmQk5mQVGO4\r\n"),
                        caption: "Вот ваша картинка!");
                    break;

                case "видео":
                    await bot.SendVideoAsync(chatId, InputFile.FromUri("https://drive.google.com/uc?export=download&id=186bLhdUmNu2Lyz7_s0ThTJGI5P0zOAYa"),
                        caption: "Вот ваше видео!");
                    break;

                case "стикер":
                    await bot.SendStickerAsync(chatId, InputFile.FromUri("https://drive.google.com/uc?export=download&id=1FMq4x9njben4ivekxM-AmMpTh9hK2Zkt\r\n"));
                    break;

                default:
                    await bot.SendTextMessageAsync(chatId, "Извините, я не понимаю эту команду. Попробуйте ещё раз.");
                    break;
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Ошибка в источнике {source}: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}





//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using Telegram.Bot;
//using Telegram.Bot.Polling;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.Enums;
//using Telegram.Bot.Types.ReplyMarkups;

//class Program
//{
//    static string token = "7064244196:AAFO5ows5pFqCXVNfLSHcymzyTEZnDILfsU"; 
//    static TelegramBotClient botClient;
//    static Dictionary<long, string> userNames = new Dictionary<long, string>(); // Словарь для хранения имен пользователей
//    static List<string> tips = new List<string>()
//    {
//        "Не забудь зарядить телефон перед выходом из дома.",
//        "Собака – не лучший советчик в финансах.",
//        "Улыбайся – это бесплатный аксессуар.",
//        "Сегодня лучше отдохнуть, чем делать ненужное.",
//        "Не клади яйца в одну корзину. А лучше вообще их съешь!"
//    };

//    static string currentTip = ""; // Текущий совет дня
//    static DateTime lastSentDate = DateTime.MinValue; // Дата последней рассылки

//    static async Task Main(string[] args)
//    {
//        botClient = new TelegramBotClient(token);
//        Console.WriteLine("Бот запущен...");

//        // Запуск таймера для автоматической рассылки советов
//        _ = Task.Run(async () =>
//        {
//            while (true)
//            {
//                await CheckAndSendDailyTips();
//                await Task.Delay(TimeSpan.FromMinutes(1)); // Проверка каждую минуту
//            }
//        });

//        // Настройка для обработки обновлений
//        var receiverOptions = new ReceiverOptions
//        {
//            AllowedUpdates = Array.Empty<UpdateType>() // Получать все типы обновлений
//        };

//        var handler = new UpdateHandler();

//        botClient.StartReceiving(
//            updateHandler: handler,
//            receiverOptions: receiverOptions,
//            cancellationToken: CancellationToken.None
//        );

//        Console.ReadLine();
//    }

//    // Проверка и отправка советов дня
//    static async Task CheckAndSendDailyTips()
//    {
//        var today = DateTime.Today;

//        if (lastSentDate != today) // Новый день
//        {
//            lastSentDate = today;
//            var random = new Random();
//            currentTip = tips[random.Next(tips.Count)];

//            foreach (var userId in userNames.Keys)
//            {
//                try
//                {
//                    await botClient.SendTextMessageAsync(userId, $"Совет дня: {currentTip}");
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Ошибка отправки сообщения: {ex.Message}");
//                }
//            }
//        }
//    }

//    // Кастомный обработчик обновлений
//    class UpdateHandler : IUpdateHandler
//    {
//        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
//        {
//            if (update.Type != UpdateType.Message || update.Message?.Text == null)
//                return;

//            var chatId = update.Message.Chat.Id;
//            var messageText = update.Message.Text;

//            // Запрос имени пользователя
//            if (!userNames.ContainsKey(chatId))
//            {
//                await bot.SendTextMessageAsync(chatId, "Как мне к вам обращаться?");
//                userNames[chatId] = ""; // Временная запись
//                return;
//            }

//            // Сохранение имени пользователя
//            if (userNames[chatId] == "")
//            {
//                userNames[chatId] = messageText;
//                await bot.SendTextMessageAsync(chatId, $"Приятно познакомиться, {messageText}!");

//                // Генерация и отправка совета дня
//                if (currentTip == "" || lastSentDate != DateTime.Today)
//                {
//                    await CheckAndSendDailyTips(); // Генерируем совет дня, если его еще нет
//                }
//                await bot.SendTextMessageAsync(chatId, $"Совет дня: {currentTip}");

//                return;
//            }

//            var userName = userNames[chatId];

//            // Обработка команд
//            switch (messageText.ToLower())
//            {
//                case "/start":
//                    var buttons = new ReplyKeyboardMarkup(new[]
//                    {
//                        new KeyboardButton[] { "Совет дня", "Привет" },
//                        new KeyboardButton[] { "Картинка", "Видео", "Стикер" }
//                    })
//                    {
//                        ResizeKeyboard = true
//                    };
//                    await bot.SendTextMessageAsync(chatId, $"Добро пожаловать, {userName}! Выберите действие:", replyMarkup: buttons);
//                    break;

//                case "привет":
//                    await bot.SendTextMessageAsync(chatId, $"Здравствуй, {userName}!");
//                    break;

//                case "совет дня":
//                    if (currentTip == "") await CheckAndSendDailyTips(); // Если совет ещё не отправлялся сегодня
//                    await bot.SendTextMessageAsync(chatId, $"Совет дня: {currentTip}");
//                    break;

//                case "картинка":
//                    await bot.SendPhotoAsync(chatId, InputFile.FromUri("https://drive.google.com/uc?export=download&id=1HeXP2npezIoUl14h7t0q7ZmQk5mQVGO4\r\n"),
//                        caption: "Вот ваша картинка!");
//                    break;

//                case "видео":
//                    await bot.SendVideoAsync(chatId, InputFile.FromUri("https://drive.google.com/uc?export=download&id=186bLhdUmNu2Lyz7_s0ThTJGI5P0zOAYa"),
//                        caption: "Вот ваше видео!");
//                    break;

//                case "стикер":
//                    await bot.SendStickerAsync(chatId, InputFile.FromUri("https://drive.google.com/uc?export=download&id=1FMq4x9njben4ivekxM-AmMpTh9hK2Zkt\r\n"));
//                    break;

//                default:
//                    await bot.SendTextMessageAsync(chatId, "Извините, я не понимаю эту команду. Попробуйте ещё раз.");
//                    break;
//            }
//        }

//        public Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
//        {
//            Console.WriteLine($"Ошибка в источнике {source}: {exception.Message}");
//            return Task.CompletedTask;
//        }
//    }
//}
