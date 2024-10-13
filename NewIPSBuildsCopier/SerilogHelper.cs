using Serilog;

namespace IPSBuildsCopier
{
    public static class SerilogHelper
    {
        /// <summary>
        /// Инициализирует логгер Serilog с настройками для вывода в консоль и файл.
        /// </summary>
        public static void InitializeLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Устанавливаем минимальный уровень логирования
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}") // Настраиваем вывод логов в консоль с шаблоном
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}") // Настраиваем вывод логов в файл с ежедневной ротацией и шаблоном
                .CreateLogger(); // Создаем логгер
        }
    }
}
