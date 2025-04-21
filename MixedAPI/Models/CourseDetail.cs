using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MixedAPI.Models
{
    public class CourseDetail
    {
        public string Id { get; set; }
        public int? CourseId { get; set; }
        public Course Course { get; set; }
        public string TitleAz { get; set; }
        public string TitleEn { get; set; }
        public string TitleRu { get; set; }

        public string DetailDescriptionAz { get; set; }
        public string DetailDescriptionEn { get; set; }
        public string DetailDescriptionRu { get; set; }

        public string TargetAudienceAz { get; set; }
        public string TargetAudienceEn { get; set; }
        public string TargetAudienceRu { get; set; }

        public string CurriculumAz { get; set; }
        public string CurriculumEn { get; set; }
        public string CurriculumRu { get; set; }

        public string FeaturesAz { get; set; }
        public string FeaturesEn { get; set; }
        public string FeaturesRu { get; set; }
    }

}
