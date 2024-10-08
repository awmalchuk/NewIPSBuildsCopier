using Microsoft.Toolkit.Uwp.Notifications;

namespace IPSBuildsCopier
{
    /// <summary>
    /// Класс для отображения уведомлений.
    /// </summary>
    public static class NotificationHelper
    {
        /// <summary>
        /// Отображает уведомление с заданным заголовком и сообщением.
        /// </summary>
        /// <param name="title">Заголовок уведомления.</param>
        /// <param name="message">Сообщение уведомления.</param>
        public static void ShowNotification(string title, string message)
        {
            // Получаем путь к иконке, которая будет отображаться в уведомлении
            string iconPath = Path.Combine(Directory.GetCurrentDirectory(), "copy.png");

            // Создаем и настраиваем уведомление
            new ToastContentBuilder()
                .AddText(title) // Добавляем заголовок
                .AddText(message) // Добавляем сообщение
                .AddAppLogoOverride(new Uri(iconPath)) // Устанавливаем иконку
                .Show(); // Отображаем уведомление
        }
    }
}

