using System.Xml.Serialization;

namespace IPSBuildsCopier
{
    /// <summary>
    /// Класс для загрузки настроек из XML-файла.
    /// </summary>
    public static class SettingsLoader
    {
        // Имя файла с настройками
        private const string SettingsFileName = "settings.xml";

        /// <summary>
        /// Загружает настройки из XML-файла.
        /// </summary>
        /// <returns>Объект настроек.</returns>
        /// <exception cref="InvalidOperationException">Исключение, если файл настроек не найден или произошла ошибка при загрузке настроек.</exception>
        public static Settings LoadSettings()
        {
            FileInfo settingsFile = PathHelper.GetSettingsFile(SettingsFileName);

            try
            {
                // Создаем экземпляр XmlSerializer для десериализации объекта типа Settings
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));

                // Открываем файл настроек для чтения
                using (FileStream fs = settingsFile.OpenRead())
                {
                    // Десериализуем содержимое файла в объект типа Settings
                    var result = serializer.Deserialize(fs) as Settings;

                    // Проверяем, что десериализация прошла успешно
                    if (result == null)
                    {
                        throw new InvalidOperationException("Ошибка при десериализации: результат равен null.");
                    }

                    return result;
                }
            }
            catch (FileNotFoundException ex)
            {
                // Обрабатываем случай, когда файл настроек не найден
                throw new InvalidOperationException($"Файл настроек не найден: {settingsFile.FullName}.\n{ex.Message}");
            }
            catch (Exception ex)
            {
                // Обрабатываем любые другие ошибки, возникшие при загрузке настроек
                throw new InvalidOperationException($"Ошибка при загрузке настроек.\n{ex.Message}");
            }
        }
    }
}
