using System.Diagnostics;
using System.IO;

namespace IPSBuildsCopier
{
    /// <summary>
    /// Класс копирования дистрибутивов
    /// </summary>
    public class BuildCopier
    {
        // Поле для хранения данных полученных из settings.xml
        private Settings _settings;

        public BuildCopier(Settings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Асинхронно копируем все дистрибутивы из списка <BuildsList> в settings.xml
        /// </summary>
        /// <param name="settings">Настройки копирования сборок.</param>
        public async Task CopyBuildsAsync()
        {
            foreach (var buildInfo in _settings.BuildsList)
            {
                try
                {
                    // Пробуем копировать папку с дистрибутивом
                    await CopyBuildDirectoryAsync(buildInfo, _settings.TargetFolder);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка копирования дистрибутива {buildInfo.BuildName}: {ex.Message}\n");
                }
            }
        }

        /// <summary>
        /// Асинхронно копируем папку с дистрибутивом
        /// </summary>
        /// <param name="buildInfo">Информация о сборке.</param>
        /// <param name="targetDir">Целевая директория.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="IOException"></exception>
        private async Task CopyBuildDirectoryAsync(BuildInfo buildInfo, DirectoryInfo targetDir)
        {
            // Получаем имя дистрибутива
            var buildName = buildInfo.BuildName;
            // Получаем инфу о сетевой папке с дистрибутивом билда
            var sourceDir = buildInfo.NetworkPath;

            // Проверяем, существует ли сетевая директория c дистрибутивом билда.
            if (!sourceDir.Exists)
            {
                throw new DirectoryNotFoundException($"Исходная директория не найдена: {sourceDir.FullName}");
            }

            // Получаем номер сборки билда
            var currentBuildVersion = GetBuildNumber(buildInfo.BuildVersionInfoPath);
            // Задаём путь к папке назначения (\<Дистрибутив>\<Номер билда>)
            var destinationDir = new DirectoryInfo(Path.Combine(targetDir.FullName, buildName, currentBuildVersion));

            // Проверяем актуальность локальной копии билда. Если копия актуальна, то ее не перезаписываем
            if (IsBuildCopyActual(currentBuildVersion, destinationDir))
            {
                Console.WriteLine($"Копия билда {buildInfo.BuildName} уже актуальна.\n");
                return;
            }

            //TODO Навести тут порядок
            // Начало блока копирования
            Console.WriteLine($"Копирую {buildInfo.BuildName}...");
            // Копируем содержимое директории
            await CopyDirectoryContentsAsync(sourceDir, destinationDir);
            // В файл version.txt записывае номер сборки  билда
            await File.WriteAllTextAsync(Path.Combine(destinationDir.FullName, "version.txt"), currentBuildVersion);
            Console.WriteLine("Копирование завершено.\n");
            // Конец блока копирования
        }

        /// <summary>
        /// Рекурсивно копируем содержание папки с дистрибутивом
        /// </summary>
        /// <param name="sourceDir">Сетевая папка с дистрибутивом билда</param>
        /// <param name="destinationDir">Локальная папка для хранения копии билда</param>
        /// <returns></returns>
        private async Task CopyDirectoryContentsAsync(DirectoryInfo sourceDir, DirectoryInfo destinationDir)
        {
            #region Реализация асинхронного копирования (IPS9 - 1м 30 сек)
            //// Создаем целевую директорию, если она не существует
            //if (!destinationDir.Exists)
            //{
            //    TryCreateLocalDir(destinationDir);
            //}

            //// Копируем файлы из исходной директории в целевую
            //foreach (FileInfo file in sourceDir.GetFiles())
            //{
            //    // Генерируем для файла новый путь в целевую директорию
            //    var targetFilePath = Path.Combine(destinationDir.FullName, file.Name);
            //    // Асинхронно копируем файл, используя метод CopyFileAsync
            //    await CopyFileAsync(file, targetFilePath);
            //}

            //// Вызываем этот метод рекурсивно для каждой поддиректории
            //foreach (DirectoryInfo subDir in sourceDir.GetDirectories())
            //{
            //    // Создаём поддиректорию в целевой директории
            //    DirectoryInfo newDestinationDir = destinationDir.CreateSubdirectory(subDir.Name);
            //    // Рекурсивно копируем содержимое поддиректории
            //    await CopyDirectoryContentsAsync(subDir, newDestinationDir);
            //}
            #endregion

            #region Реализация асинхронного параллельного копирования v2 (IPS9 - 1м 15сек)
            //// Получаем все файлы в исходной директории и поддиректориях
            //var files = sourceDir.GetFiles("*", SearchOption.AllDirectories);
            //var tasks = new List<Task>();

            //foreach (var file in files)
            //{
            //    // Определяем относительный путь файла
            //    var relativePath = Path.GetRelativePath(sourceDir.FullName, file.FullName);
            //    // Определяем путь к целевому файлу
            //    var destinationPath = Path.Combine(destinationDir.FullName, relativePath);
            //    var destinationFile = new FileInfo(destinationPath);

            //    // Проверяем, что директория не null и создаем ее, если она не существует
            //    var directory = destinationFile.Directory;
            //    if (directory != null && !directory.Exists)
            //    {
            //        TryCreateLocalDir(directory);
            //    }

            //    // Добавляем задачу копирования файла в список задач
            //    tasks.Add(CopyFileAsync(file, destinationPath));
            //}

            //// Ожидаем завершения всех задач копирования файлов
            //await Task.WhenAll(tasks);
            #endregion

            #region Реализация асинхронного параллельного копирования v3 LINQ (IPS9 - 1 м 20 сек)
            // Получаем все файлы и поддиректории в исходной сетевой директории с дистрибутивом
            var files = sourceDir.GetFiles(); // Получаем массив всех файлов в исходной директории
            var directories = sourceDir.GetDirectories(); // Получаем массив всех поддиректорий в исходной директории

            // Создаем целевую директорию, если она не существует
            if (!destinationDir.Exists) // Проверяем, существует ли целевая директория
            {
                TryCreateLocalDir(destinationDir); // Если не существует, создаем ее
            }

            // Копируем файлы параллельно
            await Task.WhenAll(files.Select(file =>
                CopyFileAsync(file, Path.Combine(destinationDir.FullName, file.Name))));
            // Для каждого файла в исходной директории создаем задачу копирования файла в целевую директорию
            // Path.Combine(destinationDir.FullName, file.Name) формирует полный путь к файлу в целевой директории
            // Task.WhenAll запускает все задачи копирования параллельно и ожидает их завершения

            // Копируем поддиректории параллельно
            await Task.WhenAll(directories.Select(async subDir =>
            {
                var newDestinationDir = destinationDir.CreateSubdirectory(subDir.Name); // Создаем поддиректорию в целевой директории
                await CopyDirectoryContentsAsync(subDir, newDestinationDir); // Рекурсивно копируем содержимое поддиректории
            }));
            // Для каждой поддиректории в исходной директории создаем задачу копирования содержимого поддиректории в новую поддиректорию целевой директории
            // Task.WhenAll запускает все задачи копирования параллельно и ожидает их завершения
            #endregion
        }

        /// <summary>
        /// Асинхронно копирует файл.
        /// </summary>
        /// <param name="file">Исходный файл.</param>
        /// <param name="destinationPath">Путь к целевому файлу.</param>
        private async Task CopyFileAsync(FileInfo file, string destinationPath)
        {
            try
            {
                // Открываем исходный файл для чтения
                using (var sourceStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 65536, useAsync: true))
                {
                    // Создаем поток для записи в целевой файл
                    // Размер буфера по умолчанию 4Кб (4096); 65536
                    using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 65536, useAsync: true))
                    {
                        // Устанавливаем длину целевого файла
                        destinationStream.SetLength(file.Length);

                        // Асинхронно копируем данные из исходного потока в целевой
                        await sourceStream.CopyToAsync(destinationStream);
                    }
                }

                //await Task.Run(()=> file.CopyTo(destinationPath, true));
            }
            catch (Exception ex)
            {
                // Логируем ошибку, если что-то пошло не так
                Console.WriteLine($"Ошибка при копировании файла {file.Name}: {ex.Message}");
            }
        }


        /// <summary>
        /// Получает номер сборки из файла с информацией о версии.
        /// </summary>
        /// <param name="buildVersionInfoPath">Путь к файлу с информацией о версии.</param>
        /// <returns>Номер сборки.</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        private string GetBuildNumber(FileInfo buildVersionInfoPath)
        {
            // Проверяем наличие файла у которого будем номер билда
            if (!buildVersionInfoPath.Exists)
            {
                throw new FileNotFoundException($"Файл с информацией о версии билда не найден: {buildVersionInfoPath}");
            }

            // Получаем полную инфу о сборке
            FileVersionInfo currentBuildVersionInfo = FileVersionInfo.GetVersionInfo(buildVersionInfoPath.FullName);

            // Получаем номер сборки (он может быть и null)
            string? currentBuildVersion = currentBuildVersionInfo.ProductVersion;

            // Проверяем номер сборки
            if (currentBuildVersion == null)
            {
                throw new ArgumentNullException("Номер сборки не может быть null");
            }
            return currentBuildVersion;
        }

        /// <summary>
        /// Получает время последнего изменения файла
        /// </summary>
        /// <param name="buildVersionInfoPath">Путь к файлу</param>
        /// <returns></returns>
        private string GetLastModifiedInfo(FileInfo buildVersionInfoPath)
        {
            // Получаем дату и время последнего изменения файла
            DateTime lastModifiedTime = buildVersionInfoPath.LastWriteTime;
            string formattedDate = lastModifiedTime.ToString("dd.MM.yyyy_HH.mm");
            return formattedDate;
        }

        /// <summary>
        /// Проверяет, актуальна ли локальная копия сборки.
        /// </summary>
        /// <param name="currentBuildVersion">Текущая версия сборки.</param>
        /// <param name="destinationDir">Целевая директория.</param>
        /// <returns>True, если локальная копия актуальна, иначе False.</returns>
        private bool IsBuildCopyActual(string currentBuildVersion, DirectoryInfo destinationDir)
        {
            // Задаём путь для создания файла "version.txt"
            var versionFile = new FileInfo(Path.Combine(destinationDir.FullName, "version.txt"));

            // Если этого файла ещё нет 
            if (!versionFile.Exists)
            {
                return false;
            }

            // Читаем из него записанные данные о номере сборки
            string versionContent = File.ReadAllText(versionFile.FullName);

            // Проверяем актуальность локальной папки со дистрибутивом
            return versionContent == currentBuildVersion;
        }

        /// <summary>
        /// Пробуем создать папку на локальном диске
        /// </summary>
        /// <param name="targetDir">Имя локальной папки</param>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="IOException"></exception>
        private void TryCreateLocalDir(DirectoryInfo targetDir)
        {
            // Проверяем, существует ли целевая директория
            if (!targetDir.Exists)
            {
                try
                {
                    // Пытаемся создать целевую директорию
                    targetDir.Create();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка ввода-вывода при создании целевой директории.\n{ex.Message}");
                }
            }
        }

        /// <summary>
        /// Асинхронно получает общее количество элементов (файлов и папок) в указанной директории.
        /// </summary>
        /// <param name="directory">Исходная директория для подсчета элементов.</param>
        /// <returns>Общее количество файлов и папок в директории и всех её поддиректориях.</returns>
        private async Task<int> GetTotalItemCountAsync(DirectoryInfo directory)
        {
            // Начальное количество элементов: количество файлов и папок в текущей директории
            int count = directory.GetFiles().Length + directory.GetDirectories().Length;

            // Рекурсивно обходим все поддиректории
            foreach (DirectoryInfo subDir in directory.GetDirectories())
            {
                // Асинхронно добавляем количество элементов в поддиректории к общему количеству
                count += await GetTotalItemCountAsync(subDir);
            }

            // Возвращаем общее количество элементов
            return count;
        }
    }
}
