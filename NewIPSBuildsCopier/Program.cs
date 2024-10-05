namespace IPSBuildsCopier
{
   public class Program
    {
        static async Task Main(string[] args)
        {
            //// string xmlFilePath = "settings.xml";

            // // Десерилизация XML-файла
            // Settings settings = SettingsLoader.LoadSettings();

            // // Проверка десерилизованных данных
            // Console.WriteLine($"Task Start Time: {settings.TaskStartTime}\n");
            // Console.WriteLine($"Target Folder: {settings.TargetFolder} \n");
            // foreach (var build in settings.BuildsList)
            // {
            //     Console.WriteLine($"Build Name: {build.BuildName}");
            //     Console.WriteLine($"Network Path: {build.NetworkPath}");
            //     Console.WriteLine($"Build Version Info Path: {build.BuildVersionInfoPath}\n");
            // }

            var settings = SettingsLoader.LoadSettings();
            await BuildCopier.CopyBuildsAsync(settings);
            Console.WriteLine("All done.");

            Console.ReadKey();
        }
    }
}
