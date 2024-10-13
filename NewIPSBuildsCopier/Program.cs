using Serilog;

namespace IPSBuildsCopier
{
   public class Program
    {
        static async Task Main(string[] args)
        {
            //// string xmlFilePath = "settings.xml";

            // // Десерилизация XML-файла
            //Settings settings = SettingsLoader.LoadSettings();

            //// Проверка десерилизованных данных
            //Console.WriteLine($"Task Start Time: {settings.TaskStart.TaskStartTime}\n");
            //Console.WriteLine($"Task Start Hourly: {settings.TaskStart.IsHourly}\n");
            //Console.WriteLine($"Target Folder: {settings.TargetFolder} \n");
            //foreach (var build in settings.BuildsList)
            //{
            //    Console.WriteLine($"Build Name: {build.BuildName}");
            //    Console.WriteLine($"Network Path: {build.NetworkPath}");
            //    Console.WriteLine($"Build Version Info Path: {build.BuildVersionInfoPath}\n");
            //}

            //Console.WriteLine($"Добавляем задачу в планировщик задач");
            //TaskSchedulerHelper.AddTask(settings);

            //Console.ReadKey();

            //Console.WriteLine($"Удаляем задачу из планировщика задач");
            //TaskSchedulerHelper.RemoveTask();

            SerilogHelper.InitializeLogger();

            var settings = SettingsLoader.LoadSettings();
            var buildCopier = new BuildCopier(settings);
            await buildCopier.CopyBuildsAsync();
            // Console.WriteLine("Копирование завершено.");
            // Log.Information("Всезадачи копирования завершены.");
            NotificationHelper.ShowInfoNotificationAndLog("Копирование завершено", "Все задачи копирования завершены.");

            Console.ReadKey();
        }
    }
}
