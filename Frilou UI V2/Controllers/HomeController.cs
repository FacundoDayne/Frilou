using Frilou_UI_V2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.Metrics;

namespace Frilou_UI_V2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult HomePage()
        {
            return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> HomePage(LoginViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			Debug.WriteLine("Haruhi");
			using (MySqlConnection conn = new MySqlConnection("Data Source=localhost;port=3306;Initial Catalog=frilou_db;User Id=root;password=password123;"))
			{
				Debug.WriteLine("Konata");
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM frilou_users WHERE username = '" + model.Username + "' AND password = '" + model.Password + "';"))
				{
					command.Connection = conn;
					conn.Open();
					Debug.WriteLine("Dejiko");
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						Debug.WriteLine("dom");
						if (!sdr.Read())
						{
							ModelState.AddModelError("ValidationSummary", "Username or password is invalid");
							return View(model);
						}
						else
						{
							int role = Convert.ToInt32(sdr["user_role"]);
							if (role == 1)
							{
								return RedirectToAction("Account");
							}
							else if (role == 0)
							{
								return RedirectToAction("Account");
							}
						}
					}
					conn.Close();
				}
			}
			return View(model);
		}

		public IActionResult Account()
        {
            return View();
        }

        public IActionResult BillOfMaterials()
        {
            return View();  
        }

        public IActionResult GenerateBOM()
        {
            return View();  
        }

        public IActionResult MaterialCostEstimate()
        {
            return View();
        }

        public IActionResult AddProduct()
		{
			MaterialsAddModel xmodel = new MaterialsAddModel();

			xmodel.measurements = new List<MeasurementList>();
			xmodel.categories = new List<CategoryList>();
			xmodel.manufacturers = new List<ManufacturerList>();

			using (MySqlConnection conn = new MySqlConnection("Data Source=localhost;port=3306;Initial Catalog=frilou_db;User Id=root;password=password123;"))
			{
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM measurement_units;"))
				{
					command.Connection = conn;
					conn.Open();
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						Debug.WriteLine("dom");
						while (sdr.Read())
						{
							Debug.WriteLine("som");
							//xmodel.measurements.Add(new SelectListItem
							xmodel.measurements.Add(new MeasurementList
							{
								Id = sdr["measurment_unit_id"].ToString(),
								description = sdr["unit_desc"].ToString()
							}
							);
						}
					}
					conn.Close();
				}
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM material_categories;"))
				{
					command.Connection = conn;
					conn.Open();
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						Debug.WriteLine("dom");
						while (sdr.Read())
						{
							Debug.WriteLine("som");
							//xmodel.categories.Add(new SelectListItem
							xmodel.categories.Add(new CategoryList
							{
								Id = sdr["category_id"].ToString(),
								description = sdr["category_desc"].ToString()
							}
							);
						}
					}
					conn.Close();
				}
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM material_manufacturers;"))
				{
					command.Connection = conn;
					conn.Open();
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						Debug.WriteLine("dom");
						while (sdr.Read())
						{
							Debug.WriteLine("som");
							//xmodel.manufacturers.Add(new SelectListItem
							xmodel.manufacturers.Add(new ManufacturerList
							{
								Id = sdr["manufacturer_id"].ToString(),
								description = sdr["manufacturer_desc"].ToString()
							}
							);
						}
					}
					conn.Close();
				}
			}
			Debug.WriteLine($"{xmodel.measurements.Count} : {xmodel.categories.Count} : {xmodel.manufacturers.Count}");
			return View(xmodel);
		}
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult AddProduct(MaterialsAddModel model)
		{
			Debug.WriteLine("a");

			if (!ModelState.IsValid)
			{
				Debug.WriteLine("invalid");
				Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss"));

				foreach (var key in ModelState.Keys)
				{
					var errors = ModelState[key].Errors;
					foreach (var error in errors)
					{
						// Log or display the error message
						var errorMessage = error.ErrorMessage;
						Debug.WriteLine(": " + errorMessage);
						// You can also access error.Exception for exceptions if applicable
					}
				}

				model.measurements = new List<MeasurementList>();
				model.categories = new List<CategoryList>();
				model.manufacturers = new List<ManufacturerList>();

				using (MySqlConnection conn = new MySqlConnection("Data Source=localhost;port=3306;Initial Catalog=frilou_db;User Id=root;password=password123;"))
				{
					using (MySqlCommand command = new MySqlCommand("SELECT * FROM measurement_units;"))
					{
						command.Connection = conn;
						conn.Open();
						using (MySqlDataReader sdr = command.ExecuteReader())
						{
							while (sdr.Read())
							{
								//xmodel.measurements.Add(new SelectListItem
								model.measurements.Add(new MeasurementList
								{
									Id = sdr["measurment_unit_id"].ToString(),
									description = sdr["unit_desc"].ToString()
								}
								);
							}
						}
						conn.Close();
					}
					using (MySqlCommand command = new MySqlCommand("SELECT * FROM material_categories;"))
					{
						command.Connection = conn;
						conn.Open();
						using (MySqlDataReader sdr = command.ExecuteReader())
						{
							while (sdr.Read())
							{
								//xmodel.categories.Add(new SelectListItem
								model.categories.Add(new CategoryList
								{
									Id = sdr["category_id"].ToString(),
									description = sdr["category_desc"].ToString()
								}
								);
							}
						}
						conn.Close();
					}
					using (MySqlCommand command = new MySqlCommand("SELECT * FROM material_manufacturers;"))
					{
						command.Connection = conn;
						conn.Open();
						using (MySqlDataReader sdr = command.ExecuteReader())
						{
							while (sdr.Read())
							{
								//xmodel.manufacturers.Add(new SelectListItem
								model.manufacturers.Add(new ManufacturerList
								{
									Id = sdr["manufacturer_id"].ToString(),
									description = sdr["manufacturer_desc"].ToString()
								}
								);
							}
						}
						conn.Close();
					}
				}
				String cawk = $"Desc: {model.Description}\nLongDesc: {model.LongDescription}\nMUnit: {model.MeasurementUnit}"
					+ $"\nCategory: {model.Category}\nManufacturer: {model.Manufacturer}\nPrice: {model.Price}";
				Debug.WriteLine(cawk);
				return View(model);
			}
			String test = $"Desc: {model.Description}\nLongDesc: {model.LongDescription}\nMUnit: {model.MeasurementUnit}"
				+ $"\nCategory: {model.Category}\nManufacturer: {model.Manufacturer}\nPrice: {model.Price}";
			Debug.WriteLine(test);
			Console.WriteLine(test);

			decimal length = 0, width = 0, height = 0, weight = 0, volume = 0;

			if (model.Length != null)
				length = model.Length.Value;
			if (model.Width != null)
				width = model.Width.Value;
			if (model.Height != null)
				height = model.Height.Value;
			if (model.Weight != null)
				weight = model.Weight.Value;
			if (model.Volume != null)
				volume = model.Volume.Value;

			using (MySqlConnection conn = new MySqlConnection("Data Source=localhost;port=3306;Initial Catalog=frilou_db;User Id=root;password=password123;"))
			{
				using (MySqlCommand command = new MySqlCommand("INSERT INTO `frilou_db`.`materials` (`material_desc`,`material_desc_long`,`unit_id`,`category_id`,`manufacturer_id`,`price`,`length`,`width`,`height`,`weight`,`volume`) " +
					"VALUES (@material_desc,@material_desc_long, @unit_id, @category_id, @manufacturer_id, @price, " +
					"@length, @width, @height, @weight, @volume);"))
				{
					command.Connection = conn;

					command.Parameters.AddWithValue("@material_desc", model.Description);
					command.Parameters.AddWithValue("@material_desc_long", model.LongDescription);
					command.Parameters.AddWithValue("@unit_id", Convert.ToUInt32(model.MeasurementUnit));
					command.Parameters.AddWithValue("@category_id", Convert.ToUInt32(model.Category));
					command.Parameters.AddWithValue("@manufacturer_id", Convert.ToUInt32(model.Manufacturer));
					command.Parameters.AddWithValue("@price", model.Price);
					if (length != 0)
						command.Parameters.AddWithValue("@length", length);
					else
						command.Parameters.AddWithValue("@length", DBNull.Value);

					if (width != 0)
						command.Parameters.AddWithValue("@width", width);
					else
						command.Parameters.AddWithValue("@width", DBNull.Value);

					if (height != 0)
						command.Parameters.AddWithValue("@height", height);
					else
						command.Parameters.AddWithValue("@height", DBNull.Value);

					if (weight != 0)
						command.Parameters.AddWithValue("@weight", weight);
					else
						command.Parameters.AddWithValue("@weight", DBNull.Value);

					if (volume != 0)
						command.Parameters.AddWithValue("@volume", volume);
					else
						command.Parameters.AddWithValue("@volume", DBNull.Value);



					conn.Open();
					command.ExecuteNonQuery();
					conn.Close();
				}
			}

			return RedirectToAction("Account");
		}

		public IActionResult EditProduct()
        {
            return View();
        }

        public IActionResult AddEmployee()
        {
			AddEmployeeModel xmodel = new AddEmployeeModel();

			xmodel.roles = new List<RoleList>();

			using (MySqlConnection conn = new MySqlConnection("Data Source=localhost;port=3306;Initial Catalog=frilou_db;User Id=root;password=password123;"))
			{
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM frilou_users_roles;"))
				{
					command.Connection = conn;
					conn.Open();
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						Debug.WriteLine("dom");
						while (sdr.Read())
						{
							Debug.WriteLine("som");
							//xmodel.measurements.Add(new SelectListItem
							xmodel.roles.Add(new RoleList
							{
								id = sdr["role_id"].ToString(),
								name = sdr["role_name"].ToString()
							}
							);
						}
					}
					conn.Close();
				}
			}
			return View(xmodel);
        }

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult AddEmployee(AddEmployeeModel model)
		{
			if (!ModelState.IsValid)
			{

				model.roles = new List<RoleList>();

				using (MySqlConnection conn = new MySqlConnection("Data Source=localhost;port=3306;Initial Catalog=frilou_db;User Id=root;password=password123;"))
				{
					using (MySqlCommand command = new MySqlCommand("SELECT * FROM frilou_users_roles;"))
					{
						command.Connection = conn;
						conn.Open();
						using (MySqlDataReader sdr = command.ExecuteReader())
						{
							Debug.WriteLine("dom");
							while (sdr.Read())
							{
								Debug.WriteLine("som");
								//xmodel.measurements.Add(new SelectListItem
								model.roles.Add(new RoleList
								{
									id = sdr["role_id"].ToString(),
									name = sdr["role_name"].ToString()
								}
								);
							}
						}
						conn.Close();
					}
				}
				return View(model);
			}
			bool username_exists = false;

			using (MySqlConnection conn = new MySqlConnection("Data Source=localhost;port=3306;Initial Catalog=frilou_db;User Id=root;password=password123;"))
			{
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM `frilou_users` WHERE `username` = @username"))
				{
					conn.Open();
					command.Connection = conn;
					command.Parameters.AddWithValue("@username", model.Username);
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						if (sdr.Read())
						{
							username_exists = true;
						}
					}
					conn.Close();
				}
			}

			if (username_exists)
			{
				model.roles = new List<RoleList>();
				ModelState.AddModelError("Username", "Username already exists");
				using (MySqlConnection conn = new MySqlConnection("Data Source=localhost;port=3306;Initial Catalog=frilou_db;User Id=root;password=password123;"))
				{
					using (MySqlCommand command = new MySqlCommand("SELECT * FROM frilou_users_roles;"))
					{
						command.Connection = conn;
						conn.Open();
						using (MySqlDataReader sdr = command.ExecuteReader())
						{
							Debug.WriteLine("dom");
							while (sdr.Read())
							{
								Debug.WriteLine("som");
								//xmodel.measurements.Add(new SelectListItem
								model.roles.Add(new RoleList
								{
									id = sdr["role_id"].ToString(),
									name = sdr["role_name"].ToString()
								}
								);
							}
						}
						conn.Close();
					}
				}
				return View(model);
			}

			bool error = false;
			using (MySqlConnection conn = new MySqlConnection("Data Source=localhost;port=3306;Initial Catalog=frilou_db;User Id=root;password=password123;"))
			{
				conn.Open();
				using (MySqlTransaction transaction = conn.BeginTransaction())
				{
					try
					{
						using (MySqlCommand command = new MySqlCommand("INSERT INTO `frilou_db`.`user_info` (`user_info_firstname`,`user_info_middlename`,`user_info_lastname`,`user_info_contactnum`,`user_info_email`,`user_info_address`,`user_info_city`,`user_info_status`) " +
							"VALUES(@FirstName, @MiddleName, @LastName, @Contact, @Email, @Address, @City, @Status); "))
						{
							command.Connection = conn;
							command.Transaction = transaction;

							command.Parameters.AddWithValue("@FirstName", model.FirstName);
							command.Parameters.AddWithValue("@MiddleName", model.MiddleName);
							command.Parameters.AddWithValue("@LastName", model.LastName);
							command.Parameters.AddWithValue("@Contact", model.Contact);
							command.Parameters.AddWithValue("@Email", model.Email);
							command.Parameters.AddWithValue("@Address", model.Address);
							command.Parameters.AddWithValue("@City", model.City);
							command.Parameters.AddWithValue("@Status", 1);

							command.ExecuteNonQuery();
						}
						uint info_id = 0;

						using (MySqlCommand command = new MySqlCommand("SELECT * FROM `user_info` ORDER BY `user_info_id` DESC LIMIT 1;"))
						{
							command.Connection = conn;
							command.Transaction = transaction;

							using (MySqlDataReader sdr = command.ExecuteReader())
							{
								while (sdr.Read())
								{
									info_id = Convert.ToUInt32(sdr["user_info_id"]);
								}
							}


							command.ExecuteNonQuery();
						}

						using (MySqlCommand command = new MySqlCommand("INSERT INTO `frilou_db`.`frilou_users` (`username`,`password`,`user_role`,`user_status`,`user_info_id`) " +
							"VALUES(@username, @password, @user_role, @user_status, @user_info_id);"))
						{
							command.Connection = conn;
							command.Transaction = transaction;

							command.Parameters.AddWithValue("@username", model.Username);
							command.Parameters.AddWithValue("@password", model.Password);
							command.Parameters.AddWithValue("@user_role", Convert.ToUInt32(model.Role));
							command.Parameters.AddWithValue("@user_status", 1);
							command.Parameters.AddWithValue("@user_info_id", info_id);

							command.ExecuteNonQuery();
						}
						transaction.Commit();
					}
					catch (MySqlException e)
					{
						error = true;
						transaction.Rollback();
					}
				}
				conn.Close();
			}
			if (error)
			{
				model.roles = new List<RoleList>();
				using (MySqlConnection conn = new MySqlConnection("Data Source=localhost;port=3306;Initial Catalog=frilou_db;User Id=root;password=password123;"))
				{
					using (MySqlCommand command = new MySqlCommand("SELECT * FROM frilou_users_roles;"))
					{
						command.Connection = conn;
						conn.Open();
						using (MySqlDataReader sdr = command.ExecuteReader())
						{
							Debug.WriteLine("dom");
							while (sdr.Read())
							{
								Debug.WriteLine("som");
								//xmodel.measurements.Add(new SelectListItem
								model.roles.Add(new RoleList
								{
									id = sdr["role_id"].ToString(),
									name = sdr["role_name"].ToString()
								}
								);
							}
						}
						conn.Close();
					}
				}
				return View(model);
			}

			return RedirectToAction("Account");
		}


		public IActionResult EditEmployee()
        {
            return View();
        }

        public IActionResult AddPartner()
        {
            return View();
        }

        public IActionResult EditPartner()
        {
            return View();
        }

        public IActionResult LogIn()
        {
            return View();
        }

        public IActionResult AccountPartner()
        {
            return View();
        }

        public IActionResult AccountEmployee()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}