using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Frilou_UI_V2.Models
{
    public class FormulaModel
    {
        [Required]
        [Display(Name = "floorThickness")]
        public string floorThickness { get; set; }

        [Required]
        [Display(Name = "wallThickness")]
        public string wallThickness { get; set; }
        [Required]
        [Display(Name = "rebarPercentage")]
        public string rebarPercentage { get; set; }
        [Required]
        [Display(Name = "nailInterval")]
        public string nailInterval { get; set; }
        [Required]
        [Display(Name = "hollowBlockConstant")]
        public string hollowBlockConstant { get; set; }
        [Required]
        [Display(Name = "supportBeamLength")]
        public string supportBeamLength { get; set; }
        [Required]
        [Display(Name = "supportBeamWidth")]
        public string supportBeamWidth { get; set; }
        [Required]
        [Display(Name = "concreteRatioCement")]
        public string concreteRatioCement { get; set; }
        [Required]
        [Display(Name = "concreteRatioSand")]
        public string concreteRatioSand { get; set; }
        [Required]
        [Display(Name = "concreteRatioAggregate")]
        public string concreteRatioAggregate { get; set; }
        [Required]
        [Display(Name = "riserHeight")]
        public string riserHeight { get; set; }
        [Required]
        [Display(Name = "threadDepth")]
        public string threadDepth { get; set; }
        [Required]
        [Display(Name = "stairsWidth")]
        public string stairsWidth { get; set; }
        [Required]
        [Display(Name = "wastage")]
        public string wastage { get; set; }
        [Required]
        [Display(Name = "provisions")]
        public string provisions { get; set; }


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
        public string LengthOfBuilding { get; set; }

        [Required]
        [Display(Name = "WidthOfBuilding")]
        public string WidthOfBuilding { get; set; }

        [Required]
        [Display(Name = "Materials")]
        public string Materials { get; set; }
    }


}