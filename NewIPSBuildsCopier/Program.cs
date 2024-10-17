using Serilog;

// Версию текущего билда получаю из параметров файла Intermech.Interfaces.dll(лежит в папке \InstServer\Server\)
// Алгоритм:
// Получаем номер билда IPS, на локальном диске по указанному в xml файле пути - в папке с именем билда создаём подпапку с именем текущей версии билда.
// Создаём в этой папке файл version.txt и в него пишем номер текущего билда.
// В последствии проверяем наличие этого файла перед копированием и если текущая версия бидла равно версии билда, прочитанной из файла - 
// повторно билд IPS не копируем.

namespace IPSBuildsCopier
{
   public class Program
    {
        static async Task Main(string[] args)
        {
            // Инициализируем Serilog
            SerilogHelper.InitializeLogger();

            try
            {
                // Загрузка настроек из файла settings.xml
                Settings settings = SettingsLoader.LoadSettings();

                // Обработка аргументов командной строки
                await HandleCommandLineArgsAsync(args, settings);
            }
            catch (Exception ex)
            {
                // Обработка исключений и вывод сообщения об ошибке
                NotificationHelper.ShowErrorNotificationAndLog("Ошибка", $"Ошибка: ", ex);
            }
        }

        /// <summary>
        /// Обрабатывает аргументы командной строки и выполняет соответствующие действия.
        /// </summary>
        /// <param name="args">Аргументы командной строки.</param>
        /// <param name="settings">Настройки приложения.</param>
        private static async Task HandleCommandLineArgsAsync(string[] args, Settings settings)
        {
            if (args.Length == 0)
            {
                // Если аргументов нет, запускаем основное приложение
                await RunCopierAsync(settings);
                return;
            }

            switch (args[0].ToLower())
            {
                case "addtask":
                    // Если аргумент "addtask", добавляем задачу в планировщик
                    TaskSchedulerHelper.AddTask(settings);
                    break;
                case "removetask":
                    // Если аргумент "removetask", удаляем задачу из планировщика
                    TaskSchedulerHelper.RemoveTask();
                    break;
                default:
                    throw new ArgumentException("Неизвестный аргумент командной строки.");
            }
        }

        /// <summary>
        /// Запускает основное приложение.
        /// </summary>
        /// <param name="settings">Настройки приложения.</param>
        private static async Task RunCopierAsync(Settings settings)
        {
            // Создаём экземпляр класса копировальщика дистрибутивов
            var buildCopier = new BuildCopier(settings);

            // Запускем задачу копирования дистрибутивов асинхронно
            await buildCopier.CopyBuildsAsync();

            // Выводим уведомление и пишем в лог
            NotificationHelper.ShowInfoNotificationAndLog("Копирование завершено", "Все задачи копирования завершены.");

            // Финализирую работу Serilog
            Log.CloseAndFlush();

            // Пауза 5 сек
            Thread.Sleep(5000);
        }
    }
}
