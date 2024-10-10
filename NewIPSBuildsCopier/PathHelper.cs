namespace IPSBuildsCopier
{
    /// <summary>
    /// Класс для получения путей к различным ресурсам приложения.
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Возвращает объект FileInfo для исполняемого файла.
        /// </summary>
        /// <returns>Объект FileInfo для исполняемого файла.</returns>
        /// <exception cref="InvalidOperationException">Исключение, если не удалось определить путь к исполняемому файлу.</exception>
        public static FileInfo GetExecutableFile()
        {
            string exePath = Environment.ProcessPath ?? throw new InvalidOperationException("Не удалось определить путь к исполняемому файлу.");
            return new FileInfo(exePath);
        }

        /// <summary>
        /// Возвращает объект DirectoryInfo для директории, в которой находится исполняемый файл.
        /// </summary>
        /// <returns>Объект DirectoryInfo для директории исполняемого файла.</returns>
        /// <exception cref="InvalidOperationException">Исключение, если не удалось определить директорию исполняемого файла.</exception>
        public static DirectoryInfo GetExecutableDirectory()
        {
            FileInfo exeFile = GetExecutableFile();
            return exeFile.Directory ?? throw new InvalidOperationException("Не удалось определить директорию исполняемого файла.");
        }

        /// <summary>
        /// Возвращает объект FileInfo для файла настроек.
        /// </summary>
        /// <param name="SettingsFileName"></param>
        /// <returns>Объект FileInfo для файла settings.xml</returns>
        public static FileInfo GetSettingsFile(string SettingsFileName)
        {
            DirectoryInfo exeDir = GetExecutableDirectory();
            return new FileInfo(Path.Combine(exeDir.FullName, SettingsFileName));
        }
    }
}