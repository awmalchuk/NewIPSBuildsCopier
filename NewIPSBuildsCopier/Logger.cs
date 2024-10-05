namespace IPSBuildsCopier
{
    public class Logger
    {
        // Объект FileInfo для файла лога
        private readonly FileInfo _logFile;

        public Logger(FileInfo logFile)
        {
            if (logFile == null)
            {
                throw new ArgumentNullException(nameof(logFile));
            }

            // Инициализируем объект FileInfo
            _logFile = logFile;
        }

        public async Task LogAsync(string message)
        {
            using (StreamWriter writer = new StreamWriter(_logFile.FullName, append: true))
            {
                // Записываем сообщение с текущей датой и временем
                // ConfigureAwait(false) используется для предотвращения возможных проблем с контекстом синхронизации.
                await writer.WriteLineAsync($"{DateTime.Now}: {message}").ConfigureAwait(false);
            }
        }
    }
}
