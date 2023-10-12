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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Drawing.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Data.Entity.Core.Metadata.Edm;

namespace Frilou_UI_V2.Controllers
{
	public class BOMController : Controller
	{
		private readonly ILogger<BOMController> _logger;
		private readonly string connectionstring = "Data Source=localhost;port=3306;Initial Catalog=bom_mce_db;User Id=root;password=password123;";
		BOMListOfMaterials mat = new BOMListOfMaterials();
		List<BOMTemplate> temp = new List<BOMTemplate>();

		public BOMController(ILogger<BOMController> logger)
		{
			_logger = logger;
			mat = new BOMListOfMaterials();
			mat.Materials = new List<BOMMaterialItem>();
			mat.Materials.Add(new BOMMaterialItem()
			{
				ID = 1,
				Description = "Plywood",
				IsUsed = false
			});
			mat.Materials.Add(new BOMMaterialItem()
			{
				ID = 2,
				Description = "Cement",
				IsUsed = false
			});
			mat.Materials.Add(new BOMMaterialItem()
			{
				ID = 3,
				Description = "Sand",
				IsUsed = false
			});
			mat.Materials.Add(new BOMMaterialItem()
			{
				ID = 4,
				Description = "Aggregate",
				IsUsed = false
			});
			mat.Materials.Add(new BOMMaterialItem()
			{
				ID = 5,
				Description = "Rebar",
				IsUsed = false
			});
			mat.Materials.Add(new BOMMaterialItem()
			{
				ID = 6,
				Description = "Hollow Block",
				IsUsed = false
			});

			temp = new List<BOMTemplate>();
			temp.Add(new BOMTemplate()
			{
				ID = 0,
				Description = "Bungalow",
				Description_Long = "Single storey single family residential house",
				Size = "15m by 15m, 1 Floor",
				Length = 15,
				Width = 15,
				Storeys = 1,
				StoreyHeight = 2,
				Material = "Concrete",
				Materials = mat.Materials.Select(item => new BOMMaterialItem() { ID = item.ID, Description = item.Description, IsUsed = item.IsUsed }).ToList()
			});
			temp.Add(new BOMTemplate()
			{
				ID = 1,
				Description = "Townhouse",
				Description_Long = "Two storey single family residential house",
				Size = "12m by 10m, 2 Floors",
				Length = 12,
				Width = 10,
				Storeys = 2,
				StoreyHeight = 1.5,
				Material = "Concrete",
				Materials = mat.Materials.Select(item => new BOMMaterialItem() { ID = item.ID, Description = item.Description, IsUsed = item.IsUsed }).ToList()
			});
			temp.Add(new BOMTemplate()
			{
				ID = 2,
				Description = "Duplex",
				Description_Long = "Two storey two family residential house",
				Size = "15m by 15m, 2 Floors",
				Length = 15,
				Width = 15,
				Storeys = 2,
				StoreyHeight = 1.5,
				Material = "Concrete",
				Materials = mat.Materials.Select(item => new BOMMaterialItem() { ID = item.ID, Description = item.Description, IsUsed = item.IsUsed }).ToList()
			});


			temp[0].Materials[0].IsUsed = true;
			temp[1].Materials[1].IsUsed = true;
			temp[1].Materials[2].IsUsed = true;
			temp[1].Materials[3].IsUsed = true;
			temp[2].Materials[1].IsUsed = true;
			temp[2].Materials[2].IsUsed = true;
			temp[2].Materials[3].IsUsed = true;
			temp[2].Materials[5].IsUsed = true;
		}

		public IActionResult Index(int? id)
		{
			BOMListOfMaterials model = mat;
			List<BOMTemplate> template = temp;

			if (id != null)
			{
				Debug.WriteLine("A");
				model.ProjectType = template[(int)id].Description;
				for(int i = 0; i < model.Materials.Count; i++)
				{
					model.Materials[i].IsUsed = template[(int)id].Materials[i].IsUsed;
					model.Length = template[(int)id].Length;
					model.Width = template[(int)id].Length;
					model.StoreyHeight = template[(int)id].StoreyHeight;
					model.Storeys = template[(int)id].Storeys;
				} 
			}
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(BOMListOfMaterials model)
		{
			Debug.WriteLine("helvete");
			string output = "";
			foreach(BOMMaterialItem item in model.Materials) 
			{
				output += item.Description + ": ";
				output += item.IsUsed + "\n";
			}
			Debug.WriteLine(output);
			Debug.WriteLine("ingen");

			TempData["BOMMaterials"] = JsonConvert.SerializeObject(model);
			return RedirectToAction("Specifications");
		}


		public IActionResult Templates()
		{
			List<BOMTemplate> model = temp;
			return View(model);
		}

		public IActionResult Specifications()
		{
			BOMSpecifications model = new BOMSpecifications();
			model.materials = JsonConvert.DeserializeObject<BOMListOfMaterials>(TempData["BOMMaterials"].ToString());
			model.BuildingLength = model.materials.Length;
			model.BuildingWidth = model.materials.Width;
			model.NumberOfStoreys = model.materials.Storeys;
			model.FloorHeight = model.materials.StoreyHeight;

			TempData["BOMMaterials"] = JsonConvert.SerializeObject(model.materials);
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Specifications(BOMSpecifications model)
		{
			BOM_Information info = new BOM_Information();
			info.specs = model;
			string location = model.Latitude + "," + model.Longtitude;
			Debug.WriteLine(model.materials.Materials.Count);
			info.lists = new List<BOM_Materials_Lists>();

			int					noOfStories = model.NumberOfStoreys;
			double				heightOfFloors = model.FloorHeight,
								lengthOfBuilding = model.BuildingLength,
								widthOfBuilding = model.BuildingWidth,
								sqmOfBuilding = heightOfFloors * lengthOfBuilding,
								π = 3.14159f

				;

			// measurements that are changeable
			double				floorThickness = 0.127,
								wallThickness = 0.254,
								rebarPercentage = 0.10, //percentage of reinforcement by volume
								rebarDiameter = 0.01,
								nailConstant = 20,
								hollowBlockLength = 0.2,
								hollowBlockWidth = 0.2,
								hollowBlockHeight = 0.2,
								hollowBlockVolume = hollowBlockHeight * hollowBlockLength * hollowBlockWidth,
								hollowBlockConstant = 12.5,
								supportBeamLength = 0.25,
								supportBeamWidth = 0.30,
								supportBeamArea = supportBeamLength * supportBeamWidth,
								supportBeamSpace = 2.92,
								supportBeamVolume = supportBeamArea * heightOfFloors,
								supportBeamsNeeded = sqmOfBuilding / supportBeamSpace,
								concreteFormulaCement = 1,
								concreteFormulaSand = 2,
								concreteFormulaAggregate = 3,
			/* square inches */ plywoodLength = 96,
			/* square inches */ plywoodWidth = 48,
			/* square inches */ plywoodArea = plywoodLength * plywoodWidth,
								plywoodSheetsPerSqm = (double)Math.Ceiling(10764 / plywoodArea),
								riserHeight = 0.178,
								threadDepth = 0.2794,
								numberOfSteps = (double)Math.Ceiling(heightOfFloors / riserHeight)

				;

			// prices, are changeable
			double				rebarPrice = GetBestPrice(5, location).Price, // per piece. Rebars * rebarPrice
								hollowBlockPrice = GetBestPrice(6, location).Price, // per piece. NoOfHollowBlock * hollowBlockPrice
								cementPrice = GetBestPrice(2, location).Price, // per cubic meter. CementCubicMeters * cementPrice,
								sandPrice = GetBestPrice(3, location).Price, // per cubic meter
								sandReducedPrice = (1 / 3) * sandPrice,
								sandBigPrice = 2 * sandPrice,
								aggregatePrice = GetBestPrice(4, location).Price, // per cubic meter
								aggregateReducedPrice = (1 / 2) * aggregatePrice,
								aggregateBigPrice = 3 * aggregatePrice,
								concretePrice = cementPrice + sandPrice + aggregatePrice,
								concreteReducedPrice = cementPrice + sandReducedPrice + aggregateReducedPrice,
								concreteBigPrice = cementPrice + sandBigPrice + aggregateBigPrice,
								plywoodPrice = GetBestPrice(1, location).Price, // per piece 1/4  
								plywoodPricePerSqm = plywoodPrice * plywoodSheetsPerSqm

				;

			double concreteTotalRatio = concreteFormulaCement + concreteFormulaSand + concreteFormulaAggregate;
			double cementRatio = concreteFormulaCement / concreteTotalRatio;
			double sandRatio = concreteFormulaSand / concreteTotalRatio;
			double aggregateRatio = concreteFormulaAggregate / concreteTotalRatio;

			// foundation
			double foundationHeight = heightOfFloors * noOfStories + (noOfStories * floorThickness),
								foundationVolume = foundationHeight * sqmOfBuilding,
								foundationPerimeter = 2 * (lengthOfBuilding + widthOfBuilding),
								foundationWallArea = 4 * foundationPerimeter * foundationHeight,
			/* print */         foundationNoOfHollowBlock = ((foundationWallArea * hollowBlockConstant) * 1.10) * 1.15,
			/* print */         foundationRebar = (double)((((foundationVolume * rebarPercentage) / (Math.Pow(π * (rebarDiameter / 2), 2))))*1.10)*1.15,
			/* print */         foundationConcrete = (foundationVolume * 1.10) * 1.15
				;

			info.lists.Add(new BOM_Materials_Lists()
			{
				Description = $"Foundation",
				ListNumber = 1,
				Items = new List<BOM_Materials_Items>()
			});
			info.lists[0].Items.Add(new BOM_Materials_Items()
			{
				Description = $"Foundation",
				Subitems = new List<BOM_Materials_Subitems>()
			});
			int foundationConcreteCement = (int)Math.Ceiling(foundationConcrete * cementRatio);
			info.lists[0].Items[0].Subitems.Add(GetMaterial(2, foundationConcreteCement, location, 1, cementPrice));

			int foundationConcreteSand = (int)Math.Ceiling(foundationConcrete * sandRatio);
			info.lists[0].Items[0].Subitems.Add(GetMaterial(3, foundationConcreteSand, location, 2, sandPrice));

			int foundationConcreteAggregate = (int)Math.Ceiling(foundationConcrete * aggregateRatio);
			info.lists[0].Items[0].Subitems.Add(GetMaterial(4, foundationConcreteAggregate, location, 3, aggregatePrice));

			int foundationRebarAmount = (int)Math.Ceiling(foundationRebar);
			info.lists[0].Items[0].Subitems.Add(GetMaterial(5, foundationRebarAmount, location, 4, rebarPrice));

			int foundationHollowBlock = (int)Math.Ceiling(foundationNoOfHollowBlock);
			info.lists[0].Items[0].Subitems.Add(GetMaterial(6, foundationHollowBlock, location, 5, hollowBlockPrice));

			// stories. repeat for every storey the building has
			for (int i = 1; i <= model.NumberOfStoreys; i++)
			{
				info.lists.Add(new BOM_Materials_Lists()
				{
					Description = $"Storey {i}",
					ListNumber = i + 1,
					Items = new List<BOM_Materials_Items>()
				});
				info.lists[i].Items.Add(new BOM_Materials_Items()
				{
					Description = $"Storey {i} floor",
					Subitems = new List<BOM_Materials_Subitems>()
				});
				info.lists[i].Items.Add(new BOM_Materials_Items()
				{
					Description = $"Storey {i} walls",
					Subitems = new List<BOM_Materials_Subitems>()
				});
				info.lists[i].Items.Add(new BOM_Materials_Items()
				{
					Description = $"Storey {i} support beams",
					Subitems = new List<BOM_Materials_Subitems>()
				});
				info.lists[i].Items.Add(new BOM_Materials_Items()
				{
					Description = $"Storey {i} stairs",
					Subitems = new List<BOM_Materials_Subitems>()
				});
				double			storeyHeight = heightOfFloors + floorThickness,
								storeyPerimeter = 2 * (lengthOfBuilding + widthOfBuilding),
								storeyWallVolume = 4 * storeyPerimeter * storeyHeight,
								storeyFloorVolume = sqmOfBuilding * floorThickness,
								// floors
								storeyFloorPlywood = ((plywoodSheetsPerSqm * sqmOfBuilding)*1.10)*1.15,
								storeyFloorNails = ((storeyFloorPlywood * nailConstant)*1.1)*1.15,
								storeyFloorConcrete = ((storeyFloorVolume)*1.1)*1.15,
								storeyFloorRebar = (double)((((storeyFloorVolume * rebarPercentage) / (Math.Pow(π * (rebarDiameter / 2), 2))))*1.1)*1.15,

								// walls
								storeyWallConcrete = (storeyWallVolume * 1.1)*1.15,
								storeyWallRebar = (double)((((storeyWallVolume * rebarPercentage) / (Math.Pow(π * (rebarDiameter / 2), 2))))*1.1)*1.15,


								// support beams
								storeySupportBeamsNeeded = supportBeamsNeeded,
								storeySupportBeamsConcrete = (supportBeamVolume*1.1)*1.15,
								storeySupportBeamsRebar = (double)((((supportBeamVolume * rebarPercentage) / (Math.Pow(π * (rebarDiameter / 2), 2))))*1.1)*1.15,

								// stairs
								stairsVolume = numberOfSteps * riserHeight * threadDepth,
								stairsConcrete = (stairsVolume*1.1)*1.15,
								stairsRebar = (double)((((stairsVolume * rebarPercentage) / (Math.Pow(π * (rebarDiameter / 2), 2))))*1.1)*1.15
				;

				string deb = $"storey: {storeyFloorConcrete}";
				
				Debug.WriteLine(deb);
				//floor
				int floorConcreteCement = (int)Math.Ceiling(storeyFloorConcrete * cementRatio);
				info.lists[i].Items[0].Subitems.Add(GetMaterial(2, floorConcreteCement, location, 1, cementPrice));

				int floorConcreteSand = (int)Math.Ceiling(storeyFloorConcrete * sandRatio);
				info.lists[i].Items[0].Subitems.Add(GetMaterial(3, floorConcreteSand, location, 2, sandPrice));

				int floorConcreteAggregate = (int)Math.Ceiling(storeyFloorConcrete * aggregateRatio);
				info.lists[i].Items[0].Subitems.Add(GetMaterial(4, floorConcreteAggregate, location, 3, aggregatePrice));

				int floorRebarAmount = (int)Math.Ceiling(storeyFloorRebar);
				info.lists[i].Items[0].Subitems.Add(GetMaterial(5, floorRebarAmount, location, 4, rebarPrice));


				//wall
				int wallConcreteCement = (int)Math.Ceiling(storeyWallConcrete * cementRatio);
				info.lists[i].Items[1].Subitems.Add(GetMaterial(2, wallConcreteCement, location, 1, cementPrice));
				int wallConcreteSand = (int)Math.Ceiling(storeyWallConcrete * sandRatio);
				info.lists[i].Items[1].Subitems.Add(GetMaterial(3, wallConcreteSand, location, 2, sandPrice));
				int wallConcreteAggregate = (int)Math.Ceiling(storeyWallConcrete * aggregateRatio);
				info.lists[i].Items[1].Subitems.Add(GetMaterial(4, wallConcreteAggregate, location, 3, aggregatePrice));

				int wallRebarAmount = (int)Math.Ceiling(storeyWallRebar);
				info.lists[i].Items[1].Subitems.Add(GetMaterial(5, wallRebarAmount, location, 4, rebarPrice));

				//beam
				int beamConcreteCement = (int)Math.Ceiling(storeySupportBeamsConcrete * cementRatio);
				info.lists[i].Items[2].Subitems.Add(GetMaterial(2, beamConcreteCement, location, 1, cementPrice));
				int beamConcreteSand = (int)Math.Ceiling(storeySupportBeamsConcrete * sandRatio);
				info.lists[i].Items[2].Subitems.Add(GetMaterial(3, beamConcreteSand, location, 2, sandPrice));
				int beamConcreteAggregate = (int)Math.Ceiling(storeySupportBeamsConcrete * aggregateRatio);
				info.lists[i].Items[2].Subitems.Add(GetMaterial(4, beamConcreteAggregate, location, 3, aggregatePrice));

				int beamRebarAmount = (int)Math.Ceiling(storeySupportBeamsRebar);
				info.lists[i].Items[2].Subitems.Add(GetMaterial(5, beamRebarAmount, location, 4, rebarPrice));

				//stair
				int stairsConcreteCement = (int)Math.Ceiling(stairsConcrete * cementRatio);
				info.lists[i].Items[3].Subitems.Add(GetMaterial(2, stairsConcreteCement, location, 1, cementPrice));
				int stairsConcreteSand = (int)Math.Ceiling(stairsConcrete * sandRatio);
				info.lists[i].Items[3].Subitems.Add(GetMaterial(3, stairsConcreteSand, location, 2, sandPrice));
				int stairsConcreteAggregate = (int)Math.Ceiling(stairsConcrete * aggregateRatio);
				info.lists[i].Items[3].Subitems.Add(GetMaterial(4, stairsConcreteAggregate, location, 3, aggregatePrice));

				int stairsRebarAmount = (int)Math.Ceiling(stairsRebar);
				info.lists[i].Items[3].Subitems.Add(GetMaterial(5, stairsRebarAmount, location, 4, rebarPrice));
			}

			info.totalCost = 0;
			foreach (BOM_Materials_Lists x in info.lists)
			{
				foreach(BOM_Materials_Items y in x.Items)
				{
					foreach (BOM_Materials_Subitems z in y.Subitems)
					{
						info.totalCost += (z.MaterialCost * Math.Ceiling((double)z.MaterialQuantity));
					}
				}
			}

			TempData["BOMModel"] = JsonConvert.SerializeObject(info);

			return RedirectToAction("BillOfMaterials");
		}

		public IActionResult BillOfMaterials()
		{
			BOM_Information model = null;
			if (TempData["BOMModel"] != null)
			{
				model = JsonConvert.DeserializeObject<BOM_Information>(TempData["BOMModel"].ToString());
				TempData["BOMModel"] = JsonConvert.SerializeObject(model);
			}
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> BillOfMaterials(BOM_Information model)
		{
			Debug.WriteLine("pMCE: " + model.lists.Count);

			TempData["BOMModel"] = JsonConvert.SerializeObject(model);
			//return View(model);
			return RedirectToAction("MateriaslCostEstimate");
		}

		public IActionResult MateriaslCostEstimate()
		{
			MCE_Information model = new MCE_Information();
			model.info = JsonConvert.DeserializeObject<BOM_Information>(TempData["BOMModel"].ToString());
			//TempData["BOMModel"] = JsonConvert.SerializeObject(model.info);
			//TempData["MCEModel"] = JsonConvert.SerializeObject(model);
			model.materialCost = model.info.totalCost;
			Debug.WriteLine("MCE: " + model.info.lists.Count);
			return View(model);
		}


		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> MateriaslCostEstimate(MCE_Information model, string submitButton)
		{
			
			if (submitButton == "Submit1")
			{
				double temp = 0;
				for (int x = 0; x < model.info.lists.Count; x++)
				{
					for (int y = 0; y < model.info.lists[x].Items.Count; y++)
					{
						for (int z = 0; z < model.info.lists[x].Items[y].Subitems.Count; z++)
						{
							model.info.lists[x].Items[y].Subitems[z].TotalUnitRate =
								model.info.lists[x].Items[y].Subitems[z].MaterialCost +
								model.info.lists[x].Items[y].Subitems[z].LabourCost;
							model.info.lists[x].Items[y].Subitems[z].MaterialAmount =
								model.info.lists[x].Items[y].Subitems[z].TotalUnitRate *
								model.info.lists[x].Items[y].Subitems[z].MaterialQuantity;
							temp += model.info.lists[x].Items[y].Subitems[z].MaterialAmount;
						}
					}
				}
				model.totalCost = temp;
				return View(model);
			}
			else if (submitButton == "Submit2")
			{

			}
			return View();
		}


		public ActionResult MCE_EditRow(string? ids)
		{
			Debug.WriteLine("AAASSS");

			string[] idd = ids.Split('s');
			int[] id = { Convert.ToInt32(idd[0]), Convert.ToInt32(idd[1]), Convert.ToInt32(idd[2]) };

			MCE_Information model = new MCE_Information();
			model = JsonConvert.DeserializeObject<MCE_Information>(TempData["MCEModel"].ToString());
			TempData["MCEModel"] = JsonConvert.SerializeObject(model);

			MCE_ItemEditModel editmodel = new MCE_ItemEditModel();
			editmodel.MaterialDesc = model.info.lists[id[0]].Items[id[1]].Subitems[id[2]].MaterialDesc;
			editmodel.MaterialUoM = model.info.lists[id[0]].Items[id[1]].Subitems[id[2]].MaterialUoM;
			editmodel.MaterialQuantity = model.info.lists[id[0]].Items[id[1]].Subitems[id[2]].MaterialQuantity;
			editmodel.MaterialCost = model.info.lists[id[0]].Items[id[1]].Subitems[id[2]].MaterialCost;
			editmodel.MaterialAmount = model.info.lists[id[0]].Items[id[1]].Subitems[id[2]].MaterialAmount;
			editmodel.LabourCost = model.info.lists[id[0]].Items[id[1]].Subitems[id[2]].LabourCost;
			editmodel.TotalUnitRate = model.info.lists[id[0]].Items[id[1]].Subitems[id[2]].TotalUnitRate;

			return View(editmodel);
		}



		public BOM_Materials_Subitems GetMaterial(int material_id, double Quantity, string destination, int subitem_num, double cost)
		{
			BOM_Materials_Subitems item = new BOM_Materials_Subitems();
			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				conn.Open();
				using (MySqlCommand command = new MySqlCommand("SELECT a.material_id, a.material_desc, b.measurement_unit_desc_short FROM materials a " +
					" INNER JOIN measurement_units b " +
					" ON a.measurement_unit_id = b.measurement_unit_id " +
					" WHERE material_id = @material_id;"))
				{
					command.Parameters.AddWithValue("@material_id", material_id);
					command.Connection = conn;
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							item.SubitemNumber = subitem_num;
							item.MaterialID = Convert.ToInt32(sdr["material_id"]);
							item.MaterialDesc = sdr["material_desc"].ToString();
							item.MaterialUoM = sdr["measurement_unit_desc_short"].ToString();
							item.MaterialQuantity = (int)Math.Ceiling(Quantity);
							item.MaterialCost = cost;
							item.MaterialAmount = Math.Ceiling(Quantity) * cost;
						}
					}
				}
			}
			return item;
		}

		public BOMItems GetConcreteMaterials(double ConcreteQuantity, double RebarQuantity, string destination)
		{
			BOMItems item = new BOMItems();
			item.subitems = new List<BOMSubitems>();
			double[] ratio = { 0, 0, 0 }, ratio_final = { 0, 0, 0 };
			double ratio_total = 0;
			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				conn.Open();
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM material_categories WHERE category_id = 2;"))
				{
					command.Connection = conn;
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							item.item_id = Convert.ToUInt32(sdr["category_id"]);
							item.item_desc = sdr["category_desc"].ToString();
						}
					}
				}
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM materials WHERE material_id IN (2,3,4,5);"))
				{
					command.Connection = conn;
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							item.subitems.Add(new BOMSubitems()
							{
								item_id = Convert.ToUInt32(sdr["material_id"]),
								subitem_desc = sdr["material_desc"].ToString()
							});
						}
					}
				}
				using (MySqlCommand command = new MySqlCommand("SELECT b.* FROM employee_info a " +
					"JOIN employee_formula_constants b " +
					"ON a.formula_constants_id = b.formula_constants_id " +
					"WHERE a.employee_info_id = @id;"))
				{
					command.Connection = conn;
					command.Parameters.AddWithValue("@id", HttpContext.Session.GetInt32("EmployeeID"));
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							ratio[0] = Convert.ToInt32(sdr["concrete_ratio_cement"]);
							ratio[1] = Convert.ToInt32(sdr["concrete_ratio_sand"]);
							ratio[2] = Convert.ToInt32(sdr["concrete_ratio_aggregate"]);
						}
					}
				}
			}
			Debug.WriteLine("EEE: " + item.subitems.Count());
			ratio_total = ratio[0] + ratio[1] + ratio[2];
			for (int i = 0; i < ratio.Count(); i++)
			{
				ratio_final[i] = ratio[i] / ratio_total;
			}


			for (int i = 0; i < item.subitems.Count; i++)
			{
				MaterialsCostComparisonItem price = GetBestPrice(item.subitems[i].item_id, destination);
				{
					item.subitems[i].subitem_cost = price.Price.ToString();
					item.subitems[i].Supplier = price.SupplierDesc;
				}
			}

			for (int i = 0; i < 3; i++)
			{
				item.subitems[i].Quantity = Math.Ceiling(ConcreteQuantity * ratio_final[i]).ToString();
				item.subitems[i].Amount = Math.Round(Convert.ToDouble(item.subitems[i].Quantity) * Convert.ToDouble(item.subitems[i].subitem_cost), 2).ToString();
			}
			item.subitems[3].Quantity = Math.Ceiling(RebarQuantity).ToString();
			item.subitems[3].Amount = Math.Round(Convert.ToDouble(RebarQuantity) * Convert.ToDouble(item.subitems[3].subitem_cost), 2).ToString();
			return item;
		}

		public BOMItems GetBrickMaterials(double Quantity, string destination)
		{
			BOMItems item = new BOMItems();
			item.subitems = new List<BOMSubitems>();
			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				conn.Open();
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM material_categories WHERE category_id = 3;"))
				{
					command.Connection = conn;
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							item.item_id = Convert.ToUInt32(sdr["category_id"]);
							item.item_desc = sdr["category_desc"].ToString();
						}
					}
				}
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM materials WHERE material_id IN (1);"))
				{
					command.Connection = conn;
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							item.subitems.Add(new BOMSubitems()
							{
								item_id = Convert.ToUInt32(sdr["material_id"]),
								subitem_desc = sdr["material_desc"].ToString()
							});
						}
					}
				}
			}


			for (int i = 0; i < item.subitems.Count; i++)
			{
				MaterialsCostComparisonItem price = GetBestPrice(item.subitems[i].item_id, destination);
				{
					item.subitems[i].subitem_cost = price.Price.ToString();
					item.subitems[i].Supplier = price.SupplierDesc;
					item.subitems[i].Quantity = Math.Ceiling(Quantity).ToString();
					item.subitems[i].Amount = Math.Round(Quantity * Convert.ToDouble(item.subitems[i].subitem_cost), 2).ToString();
				}
			}

			return item;
		}

		private MaterialsCostComparisonItem GetBestPrice(uint MaterialID, string destination)
		{
			string _apikey = "ApFkiZUGSuNuTphyHstPFnkvL0IGwOKelabezyQVt4RwYTD-yE5n5dMgmeHugQgN";

			List<MaterialsCostComparisonItem> MaterialsCosts = new List<MaterialsCostComparisonItem>();
			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				conn.Open();
				using (MySqlCommand command = new MySqlCommand("SELECT c.material_id AS MaterialID, c.material_desc_long AS Material, " +
					"a.supplier_material_price AS Price, d.supplier_id AS SupplierID, d.supplier_desc AS Supplier, " +
					"CONCAT(d.supplier_coordinates_latitude, ',', d.supplier_coordinates_longtitude) AS Coordinates " +
					"FROM supplier_materials a JOIN (" +
					"SELECT MIN(b.supplier_material_price) AS min_value FROM supplier_materials b WHERE b.material_id = @id AND b.supplier_material_availability = 1) min_table " +
					"ON a.supplier_material_price = min_table.min_value " +
					"INNER JOIN materials c ON a.material_id = c.material_id " +
					"INNER JOIN supplier_info d ON a.supplier_id = d.supplier_id " +
					"WHERE a.material_id = @id " +
					"AND a.supplier_material_availability = 1;"))
				{
					command.Parameters.AddWithValue("@id", MaterialID);
					command.Connection = conn;
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							MaterialsCosts.Add(new MaterialsCostComparisonItem()
							{
								MaterialID = Convert.ToUInt32(sdr["MaterialID"]),
								Description_Long = sdr["Material"].ToString(),
								Price = Convert.ToDouble(sdr["Price"]) / 100,
								SupplierID = Convert.ToUInt32(sdr["SupplierID"]),
								SupplierDesc = sdr["Supplier"].ToString(),
								SupplierCoords = sdr["Coordinates"].ToString()
							});

						}
					}
				}
				conn.Close();
			}
			Debug.WriteLine("AAA");

			Debug.WriteLine(destination);
			Debug.WriteLine(MaterialsCosts[0].SupplierCoords);


			if (MaterialsCosts.Count > 1)
			{
				Debug.WriteLine("BBB");
				List<string> coords = new List<string>();
				foreach (MaterialsCostComparisonItem x in MaterialsCosts)
				{
					coords.Add(x.SupplierCoords);
				}
				Debug.WriteLine(coords.Count);
				foreach (string s in coords)
				{
					Debug.WriteLine(s);
				}
				Debug.WriteLine("d: " + destination);
				BingMapsService bing = new BingMapsService(_apikey);
				List<double> distances = bing.GetDistancesAsync(coords, destination).Result;


				int lowestIndex = 0;
				double lowestValue = distances[0];

				for (int i = 0; i < MaterialsCosts.Count; i++)
				{
					MaterialsCosts[i].Distance = distances[0];
					if (distances[0] < lowestValue)
					{
						lowestIndex = i;
						lowestValue = distances[0];
					}
				}

				return MaterialsCosts[lowestIndex];
			}
			else
			{
				Debug.WriteLine("CCC");
				return MaterialsCosts[0];
			}
		}




		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
