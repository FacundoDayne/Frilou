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
using Newtonsoft.Json;
using Microsoft.Build.ObjectModelRemoting;
using MySqlX.XDevAPI;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;
using Google.Protobuf.Collections;

namespace Frilou_UI_V2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly string connectionstring = "Data Source=localhost;port=3306;Initial Catalog=frilou_db;User Id=root;password=password123;";

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
			using (MySqlConnection conn = new MySqlConnection(connectionstring))
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
							HttpContext.Session.SetInt32("UserID", Convert.ToInt32(sdr["user_id"])); //.Session["UserID"] = Convert.ToInt32(sdr["user_id"]);
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
			TempData["BOMModel"] = null;
			BOMProjectsList model = new BOMProjectsList();
			model.projects = new List<BOMProjectsListItem>();
			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM bom;"))
				{
					command.Connection = conn;
					conn.Open();
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.projects.Add(new BOMProjectsListItem()
							{
								id = Convert.ToInt32(sdr["bom_id"]),
								title = sdr["project_title"].ToString(),
								date = DateTime.Parse(sdr["project_date"].ToString())
							});
						}
					}
				}
			}
			return View(model);  
        }

        public IActionResult GenerateBOM()
        {
			GenerateBOMModel model = new GenerateBOMModel();
			model.MaterialsList = new List<BuidlingMaterialItem>();
			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM building_material;"))
				{
					command.Connection = conn;
					conn.Open();
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.MaterialsList.Add(new BuidlingMaterialItem()
							{
								Id = sdr["building_material_id"].ToString(),
								description = sdr["building_material_desc"].ToString()
							});
						}
					}
				}
			}

			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GenerateBOM(GenerateBOMModel model)
		{
			model.MaterialsList = new List<BuidlingMaterialItem>();
			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM building_material;"))
				{
					command.Connection = conn;
					conn.Open();
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.MaterialsList.Add(new BuidlingMaterialItem()
							{
								Id = sdr["building_material_id"].ToString(),
								description = sdr["building_material_desc"].ToString()
							});
						}
					}
				}
			}

			List<CategoryList> Categories = GetCategoriesFromDB();
			List<MaterialItem> Materials = GetMaterialsFromDB();
			List<MeasurementList> Measurements = GetMeasurementsFromDB();

			if (!ModelState.IsValid)
			{

				return View(model);
			}

			double BuildingArea = model.BuildingLength * model.BuildingWidth;
			double floorThickness = 0.127,
					wallThickness = 0.254,
					rebarConstant = 0.15,
					nailConstant = 20f,
					hollowBlockConstant = 12.5,
					supportBeamLength = 0.25,
					supportBeamWidth = 0.30,
					supportBeamArea = supportBeamLength * supportBeamWidth,
					supportBeamSpace = 2.92f,
					supportBeamVolume = supportBeamArea * model.FloorHeight,
					supportBeamsNeeded = BuildingArea / supportBeamSpace,
					concreteFormulaCement = 1,
					concreteFormulaSand = 2,
					concreteFormulaAggregate = 3,
					plywoodLength = 96,
					plywoodWidth = 48,
					plywoodArea = plywoodLength * plywoodWidth,
					plywoodSheetsPerSqm = (float)Math.Ceiling(10764 / plywoodArea),
					riserHeight = 0.178,
					threadDepth = 0.2794,
					numberOfSteps = (float)Math.Ceiling(model.FloorHeight / riserHeight);

			Debug.WriteLine(supportBeamArea);
			Debug.WriteLine(supportBeamVolume);
			Debug.WriteLine(supportBeamsNeeded);

			double foundationHeight, foundationVolume, foundationPerimeter, foundationWallArea, foundationNoOfHollowBlock, foundationRebar, foundationConcrete;
			double storeyHeight, storeyPerimeter, storeyWallVolume, storeyFloorVolume, storeyFloorPlywood=0, storeyFloorNails=0, storeyFloorConcrete = 0, storeyFloorRebar = 0;
			double storeySupportBeamsNeeded, storeySupportBeamsConcrete, storeySupportBeamsRebar;
			double storeyWallConcrete, storeyWallRebar, stairsVolume, stairsConcrete, stairsRebar;

			double totalConcrete, totalBlocks, totalRebar, totalPlywood, totalNails;

			foundationHeight = model.FloorHeight * model.NumberOfStoreys + (model.NumberOfStoreys * floorThickness);
			foundationVolume = foundationHeight * BuildingArea;
			foundationPerimeter = 2 * (model.BuildingWidth + model.BuildingLength);
			foundationWallArea = 4 * foundationPerimeter * foundationHeight;
			foundationNoOfHollowBlock = foundationWallArea * hollowBlockConstant;
			foundationRebar = rebarConstant * foundationVolume;
			foundationConcrete = foundationVolume;

			storeyHeight = (model.FloorHeight + floorThickness) * model.NumberOfStoreys;
			storeyPerimeter = (2 * (model.BuildingWidth + model.BuildingLength)) * model.NumberOfStoreys;
			storeyWallVolume = (storeyPerimeter * storeyHeight) * model.NumberOfStoreys;
			storeyFloorVolume = (BuildingArea * floorThickness) * model.NumberOfStoreys;

			if (Convert.ToInt32(model.BuildingMaterial) == 1)
			{
				storeyFloorConcrete = storeyFloorVolume;
                storeyFloorRebar = rebarConstant * storeyFloorVolume;
			}
			else if (Convert.ToInt32(model.BuildingMaterial) == 2)
			{
				storeyFloorPlywood = plywoodSheetsPerSqm * BuildingArea;
                storeyFloorNails = storeyFloorPlywood * nailConstant;
			}

			storeySupportBeamsNeeded = supportBeamsNeeded;
            storeySupportBeamsConcrete = supportBeamVolume * supportBeamsNeeded;
            storeySupportBeamsRebar = rebarConstant * supportBeamVolume * supportBeamsNeeded;


			storeyWallConcrete = storeyWallVolume;
			storeyWallRebar = rebarConstant * storeyWallVolume;

			stairsVolume = numberOfSteps * riserHeight * threadDepth * 1.25;
			stairsConcrete = stairsVolume;
			stairsRebar = rebarConstant * stairsVolume;

			totalConcrete = stairsConcrete + storeyWallConcrete + storeySupportBeamsConcrete + storeyFloorConcrete + foundationConcrete;
			totalBlocks = foundationNoOfHollowBlock;
			totalRebar = stairsRebar + storeyWallRebar + storeySupportBeamsRebar + storeyFloorRebar + foundationRebar;
			totalPlywood = storeyFloorPlywood;
			totalNails = storeyFloorNails;

			double costConcrete = totalConcrete * ((230 * 0.16667) + (800 * 0.33333) + (950 * 0.5));
			double costBlocks = totalBlocks * 13;
			double costRebar = totalRebar * 13;
			double costPlywood = totalPlywood * 490;

			string debug = $"Total Concrete: {totalConcrete} | {costConcrete}\n" +
							$"Total Rebar: {totalRebar} | {costRebar}\n" +
							$"Total Blocks: {totalBlocks} | {costBlocks}\n" +
							$"Total Plywood: {totalPlywood} | {costPlywood}\n" +
							$"Total Nails: {totalNails}\n";

			Debug.WriteLine(debug);

			BillOfMaterialsModel bommodel = new BillOfMaterialsModel();

			bommodel.Title = "";
			bommodel.Address = "";
			bommodel.ProjectDate = DateTime.Now;
			bommodel.ProjectRef = "";
			bommodel.buildingMaterialDesc = model.MaterialsList[Convert.ToInt32(model.BuildingMaterial)].description;
			bommodel.Engineer_ID = Convert.ToUInt32(HttpContext.Session.GetInt32("UserID"));
			bommodel.ID = 0;
			bommodel.storeys = model.NumberOfStoreys;
			bommodel.floorHeight = model.FloorHeight;
			bommodel.length = model.BuildingLength;
			bommodel.width = model.BuildingWidth;
			bommodel.buildingMaterial = model.BuildingMaterial.ToString();

			bommodel.materials = Materials;
			bommodel.categories = Categories;
			bommodel.measurements = Measurements;

			bommodel.lists = new List<BOMList>();
			bommodel.lists.Add(new BOMList()
			{
				Desc = "Foundation",
				items = new List<BOMItems>()
			});
			bommodel.lists[0].items.Add(new BOMItems()
			{
				item_id = 2, //Concrete,
				subitems = new List<BOMSubitems>()
			}); ;
			bommodel.lists[0].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 2,//cement
				Quantity = (Math.Round(foundationConcrete * 0.16667,2)).ToString()
			});
			bommodel.lists[0].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 3,//sand
				Quantity = (Math.Round(foundationConcrete * 0.33333,2)).ToString()
			});
			bommodel.lists[0].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 4,//aggregate
				Quantity = (Math.Round(foundationConcrete * 0.5, 2)).ToString()
			});
			bommodel.lists[0].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 5,//rebar
				Quantity = (foundationRebar).ToString()
			});
			bommodel.lists[0].items.Add(new BOMItems()
			{
				item_id = 3,//bricks
				subitems = new List<BOMSubitems>()
			});
			bommodel.lists[0].items[1].subitems.Add(new BOMSubitems()
			{
				item_id = 6,//blocks
				Quantity = (foundationNoOfHollowBlock).ToString()
			});
			////////
			bommodel.lists.Add(new BOMList()
			{
				Desc = "Storeys",
				items = new List<BOMItems>()
			});
			bommodel.lists[1].items.Add(new BOMItems()
			{
				item_id = 2,
				subitems = new List<BOMSubitems>()
			});
			bommodel.lists[1].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 2,
				Quantity = (Math.Round(storeyFloorConcrete * 0.16667, 2)).ToString()
			});
			bommodel.lists[1].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 3,
				Quantity = (Math.Round(storeyFloorConcrete * 0.33333, 2)).ToString()
			});
			bommodel.lists[1].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 4,
				Quantity = (Math.Round(storeyFloorConcrete * 0.5, 2)).ToString()
			});
			bommodel.lists[1].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 5,
				Quantity = (storeyFloorRebar).ToString()
			});
			///////
			bommodel.lists.Add(new BOMList()
			{
				Desc = "Support Beams",
				items = new List<BOMItems>()
			});
			bommodel.lists[2].items.Add(new BOMItems()
			{
				item_id = 2,
				subitems = new List<BOMSubitems>()
			});
			bommodel.lists[2].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 2,
				Quantity = (Math.Round(storeySupportBeamsConcrete * 0.16667, 2)).ToString()
			});
			bommodel.lists[2].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 3,
				Quantity = (Math.Round(storeySupportBeamsConcrete * 0.33333, 2)).ToString()
			});
			bommodel.lists[2].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 4,
				Quantity = (Math.Round(storeySupportBeamsConcrete * 0.5, 2)).ToString()
			});
			bommodel.lists[2].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 5,
				Quantity = (storeySupportBeamsRebar).ToString()
			});
			//////
			bommodel.lists.Add(new BOMList()
			{
				Desc = "Storey Walls",
				items = new List<BOMItems>()
			});
			bommodel.lists[3].items.Add(new BOMItems()
			{
				item_id = 2,
				subitems = new List<BOMSubitems>()
			});
			bommodel.lists[3].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 2,
				Quantity = (Math.Round(storeyWallConcrete * 0.16667, 2)).ToString()
			});
			bommodel.lists[3].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 3,
				Quantity = (Math.Round(storeyWallConcrete * 0.33333, 2)).ToString()
			});
			bommodel.lists[3].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 4,
				Quantity = (Math.Round(storeyWallConcrete * 0.5, 2)).ToString()
			});
			bommodel.lists[3].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 5,
				Quantity = (storeyWallRebar).ToString()
			});
			//////
			bommodel.lists.Add(new BOMList()
			{
				Desc = "Stairs",
				items = new List<BOMItems>()
			});
			bommodel.lists[4].items.Add(new BOMItems()
			{
				item_id = 2,
				subitems = new List<BOMSubitems>()
			});
			bommodel.lists[4].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 2,
				Quantity = (Math.Round(stairsConcrete * 0.16667, 2)).ToString()
			});
			bommodel.lists[4].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 3,
				Quantity = (Math.Round(stairsConcrete * 0.33333, 2)).ToString()
			});
			bommodel.lists[4].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 4,
				Quantity = (Math.Round(stairsConcrete * 0.5, 2)).ToString()
			});
			bommodel.lists[4].items[0].subitems.Add(new BOMSubitems()
			{
				item_id = 5,
				Quantity = (stairsRebar).ToString()
			});

			TempData["BOMModel"] = JsonConvert.SerializeObject(bommodel);

			Debug.WriteLine(bommodel.lists.Count);
			Debug.WriteLine(bommodel.lists[0].items.Count);
			Debug.WriteLine(bommodel.lists[0].items[0].subitems.Count);

			return RedirectToAction("BOMView");
		}

		public IActionResult MaterialCostEstimate(MaterialCostEstimateModel? bommodel)
        {
			MaterialCostEstimateModel model = null;
			if (TempData["MVCModel"] == null)
			{
				Debug.WriteLine("nuts");
				model = new MaterialCostEstimateModel();
				model.lists = new List<MCEList>();
			}
			else
			{
				model = JsonConvert.DeserializeObject<MaterialCostEstimateModel>(TempData["MVCModel"].ToString());
			}
			TempData["MVCModel"] = JsonConvert.SerializeObject(model);
			return View(model);
        }

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> MaterialCostEstimate(MaterialCostEstimateModel model, string submitButton) //
		{
			switch(submitButton)
			{
				case "a":
					return RedirectToAction("AccountPartner");
					break;
				case "b":
					return RedirectToAction("AccountEmployee");
					break;
			}
			return View();
		}

		public IActionResult MCEAddList()
		{
			return View();
		}
		
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> MCEAddList(MCEAddListModel listmodel)
		{
			MaterialCostEstimateModel model = JsonConvert.DeserializeObject<MaterialCostEstimateModel>(TempData["MVCModel"].ToString());
			model.lists.Add(new MCEList()
			{
				Desc = listmodel.Description,
				items = new List<MCEItems>()
			});
			TempData["MVCModel"] = JsonConvert.SerializeObject(model);
			return RedirectToAction("MaterialCostEstimate");
		}

		public IActionResult MCEAddItem(int? id)
		{
			MCEAddItemModel model = new MCEAddItemModel();
			model.listId = id;
			return View(model);
		}
		
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> MCEAddItem(MCEAddItemModel itemmodel, int? id)
		{
			MaterialCostEstimateModel model = JsonConvert.DeserializeObject<MaterialCostEstimateModel>(TempData["MVCModel"].ToString());
			model.lists[Convert.ToInt32(id)].items.Add(new MCEItems()
			{
				Desc = itemmodel.Description,
				subitems = new List<MCESubitems>()
			});
			TempData["MVCModel"] = JsonConvert.SerializeObject(model);
			return RedirectToAction("MaterialCostEstimate");
		}

		public IActionResult MCEAddSubitem(string? id)
		{
			string[] id_split = id.Split('s');
			return View();
		}
		/*
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public Task<ActionResult> MCEAddSubitem()
		{
			return View();
		}*/

		public IActionResult AddProduct()
		{
			MaterialsAddModel xmodel = new MaterialsAddModel();

			xmodel.measurements = new List<MeasurementList>();
			xmodel.categories = new List<CategoryList>();
			xmodel.manufacturers = new List<ManufacturerList>();

			using (MySqlConnection conn = new MySqlConnection(connectionstring))
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

				using (MySqlConnection conn = new MySqlConnection(connectionstring))
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
					command.Parameters.AddWithValue("@price", Convert.ToInt32(Math.Floor(model.Price * 100)));
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

		public IActionResult EditProduct(int? id)
        {
			MaterialsEditModel xmodel = new MaterialsEditModel();

			xmodel.measurements = new List<MeasurementList>();
			xmodel.categories = new List<CategoryList>();
			xmodel.manufacturers = new List<ManufacturerList>();

			using (MySqlConnection conn = new MySqlConnection(connectionstring))
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

			if (id != null)
			{
				using (MySqlConnection connection = new MySqlConnection(connectionstring))
				{
					using (MySqlCommand command = new MySqlCommand("SELECT * FROM `materials` WHERE `material_id` = @id;"))
					{
						connection.Open();
						command.Connection = connection;
						command.Parameters.AddWithValue("@id", Convert.ToUInt32(id));
						using (MySqlDataReader sdr = command.ExecuteReader())
						{
							while (sdr.Read())
							{
								xmodel.ID = id.ToString();
								xmodel.Description = sdr["material_desc"].ToString();
								xmodel.LongDescription = sdr["material_desc_long"].ToString();
								xmodel.MeasurementUnit = sdr["unit_id"].ToString();
								xmodel.Category = sdr["category_id"].ToString();
								xmodel.Manufacturer = sdr["manufacturer_id"].ToString();
								xmodel.Price = Convert.ToDecimal(sdr["price"]) / 100;
								if (sdr["length"] != DBNull.Value)
									xmodel.Length = Convert.ToDecimal(sdr["length"]);
								if (sdr["width"] != DBNull.Value)
									xmodel.Width = Convert.ToDecimal(sdr["width"]);
								if (sdr["height"] != DBNull.Value)
									xmodel.Height = Convert.ToDecimal(sdr["height"]);
								if (sdr["weight"] != DBNull.Value)
									xmodel.Weight = Convert.ToDecimal(sdr["weight"]);
								if (sdr["volume"] != DBNull.Value)
									xmodel.Volume = Convert.ToDecimal(sdr["volume"]);
							}
						}
					}
				}
			}


			return View(xmodel);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult EditProduct(MaterialsEditModel model)
		{
			if(!ModelState.IsValid)
			{
				model.measurements = new List<MeasurementList>();
				model.categories = new List<CategoryList>();
				model.manufacturers = new List<ManufacturerList>();
				using (MySqlConnection conn = new MySqlConnection(connectionstring))
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
							Debug.WriteLine("dom");
							while (sdr.Read())
							{
								Debug.WriteLine("som");
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
							Debug.WriteLine("dom");
							while (sdr.Read())
							{
								Debug.WriteLine("som");
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
				return View(model);
			}

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

			using (MySqlConnection connection = new MySqlConnection(connectionstring))
			{
				using (MySqlCommand command = new MySqlCommand("UPDATE materials SET " +
					"material_desc = @material_desc, " +
					"material_desc_long = @material_desc_long, " +
					"unit_id = @unit_id, " +
					"category_id = @category_id, " +
					"manufacturer_id = @manufacturer_id, " +
					"price = @price, " +
					"length = @length, " +
					"width = @width, " +
					"height = @height, " +
					"weight = @weight, " +
					"volume = @volume " +
					"WHERE `material_id` = @id;"))
				{
					connection.Open();
					command.Connection = connection;
					command.Parameters.AddWithValue("@id", Convert.ToUInt32(model.ID));
					command.Parameters.AddWithValue("@material_desc", model.Description);
					command.Parameters.AddWithValue("@material_desc_long", model.LongDescription);
					command.Parameters.AddWithValue("@unit_id", Convert.ToUInt32(model.MeasurementUnit));
					command.Parameters.AddWithValue("@category_id", Convert.ToUInt32(model.Category));
					command.Parameters.AddWithValue("@manufacturer_id", Convert.ToUInt32(model.Manufacturer));
					command.Parameters.AddWithValue("@price", Convert.ToInt32(Math.Floor(model.Price * 100)));

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

					command.ExecuteNonQuery();
				}
			}

			return RedirectToAction("Account");
		}

		/*
		public IActionResult EditProduct(int id)
		{
			MaterialsEditModel model = new MaterialsEditModel();
			using (MySqlConnection connection = new MySqlConnection(connectionstring))
			{
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM `materials` WHERE `material_id` = @id;"))
				{
					connection.Open();
					command.Connection = connection;
					command.Parameters.AddWithValue("@id", Convert.ToUInt32(id));
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.ID = id.ToString();
							model.Description = sdr["material_desc"].ToString();
							model.LongDescription = sdr["material_desc_long"].ToString();
							model.MeasurementUnit = sdr["unit_id"].ToString();
							model.Category = sdr["category_id"].ToString();
							model.Manufacturer = sdr["manufacturer_id"].ToString();
							model.Price = Convert.ToDecimal(sdr["price"]);
							model.Length = Convert.ToDecimal(sdr["length"]);
							model.Width = Convert.ToDecimal(sdr["width"]);
							model.Height = Convert.ToDecimal(sdr["height"]);
							model.Weight = Convert.ToDecimal(sdr["weight"]);
							model.Volume = Convert.ToDecimal(sdr["volume"]);
						}
					}
				}
			}
			return View(model);
		}
		*/
		public IActionResult MaterialsList()
		{
			MaterialsListViewModel model = new MaterialsListViewModel();
			model.Materials = new List<MaterialsViewModel>();
			using (MySqlConnection connection = new MySqlConnection(connectionstring))
			{
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM `materials` a INNER JOIN `material_manufacturers` b ON a.manufacturer_id = b.manufacturer_id;"))
				{
					connection.Open();
					command.Connection = connection;
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.Materials.Add(new MaterialsViewModel
							{
								ID = sdr["material_id"].ToString(),
								Description = sdr["material_desc"].ToString(),
								Manufacturer = sdr["manufacturer_desc"].ToString()
							});
						}
					}
				}
			}
			return View(model);
		}

		public IActionResult AddEmployee()
        {
			AddEmployeeModel xmodel = new AddEmployeeModel();

			xmodel.roles = new List<RoleList>();

			using (MySqlConnection conn = new MySqlConnection(connectionstring))
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

				using (MySqlConnection conn = new MySqlConnection(connectionstring))
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

			using (MySqlConnection conn = new MySqlConnection(connectionstring))
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
				using (MySqlConnection conn = new MySqlConnection(connectionstring))
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
			using (MySqlConnection conn = new MySqlConnection(connectionstring))
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
				using (MySqlConnection conn = new MySqlConnection(connectionstring))
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

        public IActionResult AccountPartner(MaterialCostEstimateModel send)
		{
			MaterialCostEstimateModel model = JsonConvert.DeserializeObject<MaterialCostEstimateModel>(TempData["MVCModel"].ToString());
			Debug.WriteLine("wead");
			Debug.WriteLine(model.lists[0].Desc);
			return View(model);
        }

        public IActionResult AccountEmployee()
        {
            return View();
        }

		public IActionResult BOMView(int? id)
		{
			BillOfMaterialsModel model = null;

			if (TempData["BOMModel"] == null && id == null)
			{
				Debug.WriteLine("nuts");
				model = new BillOfMaterialsModel();
				model.lists = new List<BOMList>();
				model.materials = GetMaterialsFromDB();
				model.categories = GetCategoriesFromDB();
				model.measurements = GetMeasurementsFromDB();
			}
			else if (TempData["BOMModel"] != null && id == null)
			{
				Debug.WriteLine("bunger");
				model = JsonConvert.DeserializeObject<BillOfMaterialsModel>(TempData["BOMModel"].ToString());
				model.materials = GetMaterialsFromDB();
				model.categories = GetCategoriesFromDB();
				model.measurements = GetMeasurementsFromDB();
			}
			
			else if (TempData["BOMModel"] == null && id != null)
			{
				model = new BillOfMaterialsModel();
				model.lists = new List<BOMList>();
				model.materials = GetMaterialsFromDB();
				model.categories = GetCategoriesFromDB();
				model.measurements = GetMeasurementsFromDB();

				int buildingmaterialid = 0;

				using (MySqlConnection conn = new MySqlConnection(connectionstring))
				{
					conn.Open();

					MySqlCommand command1 = new MySqlCommand("SELECT * FROM bom WHERE bom_id = @bom_id");
					command1.Parameters.AddWithValue("@bom_id", id);
					command1.Connection = conn;
					using (MySqlDataReader sdr = command1.ExecuteReader())
					{
						while(sdr.Read())
						{
							model.Title = sdr["project_title"].ToString();
							model.Address = sdr["project_location"].ToString();
							model.ProjectRef = sdr["project_ref"].ToString();
							model.ProjectDate = DateTime.Parse(sdr["project_date"].ToString());
							model.Engineer_ID = Convert.ToUInt32(sdr["project_engineer_id"]);
							buildingmaterialid = Convert.ToInt32(sdr["building_material_id"]);
							model.storeys = Convert.ToInt32(sdr["building_storeys"]);
							model.floorHeight = Convert.ToDouble(sdr["building_floorheight"]);
							model.length = Convert.ToDouble(sdr["building_length"]);
							model.width = Convert.ToDouble(sdr["building_width"]);
						}
					}
					command1.Dispose();

					MySqlCommand buildingmaterialcommand = new MySqlCommand("SELECT * FROM building_material WHERE building_material_id = @building_material_id");
					buildingmaterialcommand.Parameters.AddWithValue("@building_material_id", buildingmaterialid);
					buildingmaterialcommand.Connection = conn;
					using (MySqlDataReader sdr = buildingmaterialcommand.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.buildingMaterial = buildingmaterialid.ToString();
							model.buildingMaterialDesc = sdr["building_material_desc"].ToString();
						}
					}
					buildingmaterialcommand.Dispose();

					MySqlCommand command2 = new MySqlCommand("SELECT * FROM bom_lists WHERE bom_id = @bom_id;");
					command2.Parameters.AddWithValue("@bom_id", id);
					command2.Connection = conn;
					using (MySqlDataReader sdr = command2.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.lists.Add(new BOMList()
							{
								id = Convert.ToInt32(sdr["bom_list_id"]),
								Desc = sdr["list_desc"].ToString(),
								items = new List<BOMItems>()
							});
						}
					}
					command2.Dispose();

					int listindex = 0;
					foreach(BOMList list in model.lists)
					{
						MySqlCommand command3 = new MySqlCommand("SELECT * FROM bom_items WHERE bom_list_id = @bom_list_id");

						command3.Parameters.AddWithValue("@bom_list_id", list.id);
						command3.Connection = conn;
						using (MySqlDataReader sdr = command3.ExecuteReader())
						{
							while (sdr.Read())
							{
								model.lists[listindex].items.Add(new BOMItems()
								{
									id = Convert.ToInt32(sdr["bom_item_id"]),
									item_id = Convert.ToUInt32(sdr["item_id"]),
									subitems = new List<BOMSubitems>()
								});
							}
						}
						command3.Dispose();


						int itemindex = 0;
						foreach (BOMItems item in list.items)
						{
							MySqlCommand command4 = new MySqlCommand("SELECT * FROM bom_subitems WHERE bom_item_id = @bom_item_id");
							command4.Parameters.AddWithValue("@bom_item_id", item.id);
							command4.Connection = conn;
							using (MySqlDataReader sdr = command4.ExecuteReader())
							{
								while (sdr.Read())
								{
									model.lists[listindex].items[itemindex].subitems.Add(new BOMSubitems()
									{
										id = Convert.ToInt32(sdr["bom_item_id"]),
										item_id = Convert.ToUInt32(sdr["item_id"]),
										Quantity = sdr["item_quantity"].ToString()
									});
								}
							}
							command4.Dispose();
							itemindex++;
						}
						listindex++;
					}
					
					conn.Close();
				}
			}
			TempData["BOMModel"] = JsonConvert.SerializeObject(model);

			Debug.WriteLine(model.lists.Count);
			Debug.WriteLine(model.lists[0].items.Count);
			Debug.WriteLine(model.lists[0].items[0].subitems.Count);

			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> BOMView(BillOfMaterialsModel xmodel)
		{
			BillOfMaterialsModel model = JsonConvert.DeserializeObject<BillOfMaterialsModel>(TempData["BOMModel"].ToString());
			model.Title = xmodel.Title;
			model.Address = xmodel.Address;
			model.ProjectDate = xmodel.ProjectDate;
			model.ProjectRef = xmodel.ProjectRef;
			TempData["BOMModel"] = JsonConvert.SerializeObject(model);
			if (!ModelState.IsValid)
			{
				model.materials = GetMaterialsFromDB();
				model.categories = GetCategoriesFromDB();
				model.measurements = GetMeasurementsFromDB();
				return View(model);
			}

			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				conn.Open();
				using (MySqlTransaction trans = conn.BeginTransaction())
				{
					try
					{
						MySqlCommand command = new MySqlCommand(
							"INSERT INTO `frilou_db`.`bom` (`project_title`,`project_location`,`project_date`,`project_ref`,`project_engineer_id`,`building_material_id`," +
							"`building_storeys`,`building_floorheight`,`building_length`,`building_width`) VALUES " +
							"(@project_title, " +
							"@project_location, " +
							"@project_date, " +
							"@project_ref, " +
							"@project_engineer_id," +
							"@building_material_id," +
							"@building_storeys," +
							"@building_floorheight," +
							"@building_length," +
							"@building_width);" +
							"SELECT last_insert_id() FROM `frilou_db`.`bom`;");
						Debug.WriteLine(model.Title);
						Debug.WriteLine(model.Address);
						Debug.WriteLine(model.ProjectDate.ToString("dd-MM-yyyy"));
						Debug.WriteLine(model.ProjectRef);
						command.Parameters.AddWithValue("@project_title", model.Title);
						command.Parameters.AddWithValue("@project_location", model.Address);
						command.Parameters.AddWithValue("@project_date", model.ProjectDate);
						command.Parameters.AddWithValue("@project_ref", model.ProjectRef);
						command.Parameters.AddWithValue("@project_engineer_id", Convert.ToUInt32(HttpContext.Session.GetInt32("UserID")));
						command.Parameters.AddWithValue("@building_material_id", model.buildingMaterial);
						command.Parameters.AddWithValue("@building_storeys", model.storeys);
						command.Parameters.AddWithValue("@building_floorheight", model.floorHeight);
						command.Parameters.AddWithValue("@building_length", model.length);
						command.Parameters.AddWithValue("@building_width", model.width);

						command.Connection = conn;
						command.Transaction = trans;
						int bom_id = Convert.ToInt32(command.ExecuteScalar());
						command.Dispose();
						Debug.WriteLine("BOM");

						foreach (BOMList list in model.lists)
						{
							MySqlCommand command2 = new MySqlCommand(
								"INSERT INTO `frilou_db`.`bom_lists` (`list_desc`,`bom_id`) VALUES " +
								"(@list_desc," +
								"@bom_id);" +
								"SELECT last_insert_id() FROM `frilou_db`.`bom_lists`;");

							command2.Parameters.AddWithValue("@list_desc", list.Desc);
							command2.Parameters.AddWithValue("@bom_id", bom_id);

							command2.Connection = conn; 
							command2.Transaction = trans;
							int bom_list_id = Convert.ToInt32(command2.ExecuteScalar());
							command2.Dispose();
							Debug.WriteLine("LIST");

							foreach (BOMItems item in list.items)
							{
								MySqlCommand command3 = new MySqlCommand(
								"INSERT INTO `frilou_db`.`bom_items` (`bom_list_id`,`item_id`) VALUES " +
								"(@bom_list_id," +
								"@item_id);" +
								"SELECT last_insert_id() FROM `frilou_db`.`bom_items`;");

								command3.Parameters.AddWithValue("@bom_list_id", bom_list_id);
								command3.Parameters.AddWithValue("@item_id", item.item_id);

								command3.Connection = conn;
								command3.Transaction = trans;
								int bom_list_item_id = Convert.ToInt32(command3.ExecuteScalar());
								command3.Dispose();
								Debug.WriteLine("ITEM");

								foreach (BOMSubitems subitem in item.subitems)
								{
									MySqlCommand command4 = new MySqlCommand(
										"INSERT INTO `frilou_db`.`bom_subitems` (`item_id`,`item_quantity`,`bom_item_id`) VALUES " +
										"(@item_id," +
										"@item_quantity," +
										"@bom_item_id);");

									command4.Parameters.AddWithValue("@item_id", subitem.item_id);
									command4.Parameters.AddWithValue("@item_quantity", subitem.Quantity);
									command4.Parameters.AddWithValue("@bom_item_id", bom_list_item_id);

									command4.Connection = conn;
									command4.Transaction = trans;
									command4.ExecuteNonQuery();
									command4.Dispose();
									Debug.WriteLine("SUBITEM");
								}
							}
						}
						trans.Commit();
					}
					catch (MySqlException e)
					{
						Debug.WriteLine("DB Write Error: " + e.Message);
						trans.Rollback();
					}
				}
			}
			return RedirectToAction("Account");
		}

		public IActionResult BOMAddList()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> BOMAddList(BOMAddListModel listmodel)
		{
			BillOfMaterialsModel model = JsonConvert.DeserializeObject<BillOfMaterialsModel>(TempData["BOMModel"].ToString());
			model.lists.Add(new BOMList()
			{
				Desc = listmodel.Description,
				items = new List<BOMItems>()
			});
			TempData["BOMModel"] = JsonConvert.SerializeObject(model);
			return RedirectToAction("BOMView");
		}

		public IActionResult BOMAddItem(int? id)
		{
			BOMAddItemModel model = new BOMAddItemModel();
			model.listId = id;
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> BOMAddItem(BOMAddItemModel itemmodel, int? id)
		{
			BillOfMaterialsModel model = JsonConvert.DeserializeObject<BillOfMaterialsModel>(TempData["BOMModel"].ToString());
			/*model.lists[Convert.ToInt32(id)].items.Add(new BOMItems()
			{
				Desc = itemmodel.Description,
				subitems = new List<BOMSubitems>()
			});*/
			TempData["BOMModel"] = JsonConvert.SerializeObject(model);
			return RedirectToAction("BOMView");
		}

		public IActionResult BOMAddSubitem(string? id)
		{
			BOMAddSubitemModel model = new BOMAddSubitemModel();
			model.id = id;
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> BOMAddSubitem(BOMAddSubitemModel itemmodel, string? id)
		{
			int[] ids = { Convert.ToInt32(id.Split('s')[0]), Convert.ToInt32(id.Split('s')[1]) };
			BillOfMaterialsModel model = JsonConvert.DeserializeObject<BillOfMaterialsModel>(TempData["BOMModel"].ToString());
			/*model.lists[ids[0]].items[ids[1]].subitems.Add(new BOMSubitems()
			{
				Desc = itemmodel.Description,
				UoM = itemmodel.UoM,
				Quantity = itemmodel.Quantity.ToString()
			});*/
			TempData["BOMModel"] = JsonConvert.SerializeObject(model);
			return RedirectToAction("BOMView");
		}

		public IActionResult BOMEdit(int? id)
		{
			BillOfMaterialsModel model = null;

			if (TempData["BOMModel"] == null && id == null)
			{
				Debug.WriteLine("nuts");
				model = new BillOfMaterialsModel();
				model.lists = new List<BOMList>();
				model.materials = GetMaterialsFromDB();
				model.categories = GetCategoriesFromDB();
				model.measurements = GetMeasurementsFromDB();
			}
			else if (TempData["BOMModel"] != null && id == null)
			{
				Debug.WriteLine("bunger");
				model = JsonConvert.DeserializeObject<BillOfMaterialsModel>(TempData["BOMModel"].ToString());
				model.materials = GetMaterialsFromDB();
				model.categories = GetCategoriesFromDB();
				model.measurements = GetMeasurementsFromDB();
			}

			else if (TempData["BOMModel"] == null && id != null)
			{
				model = new BillOfMaterialsModel();
				model.lists = new List<BOMList>();
				model.materials = GetMaterialsFromDB();
				model.categories = GetCategoriesFromDB();
				model.measurements = GetMeasurementsFromDB();

				int buildingmaterialid = 0;

				using (MySqlConnection conn = new MySqlConnection(connectionstring))
				{
					conn.Open();

					MySqlCommand command1 = new MySqlCommand("SELECT * FROM bom WHERE bom_id = @bom_id");
					command1.Parameters.AddWithValue("@bom_id", id);
					command1.Connection = conn;
					using (MySqlDataReader sdr = command1.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.Title = sdr["project_title"].ToString();
							model.Address = sdr["project_location"].ToString();
							model.ProjectRef = sdr["project_ref"].ToString();
							model.ProjectDate = DateTime.Parse(sdr["project_date"].ToString());
							model.Engineer_ID = Convert.ToUInt32(sdr["project_engineer_id"]);
							buildingmaterialid = Convert.ToInt32(sdr["building_material_id"]);
							model.storeys = Convert.ToInt32(sdr["building_storeys"]);
							model.floorHeight = Convert.ToDouble(sdr["building_floorheight"]);
							model.length = Convert.ToDouble(sdr["building_length"]);
							model.width = Convert.ToDouble(sdr["building_width"]);
						}
					}
					command1.Dispose();

					MySqlCommand buildingmaterialcommand = new MySqlCommand("SELECT * FROM building_material WHERE building_material_id = @building_material_id");
					buildingmaterialcommand.Parameters.AddWithValue("@building_material_id", buildingmaterialid);
					buildingmaterialcommand.Connection = conn;
					using (MySqlDataReader sdr = buildingmaterialcommand.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.buildingMaterial = buildingmaterialid.ToString();
							model.buildingMaterialDesc = sdr["building_material_desc"].ToString();
						}
					}
					buildingmaterialcommand.Dispose();

					MySqlCommand command2 = new MySqlCommand("SELECT * FROM bom_lists WHERE bom_id = @bom_id;");
					command2.Parameters.AddWithValue("@bom_id", id);
					command2.Connection = conn;
					using (MySqlDataReader sdr = command2.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.lists.Add(new BOMList()
							{
								id = Convert.ToInt32(sdr["bom_list_id"]),
								Desc = sdr["list_desc"].ToString(),
								items = new List<BOMItems>()
							});
						}
					}
					command2.Dispose();

					int listindex = 0;
					foreach (BOMList list in model.lists)
					{
						MySqlCommand command3 = new MySqlCommand("SELECT * FROM bom_items WHERE bom_list_id = @bom_list_id");

						command3.Parameters.AddWithValue("@bom_list_id", list.id);
						command3.Connection = conn;
						using (MySqlDataReader sdr = command3.ExecuteReader())
						{
							while (sdr.Read())
							{
								model.lists[listindex].items.Add(new BOMItems()
								{
									id = Convert.ToInt32(sdr["bom_item_id"]),
									item_id = Convert.ToUInt32(sdr["item_id"]),
									subitems = new List<BOMSubitems>()
								});
							}
						}
						command3.Dispose();


						int itemindex = 0;
						foreach (BOMItems item in list.items)
						{
							MySqlCommand command4 = new MySqlCommand("SELECT * FROM bom_subitems WHERE bom_item_id = @bom_item_id");
							command4.Parameters.AddWithValue("@bom_item_id", item.id);
							command4.Connection = conn;
							using (MySqlDataReader sdr = command4.ExecuteReader())
							{
								while (sdr.Read())
								{
									model.lists[listindex].items[itemindex].subitems.Add(new BOMSubitems()
									{
										id = Convert.ToInt32(sdr["bom_item_id"]),
										item_id = Convert.ToUInt32(sdr["item_id"]),
										Quantity = sdr["item_quantity"].ToString()
									});
								}
							}
							command4.Dispose();
							itemindex++;
						}
						listindex++;
					}

					conn.Close();
				}
			}
			TempData["BOMModel"] = JsonConvert.SerializeObject(model);

			Debug.WriteLine(model.lists.Count);
			Debug.WriteLine(model.lists[0].items.Count);
			Debug.WriteLine(model.lists[0].items[0].subitems.Count);

			return View(model);
		}



		public List<MaterialItem> GetMaterialsFromDB()
		{
			List<MaterialItem> materials = new List<MaterialItem>();
			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM materials;"))
				{
					command.Connection = conn;
					conn.Open();
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							double? Length = null, Width = null, Height = null, Weight = null, Volume = null;
							if (sdr["length"] != DBNull.Value)
								Length = Convert.ToDouble(sdr["length"]);
							if (sdr["width"] != DBNull.Value)
								Width = Convert.ToDouble(sdr["width"]);
							if (sdr["height"] != DBNull.Value)
								Height = Convert.ToDouble(sdr["height"]);
							if (sdr["weight"] != DBNull.Value)
								Weight = Convert.ToDouble(sdr["weight"]);
							if (sdr["volume"] != DBNull.Value)
								Volume = Convert.ToDouble(sdr["volume"]);

							materials.Add(new MaterialItem()
							{
								material_id_string = sdr["material_id"].ToString(),
								material_desc = sdr["material_desc"].ToString(),
								material_long_desc = sdr["material_desc_long"].ToString(),
								unit_id = Convert.ToUInt32(sdr["unit_id"]),
								category_id = Convert.ToUInt32(sdr["category_id"]),
								manufacturer_id = Convert.ToUInt32(sdr["manufacturer_id"]),
								price = Convert.ToDouble(sdr["price"]),
								length = Length,
								width = Width,
								height = Height,
								weight = Weight,
								volume = Volume
							});
						}
					}
					conn.Close();
				}
			}
			return materials;
		}

		public List<CategoryList> GetCategoriesFromDB()
		{
			List<CategoryList> materials = new List<CategoryList>();
			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM material_categories;"))
				{
					command.Connection = conn;
					conn.Open();
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							materials.Add(new CategoryList()
							{
								Id = sdr["category_id"].ToString(),
								description = sdr["category_desc"].ToString()
							});
						}
					}
					conn.Close();
				}
			}
			return materials;
		}

		public List<MeasurementList> GetMeasurementsFromDB()
		{
			List<MeasurementList> materials = new List<MeasurementList>();
			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM measurement_units;"))
				{
					command.Connection = conn;
					conn.Open();
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							materials.Add(new MeasurementList()
							{
								Id = sdr["measurment_unit_id"].ToString(),
								description = sdr["unit_desc"].ToString()
							});
						}
					}
					conn.Close();
				}
			}
			return materials;
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

		
    }
}