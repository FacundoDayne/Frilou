using System.ComponentModel.DataAnnotations;

namespace Frilou_UI_V2.Models
{
	public class EmployeeNewProject
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public string ClientName { get; set; }
		public DateTime Date { get; set; }
	}
	public class EmployeeDashboardModel
	{
		public List<EmployeeNewProject> projects { get; set; }
	}

	public class NewTemplateModel
	{
		[Display(Name = "Name")]
		public string Descritpion { get; set; }
		[Display(Name = "Description")]
		public string Long_Description { get; set; }

		[Display(Name = "Floor Thickness")]
		public double floorThickness { get; set; }

		[Display(Name = "Wall Thickness")]
		public double wallThickness { get; set; }

		[Display(Name = "Rebar diameter")]
		public double rebarDiameter { get; set; }

		[Display(Name = "Nail constant")]
		public double nailConstant { get; set; }

		[Display(Name = "Hollow block constant")]
		public double hollowBlockConstant { get; set; }

		[Display(Name = "Support beam length")]
		public double supportBeamLength { get; set; }

		[Display(Name = "Support beam width")]
		public double supportBeamWidth { get; set; }

		[Display(Name = "Support beam interval")]
		public double supportBeamSpace { get; set; }

		[Display(Name = "Concrete Cement Ratio")]
		public double concreteRatioCement { get; set; }

		[Display(Name = "Concrete Sand Ratio")]
		public double concreteRatioSand { get; set; }

		[Display(Name = "Concrete Aggregate Ratio")]
		public double concreteRatioAggregate { get; set; }

		[Display(Name = "Plywood Length")]
		public double plywoodLength { get; set; }

		[Display(Name = "Plywood Width")]
		public double plywoodWidth { get; set; }

		[Display(Name = "Stairs riser height")]
		public double riserHeight { get; set; }

		[Display(Name = "Stairs thread depth")]
		public double threadDepth { get; set; }

		[Display(Name = "Wastage")]
		public double wasteage { get; set; }

		[Display(Name = "Provisions")]
		public double provisions { get; set; }
	}
	public class TemplateListItem
	{
		public int ID { get; set; }
		public string Descritpion { get; set; }
		public string Long_Description { get; set; }
	}
	public class EditTemplateModel
	{
		public int ID { get; set; }
		public int FormulaID { get; set; }
		[Display(Name = "Name")]
		public string Descritpion { get; set; }
		[Display(Name = "Description")]
		public string Long_Description { get; set; }

		[Display(Name = "Floor Thickness")]
		public double floorThickness { get; set; }

		[Display(Name = "Wall Thickness")]
		public double wallThickness { get; set; }

		[Display(Name = "Rebar diameter")]
		public double rebarDiameter { get; set; }

		[Display(Name = "Nail constant")]
		public double nailConstant { get; set; }

		[Display(Name = "Hollow block constant")]
		public double hollowBlockConstant { get; set; }

		[Display(Name = "Support beam length")]
		public double supportBeamLength { get; set; }

		[Display(Name = "Support beam width")]
		public double supportBeamWidth { get; set; }

		[Display(Name = "Support beam interval")]
		public double supportBeamSpace { get; set; }

		[Display(Name = "Concrete Cement Ratio")]
		public double concreteRatioCement { get; set; }

		[Display(Name = "Concrete Sand Ratio")]
		public double concreteRatioSand { get; set; }

		[Display(Name = "Concrete Aggregate Ratio")]
		public double concreteRatioAggregate { get; set; }

		[Display(Name = "Plywood Length")]
		public double plywoodLength { get; set; }

		[Display(Name = "Plywood Width")]
		public double plywoodWidth { get; set; }

		[Display(Name = "Stairs riser height")]
		public double riserHeight { get; set; }

		[Display(Name = "Stairs thread depth")]
		public double threadDepth { get; set; }

		[Display(Name = "Wastage")]
		public double wasteage { get; set; }

		[Display(Name = "Provisions")]
		public double provisions { get; set; }
	}



	public class Employee_BOM_Template_List
	{
		public string ID { get; set; }
		public string Description { get; set; }
	}

	public class Employee_BOM_Materials_Subitems
	{
		public int SubitemNumber { get; set; }
		public int MaterialID { get; set; }
		public string MaterialDesc { get; set; }
		public string MaterialUoM { get; set; }
		public int MaterialQuantity { get; set; }
		public int MaterialQuantityWastage { get; set; }
		public int MaterialQuantityProvisions { get; set; }
		public double MaterialCost { get; set; }
		public double MaterialAmount { get; set; }
		public double LabourCost { get; set; }
		public double TotalUnitRate { get; set; }
	}

	public class Employee_BOM_Materials_Items
	{
		public IList<Employee_BOM_Materials_Subitems> Subitems { get; set; }
		public int ItemNumber { get; set; }
		public string Description { get; set; }
	}

	public class Employee_BOM_Materials_Lists
	{
		public IList<Employee_BOM_Materials_Items> Items { get; set; }
		public char ListLetter { get; set; }
		public int ListNumber { get; set; }
		public string Description { get; set; }
	}

	public class EmployeeBOMModel
	{
		public List<Employee_BOM_Materials_Lists> lists { get; set; }
		public List<Employee_BOM_Template_List> templates { get; set; }

		public double totalCost { get; set; }

		public int ProjectID { get; set; }
		[Required]
		[Display(Name = "Project Title")]
		public string TemplateID { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Project Title")]
		public string Title { get; set; }

		[StringLength(100, MinimumLength = 0)]
		[Required]
		[Display(Name = "Client Name")]
		public string ClientName { get; set; }

		[StringLength(100, MinimumLength = 0)]
		[Required]
		[Display(Name = "Client Contact")]
		public string ClientContact { get; set; }

		[Required]
		[Display(Name = "Project Date")]
		public DateTime Date { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Project Address")]
		public string Address { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Project City")]
		public string City { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Project Region")]
		public string Region { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Project Country")]
		public string Country { get; set; }

		[Required]
		[Display(Name = "Longtitude")]
		public string Longtitude { get; set; }

		[Required]
		[Display(Name = "Latitude")]
		public string Latitude { get; set; }

		[Required]
		[Display(Name = "Building Type")]
		public string BuildingType { get; set; }

		[Required]
		[Display(Name = "Number of Storeys")]
		[Range(1, 1000000000)]
		public int NumberOfStoreys { get; set; }

		[Required]
		[Display(Name = "Height of floors")]
		[Range(1, 1000000000)]
		public double FloorHeight { get; set; }

		[Required]
		[Display(Name = "Building Length")]
		[Range(1, 1000000000)]
		public double BuildingLength { get; set; }

		[Required]
		[Display(Name = "Building Width")]
		[Range(1, 1000000000)]
		public double BuildingWidth { get; set; }
	}
}
