using System.ComponentModel.DataAnnotations;

namespace Frilou_UI_V2.Models
{
	public class MaterialCategories
	{
		public string ID { get; set; }
		public string Description { get; set; }
	}
	public class MeasurementTypes
	{
		public string ID { get; set; }
		public string Description { get; set; }
	}
	public class MeasurementUnits
	{
		public string ID { get; set; }
		public string Description { get; set; }
		public string Description_Plural { get; set; }
		public string Description_Abrev { get; set; }
		public int Type { get; set; }
	}

	public class AddMaterialModel
	{
		public IList<MaterialCategories>? CategoryList { get; set; }
		public IList<MeasurementTypes>? MeasurementTypeList { get; set; }
		public IList<MeasurementUnits>? MeasurementUnitList { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 1)]
		[Display(Name = "Description")]
		public string Description { get; set; }

		[Required]
		[StringLength(255, MinimumLength = 1)]
		[Display(Name = "Long Description")]
		public string Description_Long { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Category")]
		public int CategoryID { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Type")]
		public int MeasurementType { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Unit")]
		public int MeasurementID { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double MeasurementValue { get; set; }

	}

	public class EditMaterialModel
	{
		public IList<MaterialCategories>? CategoryList { get; set; }
		public IList<MeasurementTypes>? MeasurementTypeList { get; set; }
		public IList<MeasurementUnits>? MeasurementUnitList { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "ID")]
		public uint ID { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 1)]
		[Display(Name = "Description")]
		public string Description { get; set; }

		[Required]
		[StringLength(255, MinimumLength = 1)]
		[Display(Name = "Long Description")]
		public string Description_Long { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Category")]
		public int CategoryID { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Type")]
		public int MeasurementType { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Unit")]
		public int MeasurementID { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double MeasurementValue { get; set; }

	}

	public class MaterialsEditViewItem
	{
		public uint ID { get; set; }
		public string Description { get; set; }
	}

	public class MaterialsEditViewModel
	{
		public IList<MaterialsEditViewItem> items { get; set; }
	}



	public class SupplierMaterialViewItems
	{
		public bool IsAvailable { get; set; }

		public uint ID { get; set; }

		public string Description_Long { get; set; }
		public string MeasurementString { get; set; }
		public string MeasurementValue { get; set; }

		public double Price { get; set; }

	}

	public class SupplierMaterialViewModel
	{
		public IList<SupplierMaterialViewItems> items { get; set; }
	}

	public class MaterialsCostListModel
	{
		public string Description_Long { get; set; }
		public string Supplier_Desc { get; set; }
		public double Price { get; set; }

	}

	public class MaterialsListModel
	{
		public uint ID { get; set; }
		public string Description { get; set; }
		public string Description_Long { get; set; }
		public uint Category_ID { get; set; }
		public string Category_Desc { get; set; }
		public uint UoM_ID { get; set; }
		public string UoM_Desc { get; set; }
		public uint MeasurementType { get; set; }
		public double MeasurementValue { get; set; }
	}

	public class MaterialsCostComparisonItem
	{
		public uint MaterialID { get; set; }
		public string Description_Long { get; set; }
		public double Price { get; set; }
		public uint SupplierID { get; set; }
		public string SupplierDesc { get; set; }
		public string SupplierCoords { get; set; }
		public double Distance { get; set; }
	}

	public class FormulaConstantsViewModel
	{
		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double floorThickness { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double wallThickness { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double rebarThickness { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double nailInterval { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double hollowBlockConstant { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double supportBeamLength { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double supportBeamWidth { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double supportBeamInterval { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double concreteRatioCement { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double concreteRatioSand { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double concreteRatioAggregate { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double plywoodLength { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double plywoodWidth { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double riserHeight { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double threadDepth { get; set; }

		[Required]
		[Range(0, 1000000000)]
		[Display(Name = "Measurement Value")]
		public double overorder { get; set; }
	}

	public class BOMMaterialItem
	{
		public int ID { get; set; }
		public string Description { get; set; }
		[Required]
		public bool IsUsed { get; set; } = false;
	}

	public class BOMListOfMaterials
	{
		public string ProjectType { get; set; }
		public IList<BOMMaterialItem> Materials { get; set; }
		public double Width { get; set; } = 0;
		public double Length { get; set; } = 0;
		public int Storeys { get; set; } = 0;
		public double StoreyHeight { get; set; } = 0;
	}

	public class BOMTemplate
	{
		public int ID { get; set; }
		public string Description { get; set; }
		public string Description_Long { get; set; }
		public string Material { get; set; }
		public string Size { get; set; }
		public double Width { get; set; }
		public double Length { get; set; }
		public int Storeys { get; set; }
		public double StoreyHeight { get; set; }
		public IList<BOMMaterialItem> Materials { get; set; }
	}

	public class BOMSpecifications
	{
		public BOMListOfMaterials materials { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Project Title")]
		public string Title { get; set; }

		[StringLength(100, MinimumLength = 0)]
		[Required]
		[Display(Name = "Project Reference")]
		public string Reference { get; set; }

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
		[Display(Name = "Number of Storeys")]
		[Range(1, 1000000000)]
		public int NumberOfStoreys { get; set; }

		[Required]
		[Display(Name = "Height between floors")]
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

	public class BOM_Materials_Subitems
	{
		public int SubitemNumber { get; set; }
		public int MaterialID { get; set; }
		public string MaterialDesc { get; set; }
		public string MaterialUoM { get; set; }
		public int MaterialQuantity { get; set; }
		public double MaterialCost { get; set; }
		public double MaterialAmount { get; set; }
		public double LabourCost { get; set; } = 0;
		public double TotalUnitRate { get; set; } = 0;
	}

	public class BOM_Materials_Items
	{
		[Required]
		public IList<BOM_Materials_Subitems> Subitems { get; set; }
		public int ItemNumber { get; set; }
		public string Description { get; set; }
	}

	public class BOM_Materials_Lists
	{
		[Required]
		public IList<BOM_Materials_Items> Items { get; set; }
		public char ListLetter { get; set; }
		public int ListNumber { get; set; }
		public string Description { get; set; }
	}

	public class BOM_Information
	{
		public BOMSpecifications specs { get; set; }
		[Required]
		public List<BOM_Materials_Lists> lists { get; set; }
		public double totalCost { get; set; }
	}

	public class MCE_ItemEditModel
	{
		[Display(Name = "Material Name")]
		public string MaterialDesc { get; set; }
		[Display(Name = "Unit of Measurement")]
		public string MaterialUoM { get; set; }
		[Display(Name = "Material Quantity")]
		public int MaterialQuantity { get; set; }
		[Display(Name = "Material Cost")]
		public double MaterialCost { get; set; }
		[Display(Name = "Material Total Cost")]
		public double MaterialAmount { get; set; }
		[Display(Name = "Labour Cost")]
		public double LabourCost { get; set; }
		[Display(Name = "Total Material Unit Rate")]
		public double TotalUnitRate { get; set; }
	}


	public class MCE_Information
	{
		public BOM_Information info { get; set; }

		public double materialCost { get; set; } = 0;
		public double labourCost { get; set; } = 0;
		public double totalCost { get; set; } = 0;
		public double profit { get; set; } = 0;
		public double profitPercent { get; set; } = 0;
		public double VAT { get; set; } = 0;
		public double totalProjectCost { get; set; } = 0;
	}
}
