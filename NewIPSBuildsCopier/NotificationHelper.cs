using Microsoft.Toolkit.Uwp.Notifications;
using Serilog;

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
            //string iconPath = Path.Combine(Directory.GetCurrentDirectory(), "copy.png");
            string iconPath = Path.Combine(PathHelper.GetExecutableDirectory().FullName, "copy.png");

            // Создаем и настраиваем уведомление
            new ToastContentBuilder()
                .AddText(title) // Добавляем заголовок
                .AddText(message) // Добавляем сообщение
                .AddAppLogoOverride(new Uri(iconPath)) // Устанавливаем иконку
                .Show(); // Отображаем уведомление
        }

        // Информационное уведомление
        public static void ShowInfoNotificationAndLog(string title, string message)
        {
            ShowNotification(title,message);
            Log.Information(message);
        }

        // Уведомление о предупреждении
        public static void ShowWarningNotificationAndLog(string title, string message)
        {
            ShowNotification(title, message);
            Log.Warning(message);
        }

        // Уведомление об ошибке
        public static void ShowErrorNotificationAndLog(string title, string message, Exception ex)
        {
            ShowNotification(title, message);
            Log.Warning(ex, message);
        }
    }
}

