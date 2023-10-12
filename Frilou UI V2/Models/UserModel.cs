using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Frilou_UI_V2.Models
{
	public class AddSupplierModel
	{
		[Required]
		[StringLength(62, MinimumLength = 1)]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[Required]
		[StringLength(62, MinimumLength = 6)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 1)]
		[Display(Name = "Name")]
		public string Description { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 1)]
		[Display(Name = "Address")]
		public string Address { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 1)]
		[Display(Name = "City")]
		public string City { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 1)]
		[Display(Name = "Admin District")]
		public string AdminDistrict { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 1)]
		[Display(Name = "Country")]
		public string Country { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 1)]
		[Display(Name = "Longtitude")]
		public string Longtitude { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 1)]
		[Display(Name = "Latitude")]
		public string Latitude { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 1)]
		[Display(Name = "Contact Name")]
		public string ContactName { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 1)]
		[Display(Name = "Contact Number")]
		public string ContactNumber { get; set; }

	}
}
