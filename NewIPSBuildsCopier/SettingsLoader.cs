using System.Xml.Serialization;

namespace IPSBuildsCopier
{
    /// <summary>
    /// Класс для загрузки настроек из XML-файла.
    /// </summary>
    public static class SettingsLoader
    {
        /// Путь к файлу настроек.
        private static readonly string settingsFilePath = "settings.xml";

        /// <summary>
        /// Загружает настройки из XML-файла.
        /// </summary>
        /// <returns>Объект настроек.</returns>
        /// <exception cref="InvalidOperationException">Исключение, если файл настроек не найден или произошла ошибка при загрузке настроек.</exception>
        public static Settings LoadSettings()
        {
            try
            {
                // Создаем экземпляр XmlSerializer для десериализации объекта типа Settings
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));

                // Открываем файл настроек для чтения
                using (FileStream fs = new FileStream(settingsFilePath, FileMode.Open))
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
                throw new InvalidOperationException($"Файл настроек не найден: {settingsFilePath}", ex);
            }
            catch (Exception ex)
            {
                // Обрабатываем любые другие ошибки, возникшие при загрузке настроек
                throw new InvalidOperationException("Ошибка при загрузке настроек", ex);
            }
        }
    }
}
