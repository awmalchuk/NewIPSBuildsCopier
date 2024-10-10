using Microsoft.Win32.TaskScheduler;

namespace IPSBuildsCopier
{
    /// <summary>
    /// Класс для работы с планировщиком задач Windows.
    /// </summary>
    public static class TaskSchedulerHelper
    {
        // Приватные константы для имени задачи и ее описания
        private const string TaskName = "IPS builds copy task";
        private const string TaskDescription = "Task to everyday copy builds on a scheduled basis";

        /// <summary>
        /// Добавляет задачу в планировщик задач Windows.
        /// </summary>
        /// <param name="settings">Настройки приложения.</param>
        public static void AddTask(Settings settings)
        {
            using (TaskService ts = new TaskService())
            {
                // Проверяем, существует ли уже задача с именем "IPS builds copy task"
                if (ts.FindTask(TaskName) != null)
                {
                    Console.WriteLine("Задача уже присутствует в планировщике задач Windows.");
                    return;
                }

                // Создаем новое задание
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = TaskDescription;

                // Настраиваем триггер для ежедневного запуска задачи
                DailyTrigger dailyTrigger = new DailyTrigger { DaysInterval = 1 };
                if (settings.TaskStart.IsHourly)
                {
                    // Если настройка Hourly = true, задача запускается ежечасно
                    dailyTrigger.Repetition.Interval = TimeSpan.FromHours(1);
                }
                else
                {
                    // Иначе задача запускается ежедневно в указанное время
                    dailyTrigger.StartBoundary = DateTime.Today + settings.TaskStart.TaskStartTime;
                    dailyTrigger.Repetition.Interval = TimeSpan.FromDays(1);
                }
                td.Triggers.Add(dailyTrigger);

                // Указываем путь к исполняемому файлу текущего приложения
                string exePath = PathHelper.GetExecutableFile().FullName;
                td.Actions.Add(new ExecAction(exePath, null, null));

                try
                {
                    // Регистрируем задачу в планировщике задач Windows
                    ts.RootFolder.RegisterTaskDefinition(TaskName, td);
                    Console.WriteLine(exePath);
                    Console.WriteLine("Задача добавлена в планировщик задач Windows.");
                }
                catch (Exception ex)
                {
                    // Обработка исключений при добавлении задачи
                    Console.WriteLine($"Ошибка при добавлении задачи: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Удаляет задачу из планировщика задач Windows.
        /// </summary>
        public static void RemoveTask()
        {
            using (TaskService ts = new TaskService())
            {
                // Проверяем, существует ли задача с именем "Builds copy task"
                if (ts.FindTask(TaskName) == null)
                {
                    Console.WriteLine("Задачи нет в планировщике.");
                    return;
                }

                try
                {
                    // Удаляем задачу из планировщика задач Windows
                    ts.RootFolder.DeleteTask(TaskName);
                    Console.WriteLine("Задача успешно удалена из планировщика задач Windows.");
                }
                catch (Exception ex)
                {
                    // Обработка исключений при удалении задачи
                    Console.WriteLine($"Ошибка при удалении задачи: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
