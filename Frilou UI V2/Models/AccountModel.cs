using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Frilou_UI_V2.Models
{
	public class AccountModels
	{
		public int Id { get; set; }
		public string Username { get; set; }
	}

	public class LoginViewModel
	{
		[Required]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }
	}

	public class AdminModel
	{
		public IList<EmployeesList> employees { get; set; }
		public IList<MaterialsList> materials { get; set; }
	}

	public class EmployeesList
	{
		public string username { get; set; }
		public string role { get; set; }
	}

	public class MaterialsList
	{
		public string description { get; set; }
		public string description_long { get; set; }
		public string category { get; set; }
		public string measurement_unit { get; set; }
		public double price { get; set; }
	}

	public class MeasurementList
	{
		public string Id { get; set; }
		public string description { get; set; }

		public int category_id
		{
			get
			{
				return Convert.ToInt32(Id);
			}
		}
	}

	public class CategoryList
	{
		public string Id { get; set; }
		public string description { get; set; }

		public int category_id
		{
			get
			{
				return Convert.ToInt32(Id);
			}
		}
	}

	public class ManufacturerList
	{
		public string Id { get; set; }
		public string description { get; set; }

		public int category_id
		{
			get
			{
				return Convert.ToInt32(Id);
			}
		}
	}

	public class AddMaterialModel
	{
		public IList<MeasurementList> measurements { get; set; }
		public IList<CategoryList> categories { get; set; }
		public IList<ManufacturerList> manufacturers { get; set; }
	}

	public class MaterialsAddModel
	{
		public IList<MeasurementList>? measurements { get; set; }
		public IList<CategoryList>? categories { get; set; }
		public IList<ManufacturerList>? manufacturers { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Description")]
		public string Description { get; set; } = string.Empty;

		[StringLength(255, MinimumLength = 0)]
		[Display(Name = "Long Description")]
		public string LongDescription { get; set; } = string.Empty;

		[StringLength(10, MinimumLength = 1)]
		[Required]
		[Display(Name = "Measurement Unit")]
		public string MeasurementUnit { get; set; } = string.Empty;

		[StringLength(10, MinimumLength = 1)]
		[Required]
		[Display(Name = "Category")]
		public string Category { get; set; } = string.Empty;

		[StringLength(10, MinimumLength = 1)]
		[Required]
		[Display(Name = "Manufacturer")]
		public string Manufacturer { get; set; } = string.Empty;

		[Range(0, 1000000000)]
		[DataType(DataType.Currency)]
		[Display(Name = "Price")]
		[Required]
		public decimal Price { get; set; }

		[Range(0, 1000000000)]
		[Display(Name = "Length")]
		public decimal? Length { get; set; }

		[Range(0, 1000000000)]
		[Display(Name = "Width")]
		public decimal? Width { get; set; }

		[Range(0, 1000000000)]
		[Display(Name = "Height")]
		public decimal? Height { get; set; }

		[Range(0, 1000000000)]
		[Display(Name = "Weight")]
		public decimal? Weight { get; set; }

		[Range(0, 1000000000)]
		[Display(Name = "Volume")]
		public decimal? Volume { get; set; }
	}

	public class RoleList
	{
		public string id { get; set; }
		public string name { get; set; }
	}

	public class AddEmployeeModel
	{
		public IList<RoleList>? roles { get; set; }

		[Required]
		[StringLength(62, MinimumLength = 1)]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[Required]
		[StringLength(62, MinimumLength = 6)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[StringLength(10, MinimumLength = 1)]
		[Required]
		[Display(Name = "Role")]
		public string Role { get; set; } = string.Empty;

		[Required]
		[StringLength(50, MinimumLength = 1)]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 1)]
		[Display(Name = "Middle Name")]
		public string MiddleName { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 1)]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required]
		[DataType(DataType.PhoneNumber)]
		[Display(Name = "Contact Number")]
		public string Contact { get; set; }

		[Required]
		[StringLength(62, MinimumLength = 1)]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[StringLength(95, MinimumLength = 1)]
		[Display(Name = "Address")]
		public string Address { get; set; }

		[Required]
		[StringLength(35, MinimumLength = 1)]
		[Display(Name = "City")]
		public string City { get; set; }

	}

	public class MaterialsViewModel
	{
		public string ID { get; set; }
		public string Description { get; set; }
		public string Manufacturer { get; set; }
	}

	public class MaterialsListViewModel
	{
		public IList<MaterialsViewModel> Materials { get; set; }
	}



	public class MaterialsEditModel
	{
		public IList<MeasurementList>? measurements { get; set; }
		public IList<CategoryList>? categories { get; set; }
		public IList<ManufacturerList>? manufacturers { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "ID")]
		public string ID{ get; set; } = string.Empty;

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Description")]
		public string Description { get; set; } = string.Empty;

		[StringLength(255, MinimumLength = 0)]
		[Display(Name = "Long Description")]
		public string LongDescription { get; set; } = string.Empty;

		[StringLength(10, MinimumLength = 1)]
		[Required]
		[Display(Name = "Measurement Unit")]
		public string MeasurementUnit { get; set; } = string.Empty;

		[StringLength(10, MinimumLength = 1)]
		[Required]
		[Display(Name = "Category")]
		public string Category { get; set; } = string.Empty;

		[StringLength(10, MinimumLength = 1)]
		[Required]
		[Display(Name = "Manufacturer")]
		public string Manufacturer { get; set; } = string.Empty;

		[Range(0, 1000000000)]
		[DataType(DataType.Currency)]
		[Display(Name = "Price")]
		[Required]
		public decimal Price { get; set; }

		[Range(0, 1000000000)]
		[DataType(DataType.Currency)]
		[Display(Name = "Length")]
		public decimal? Length { get; set; }

		[Range(0, 1000000000)]
		[Display(Name = "Width")]
		public decimal? Width { get; set; }

		[Range(0, 1000000000)]
		[Display(Name = "Height")]
		public decimal? Height { get; set; }

		[Range(0, 1000000000)]
		[Display(Name = "Weight")]
		public decimal? Weight { get; set; }

		[Range(0, 1000000000)]
		[Display(Name = "Volume")]
		public decimal? Volume { get; set; }
	}

	public class MCESubitems
	{
		public string Desc { get; set; }
		public string UoM { get; set; }
		public string Quantity { get; set; }
		public string UnitRate { get; set; }
		public string MarkedUp { get; set; }
		public string Amount { get; set; }
	}

	public class MCEItems
	{
		public string Desc { get; set; }
		public IList<MCESubitems> subitems { get; set; }
	}

	public class MCEList
	{
		public string Desc { get; set; }
		public IList<MCEItems> items { get; set; }
	}

	public class MaterialCostEstimateModel
	{
		public IList<MCEList> lists { get; set; }
	}

	public class MCEAddListModel
	{
		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Description")]
		public string Description { get; set; } = string.Empty;

	}

	public class MCEItemTypes
	{
		public string Name { get; set; }
	}

	public class MCEAddItemModel
	{
		public int? listId { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Description")]
		public string Description { get; set; } = string.Empty;

	}

	public class MCEAddSubitemModel
	{
		public int? listId { get; set; }
		public int? itemId { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Description")]
		public string Description { get; set; } = string.Empty;

		[StringLength(25, MinimumLength = 1)]
		[Required]
		[Display(Name = "UoM")]
		public string UoM { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Quantity")]
		[Range(0, 1000000000)]
		public decimal Quantity { get; set; }

		[Required]
		[Display(Name = "Unit Rate")]
		[Range(0, 1000000000)]
		[DataType(DataType.Currency)]
		public decimal UnitRate { get; set; }

		[Required]
		[Display(Name = "Marked Up Rate")]
		[Range(0, 1000000000)]
		[DataType(DataType.Currency)]
		public decimal MarkUp { get; set; }

		[Required]
		[Display(Name = "Amount")]
		[Range(0, 1000000000)]
		[DataType(DataType.Currency)]
		public decimal Amount { get; set; }

	}

	public class BuidlingMaterialItem
	{
		public string Id { get; set; }
		public string description { get; set; }
	}

	public class GenerateBOMModel
	{
		public IList<BuidlingMaterialItem>? MaterialsList { get; set; }

		[Required]
		[Display(Name = "Building Material")]
		[Range(1, 1000000000)]
		public int BuildingMaterial { get; set; }

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

	public class BOMSubitems
	{
		public int? id { get; set; }
		public uint item_id { get; set; }
		public string Quantity { get; set; }
	}

	public class BOMItems
	{
		public int? id { get; set; }
		public uint item_id { get; set; }
		public IList<BOMSubitems> subitems { get; set; }
	}

	public class BOMList
	{
		public int? id { get; set; }
		public string Desc { get; set; }
		public IList<BOMItems> items { get; set; }
	}

	public class BillOfMaterialsModel
	{
		public IList<MaterialItem>? materials { get; set; }
		public IList<CategoryList>? categories { get; set; }
		public IList<MeasurementList>? measurements { get; set; }

		public IList<BOMList>? lists { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Project Title")]
		public string Title { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Project Address")]
		public string Address { get; set; }

		[Required]
		[Display(Name = "Project Date")]
		[DataType(DataType.Date)]
		public DateTime ProjectDate { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Project Ref")]
		public string ProjectRef { get; set; }

		public uint? Engineer_ID { get; set; }

		[Display(Name = "Project ID")]
		public uint? ID { get; set; }

		public string? buildingMaterial { get; set; }

		[Display(Name = "Project Building Material")]
		public string? buildingMaterialDesc { get; set; }

		[Display(Name = "Nomber of Storeys")]
		public int? storeys { get; set; }

		[Display(Name = "Height between floors")]
		public double? floorHeight { get; set; }

		[Display(Name = "Building Width")]
		public double? width { get; set; }

		[Display(Name = "Building Length")]
		public double? length { get; set; }

	}

	public class BOMAddListModel
	{
		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Description")]
		public string Description { get; set; } = string.Empty;

	}

	public class BOMItemTypes
	{
		public string Name { get; set; }
	}

	public class BOMAddItemModel
	{
		public int? listId { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Description")]
		public string Description { get; set; } = string.Empty;

	}

	public class BOMAddSubitemModel
	{
		public string? id { get; set; }

		[StringLength(100, MinimumLength = 1)]
		[Required]
		[Display(Name = "Description")]
		public string Description { get; set; } = string.Empty;

		[StringLength(25, MinimumLength = 1)]
		[Required]
		[Display(Name = "UoM")]
		public string UoM { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Quantity")]
		[Range(0, 1000000000)]
		public double Quantity { get; set; }
	}

	public class MaterialItem
	{
		public string material_id_string { get; set; }
		public string material_desc { get; set; }
		public string material_long_desc { get; set; }
		public uint unit_id { get; set; }
		public uint category_id { get; set; }
		public uint manufacturer_id { get; set; }
		public double price { get; set; }
		public double? length { get; set; }
		public double? width { get; set; }
		public double? height { get; set; }
		public double? weight { get; set; }
		public double? volume { get; set; }

		public int? material_id
		{
			get
			{
				return Convert.ToInt32(material_id_string);
			}
		}
	}

	public class MaterialCategory
	{
		public string category_id_string { get; set; }
		public string category_desc { get; set; }

		public int category_id { get
			{
				return Convert.ToInt32(category_id_string);
			} }
	}

	public class BOMProjectsListItem
	{
		public int id { get; set; }
		public string title { get; set; }
		public DateTime date { get; set; }
	}

	public class BOMProjectsList
	{
		public IList<BOMProjectsListItem> projects { get; set; }
	}

	public class MCECreateModel
	{
		public int? id { get; set; }
		public int bom_id { get; set; }
		public double markup { get; set; }
	}

	public class MCESubitem
	{
		public int? id { get; set; }
		public int mce_id { get; set; }
		public int subitem_id { get; set; }
		public double markup { get; set; }

		public uint item_id { get; set; }
		public string Quantity { get; set; }
	}


	public class TestModel
	{
		public string ass { get; set; }
	}
}
