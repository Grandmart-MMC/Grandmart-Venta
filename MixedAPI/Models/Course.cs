namespace MixedAPI.Models
{
    public class Course
    {
        public int Id { get; set; }
        public CourseDetail CourseDetail { get; set; }

        // Hər dil üçün Title
        public string TitleAz { get; set; }
        public string TitleEn { get; set; }
        public string TitleRu { get; set; }

        // Hər dil üçün Description
        public string DescriptionAz { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionRu { get; set; }

        // ProgramDuration da müxtəlif dillərdə fərqli ola bilərsə
        public string ProgramDurationAz { get; set; }
        public string ProgramDurationEn { get; set; }
        public string ProgramDurationRu { get; set; }

        // Icon çox vaxt dəyişmir, dillə bağlı olmur (şəkil linki, fayl adı)
        public string Icon { get; set; }    

        // Participants rəqəmdir, dildən asılı deyil
        public int Participants { get; set; }
    }

}
