using System.Xml.Serialization;

namespace IPSBuildsCopier
{
    /// <summary>
    /// Контейнер для хранения настроек, десериализованных из settings.xml 
    /// </summary>
    [XmlRoot("Settings", IsNullable = false)]
    public class Settings
    {
        [XmlIgnore]
        public DateTime TaskStartTime { get; set; }

        [XmlElement("TaskStartTime", IsNullable = true)]
        public string TaskStartTimeString // Время ежедневного запуска задачи. Если null - то задача стартует ежечасно
        {
            get { return TaskStartTime.ToString("HH:mm:ss"); }
            set { TaskStartTime = DateTime.Parse(value); }
        }

        [XmlArray("BuildsList")]
        [XmlArrayItem("BuildInfo", IsNullable = false)]
        public required List<BuildInfo> BuildsList { get; set; } // Список билдов для копирования

        [XmlIgnore]
        public required DirectoryInfo TargetFolder { get; set; }

        [XmlElement("TargetFolder", IsNullable = false)]
        public string TargetFolderString // Директория на локальном диске для хранения дистрибутивов
        {
            get { return TargetFolder.FullName; }
            set { TargetFolder = new DirectoryInfo(value); }
        }
    }
}
