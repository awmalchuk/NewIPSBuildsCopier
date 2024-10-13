using System.Xml.Serialization;

namespace IPSBuildsCopier
{
    /// <summary>
    ///  Контейнер для хранения десерелиазованной из settihgs.xml информации об скачеваемых билдах
    /// </summary>
    [XmlType("BuildInfo")]
    public class BuildInfo
    {
        [XmlElement("BuildName", IsNullable = false)]
        public required string BuildName { get; set; } // Имя билда

        [XmlIgnore]
        public required DirectoryInfo NetworkPath { get; set; }

        [XmlElement("NetworkPath", IsNullable = false)]
        public required string NetworkPathString // Сетевой путь к дистрибутиву на сервер сборки
        {
            get { return NetworkPath.FullName; }
            set { NetworkPath = new DirectoryInfo(value); }
        }

        [XmlIgnore]
        public required FileInfo BuildVersionInfoPath { get; set; }

        [XmlElement("BuildVersionInfoPath", IsNullable = false)]
        public required string BuildVersionInfoPathString // Сетевой путь к файлу-источнику информации о номере сборки билда
        {
            get { return BuildVersionInfoPath.FullName; }
            set { BuildVersionInfoPath = new FileInfo(value); }
        }

        [XmlElement("UseFileLastModifiedInfo", IsNullable = false)]
        public required bool UseFileLastModifiedInfo { get; set; }
    }
}
