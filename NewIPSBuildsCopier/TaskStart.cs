using System.Xml.Serialization;

namespace IPSBuildsCopier
{
    /// <summary>
    /// Вспомогательный класс для элемента TaskStart.
    /// </summary>
    public class TaskStart
    {
        [XmlIgnore]
        public required TimeSpan TaskStartTime { get; set; }

        [XmlElement("TaskStartTime", IsNullable = false)]
        public string TaskStartTimeString // Время ежедневного запуска задачи. 
        {
            get { return TaskStartTime.ToString(@"hh\:mm\:ss"); }
            set { TaskStartTime = TimeSpan.Parse(value); }
        }

        [XmlElement("IsHourly", IsNullable = false)]
        public required bool IsHourly { get; set; } // true - задача запускается ежечасно, false - однократно, во время указанное в параметре TaskStartTime
    }
}
