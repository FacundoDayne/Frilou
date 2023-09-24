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
	}

	public class CategoryList
	{
		public string Id { get; set; }
		public string description { get; set; }
	}

	public class ManufacturerList
	{
		public string Id { get; set; }
		public string description { get; set; }
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
}
