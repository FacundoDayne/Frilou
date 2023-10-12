using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Frilou_UI_V2.Models
{
    public class TemplateModel
    {

        [Required]
        [Display(Name = "TemplateName")]
        public string TemplateName { get; set; }

        [Required]
        [Display(Name = "TemplateDescription")]
        public string TemplateDescription { get; set; }

        [Required]
        [Display(Name = "TemplateID")]
        public string TemplateID { get; set; }

        [Required]
        [Display(Name = "HeightOfStoreys")]
        public string HeightOfStoreys { get; set; }

        [Required]
        [Display(Name = "LengthOfBuilding")]
        public string LengthOfBuildinge { get; set; }

        [Required]
        [Display(Name = "WidthOfBuilding")]
        public string WidthOfBuilding { get; set; }

        [Required]
        [Display(Name = "Materials")]
        public string Materials { get; set; }

        
    }
}
