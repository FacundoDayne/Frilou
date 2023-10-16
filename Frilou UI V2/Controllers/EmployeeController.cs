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

using Microsoft.Data.SqlClient;


namespace Frilou_UI_V2.Controllers
{
	public class EmployeeController : Controller
	{
		private readonly string oldconnectionstring = "Data Source=localhost;port=3306;Initial Catalog=bom_mce_db;User Id=root;password=password123;";
		private readonly string connectionstring = @"Server=LAPTOP-HJA4M31O\SQLEXPRESS;Database=bom_mce_db;User Id=bom_debug;Password=password123;Encrypt=False;Trusted_Connection=False;MultipleActiveResultSets=true";

		public IActionResult Index()
		{
			EmployeeDashboardModel model = new EmployeeDashboardModel();
			model.projects = GetNewProjects(HttpContext.Session.GetInt32("EmployeeID"));
			return View(model);
		}

		public List<EmployeeNewProject> GetNewProjects(int? id)
		{
			List<EmployeeNewProject> projects = new List<EmployeeNewProject>();

			using (SqlConnection conn = new SqlConnection(connectionstring))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("SELECT * FROM projects a " +
					"LEFT JOIN bom b " +
					"ON a.project_id = b.project_id " +
					"WHERE b.project_id IS NULL " +
					"AND project_engineer_id = @project_engineer_id;"))
				{
					command.Parameters.AddWithValue("@project_engineer_id", (int)id);
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							projects.Add(new EmployeeNewProject()
							{ 
								ID = Convert.ToInt32(sdr["project_id"]),
								Title = sdr["project_title"].ToString(),
								ClientName = sdr["project_client_name"].ToString(),
								Date = DateTime.Parse(sdr["project_date"].ToString())
							});
						}
					}
				}
			}
			return projects;
		}

		public IActionResult Templates()
		{
			List<TemplateListItem> model = new List<TemplateListItem>();

			using (SqlConnection conn = new SqlConnection(connectionstring))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("SELECT * FROM employee_templates WHERE employee_id = @employee_id;"))
				{
					command.Parameters.AddWithValue("@employee_id", HttpContext.Session.GetInt32("EmployeeID"));
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.Add(new TemplateListItem()
							{
								ID = Convert.ToInt32(sdr["template_id"]),
								Descritpion = sdr["template_description"].ToString(),
								Long_Description = sdr["template_descritpion_long"].ToString()
							});
						}
					}
				}
			}
			return View(model);
		}

		public IActionResult NewTemplate()
		{
			NewTemplateModel model = new NewTemplateModel();
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> NewTemplate(NewTemplateModel model)
		{
			int id = 0;
			using (SqlConnection conn = new SqlConnection(connectionstring))
			{
				conn.Open();
				using (SqlTransaction transaction = conn.BeginTransaction())
				{
					using (SqlCommand command = new SqlCommand("INSERT INTO template_formula_constants " +
						"(floor_thickness,wall_thickness,rebar_thickness,nail_interval,hollow_block_constant,support_beam_length, " +
						"support_beam_width,support_beam_interval,concrete_ratio_cement,concrete_ratio_sand,concrete_ratio_aggregate, " +
						"plywood_length,plywood_width,riser_height,thread_depth,wastage,provisions) VALUES( " +
						"@floor_thickness,@wall_thickness,@rebar_thickness,@nail_interval,@hollow_block_constant,@support_beam_length, " +
						"@support_beam_width,@support_beam_interval,@concrete_ratio_cement,@concrete_ratio_sand,@concrete_ratio_aggregate, " +
						"@plywood_length,@plywood_width,@riser_height,@thread_depth,@wastage,@provisions); " +
						"SELECT SCOPE_IDENTITY() FROM template_formula_constants;"))
					{
						command.Parameters.AddWithValue("@floor_thickness",				model.floorThickness		);
						command.Parameters.AddWithValue("@wall_thickness",				model.wallThickness			);
						command.Parameters.AddWithValue("@rebar_thickness",				model.rebarDiameter			);
						command.Parameters.AddWithValue("@nail_interval",				model.nailConstant			);
						command.Parameters.AddWithValue("@hollow_block_constant",		model.hollowBlockConstant	);
						command.Parameters.AddWithValue("@support_beam_length",			model.supportBeamLength		);
						command.Parameters.AddWithValue("@support_beam_width",			model.supportBeamWidth		);
						command.Parameters.AddWithValue("@support_beam_interval",		model.supportBeamSpace		);
						command.Parameters.AddWithValue("@concrete_ratio_cement",		model.concreteRatioCement	);
						command.Parameters.AddWithValue("@concrete_ratio_sand",			model.concreteRatioSand		);
						command.Parameters.AddWithValue("@concrete_ratio_aggregate",	model.concreteRatioAggregate);
						command.Parameters.AddWithValue("@plywood_length",				model.plywoodLength			);
						command.Parameters.AddWithValue("@plywood_width",				model.plywoodWidth			);
						command.Parameters.AddWithValue("@riser_height",				model.riserHeight			);
						command.Parameters.AddWithValue("@thread_depth",				model.threadDepth			);
						command.Parameters.AddWithValue("@wastage",						model.wasteage				);
						command.Parameters.AddWithValue("@provisions",					model.provisions			);
						command.Connection = conn;
						command.Transaction = transaction;
						id = Convert.ToInt32(command.ExecuteScalar());
					}
					using (SqlCommand command = new SqlCommand("INSERT INTO employee_templates " +
						"(employee_id, formula_constants_id, template_description, template_descritpion_long) VALUES ( " +
						"@employee_id, @formula_constants_id, @template_description, @template_descritpion_long);"))
					{
						command.Parameters.AddWithValue("@employee_id", (int)HttpContext.Session.GetInt32("EmployeeID"));
						command.Parameters.AddWithValue("@formula_constants_id", id);
						command.Parameters.AddWithValue("@template_description", model.Descritpion);
						command.Parameters.AddWithValue("@template_descritpion_long", model.Long_Description);
						command.Connection = conn;
						command.Transaction = transaction;
						command.ExecuteNonQuery();
					}
					transaction.Commit();
				}
				
			}
			return RedirectToAction("Templates");
		}

		public IActionResult TemplatesEdit(int? id)
		{
			EditTemplateModel model = new EditTemplateModel();
			model.ID = (int)id;
			using (SqlConnection conn = new SqlConnection(connectionstring))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("SELECT * FROM employee_templates a " +
					"INNER JOIN template_formula_constants b ON a.formula_constants_id = b.formula_constants_id " +
					"WHERE template_id = @template_id;"))
				{
					command.Parameters.AddWithValue("@template_id", (int)id);
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							model = new EditTemplateModel()
							{
								FormulaID = Convert.ToInt32(sdr["formula_constants_id"]),
								Descritpion = sdr["template_description"].ToString(),
								Long_Description = sdr["template_descritpion_long"].ToString(),
								floorThickness = Convert.ToDouble(sdr["floor_thickness"]),
								wallThickness = Convert.ToDouble(sdr["wall_thickness"]),
								rebarDiameter = Convert.ToDouble(sdr["rebar_thickness"]),
								nailConstant = Convert.ToDouble(sdr["nail_interval"]),
								hollowBlockConstant = Convert.ToDouble(sdr["hollow_block_constant"]),
								supportBeamLength = Convert.ToDouble(sdr["support_beam_length"]),
								supportBeamWidth = Convert.ToDouble(sdr["support_beam_width"]),
								supportBeamSpace = Convert.ToDouble(sdr["support_beam_interval"]),
								concreteRatioCement = Convert.ToDouble(sdr["concrete_ratio_cement"]),
								concreteRatioSand = Convert.ToDouble(sdr["concrete_ratio_sand"]),
								concreteRatioAggregate = Convert.ToDouble(sdr["concrete_ratio_aggregate"]),
								plywoodLength = Convert.ToDouble(sdr["plywood_length"]),
								plywoodWidth = Convert.ToDouble(sdr["plywood_width"]),
								riserHeight = Convert.ToDouble(sdr["riser_height"]),
								threadDepth = Convert.ToDouble(sdr["thread_depth"]),
								wasteage = Convert.ToDouble(sdr["wastage"]),
								provisions = Convert.ToDouble(sdr["provisions"])
							};
						}
					}
				}
			}

			return View(model);
		}


		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public IActionResult TemplatesEdit(EditTemplateModel model)
		{

			using (SqlConnection conn = new SqlConnection(connectionstring))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("UPDATE employee_templates SET " +
					"template_description = @template_description, template_descritpion_long = @template_descritpion_long " +
					"WHERE template_id = @template_id;" +
					"UPDATE template_formula_constants SET " +
					"floor_thickness = @floor_thickness, wall_thickness = @wall_thickness, rebar_thickness = @rebar_thickness," +
					"nail_interval = @nail_interval, hollow_block_constant = @hollow_block_constant, support_beam_length = @support_beam_length, " +
					"support_beam_width = @support_beam_width, support_beam_interval = @support_beam_interval, concrete_ratio_cement = @concrete_ratio_cement," +
					"concrete_ratio_sand = @concrete_ratio_sand, concrete_ratio_aggregate = @concrete_ratio_aggregate, plywood_length = @plywood_length, " +
					"plywood_width = @plywood_width, riser_height = @riser_height, thread_depth = @thread_depth, wastage = @wastage, provisions = @provisions " +
					"WHERE formula_constants_id = @formula_constants_id;"))
				{
					command.Connection = conn;

					command.Parameters.AddWithValue("@template_id", model.ID);
					command.Parameters.AddWithValue("@floor_thickness", model.floorThickness);
					command.Parameters.AddWithValue("@wall_thickness", model.wallThickness);
					command.Parameters.AddWithValue("@rebar_thickness", model.rebarDiameter);
					command.Parameters.AddWithValue("@nail_interval", model.nailConstant);
					command.Parameters.AddWithValue("@hollow_block_constant", model.hollowBlockConstant);
					command.Parameters.AddWithValue("@support_beam_length", model.supportBeamLength);
					command.Parameters.AddWithValue("@support_beam_width", model.supportBeamWidth);
					command.Parameters.AddWithValue("@support_beam_interval", model.supportBeamSpace);
					command.Parameters.AddWithValue("@concrete_ratio_cement", model.concreteRatioCement);
					command.Parameters.AddWithValue("@concrete_ratio_sand", model.concreteRatioSand);
					command.Parameters.AddWithValue("@concrete_ratio_aggregate", model.concreteRatioAggregate);
					command.Parameters.AddWithValue("@plywood_length", model.plywoodLength);
					command.Parameters.AddWithValue("@plywood_width", model.plywoodWidth);
					command.Parameters.AddWithValue("@riser_height", model.riserHeight);
					command.Parameters.AddWithValue("@thread_depth", model.threadDepth);
					command.Parameters.AddWithValue("@wastage", model.wasteage);
					command.Parameters.AddWithValue("@provisions", model.provisions);
					command.Parameters.AddWithValue("@formula_constants_id", model.FormulaID);
					command.Parameters.AddWithValue("@template_description", model.Descritpion);
					command.Parameters.AddWithValue("@template_descritpion_long", model.Long_Description);
					command.ExecuteNonQuery();
				}
			}

			return RedirectToAction("Templates");
		}


		public IActionResult BOMGenerate(int? id)
		{
			EmployeeBOMModel model = new EmployeeBOMModel();
			model.templates = new List<Employee_BOM_Template_List>();
			model.ProjectID = (int)id;
			string location = model.Latitude + "," + model.Longtitude;


			using (SqlConnection conn = new SqlConnection(connectionstring))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("SELECT * FROM projects a " +
					"INNER JOIN employee_info b ON a.project_engineer_id = b.employee_info_id " +
					"INNER JOIN building_types c ON a.building_types_id = c.building_types_id " +
					"WHERE a.project_id = @project_id"))
				{
					command.Parameters.AddWithValue("@project_id", id);
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.Title = sdr["project_title"].ToString();
							model.ClientName = sdr["project_client_name"].ToString();
							model.ClientContact = sdr["project_client_contact"].ToString();
							model.Address = sdr["project_address"].ToString();
							model.City = sdr["project_city"].ToString();
							model.Region = sdr["project_region"].ToString();
							model.Country = sdr["project_country"].ToString();
							model.Date = DateTime.Parse(sdr["project_date"].ToString());
							model.BuildingType = sdr["description"].ToString();
							model.NumberOfStoreys = Convert.ToInt32(sdr["project_building_storeys"]);
							model.FloorHeight = Convert.ToDouble(sdr["project_building_floorheight"]);
							model.BuildingLength = Convert.ToDouble(sdr["project_building_length"]);
							model.BuildingWidth = Convert.ToDouble(sdr["project_building_width"]);
							model.Longtitude = sdr["project_longtitude"].ToString();
							model.Latitude = sdr["project_latitude"].ToString();
						}
					}
				}
				using (SqlCommand command = new SqlCommand("SELECT * FROM employee_templates WHERE employee_id = @employee_id;"))
				{
					command.Parameters.AddWithValue("@employee_id", HttpContext.Session.GetInt32("EmployeeID"));
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.templates.Add(new Employee_BOM_Template_List()
							{
								ID = sdr["template_id"].ToString(),
								Description = sdr["template_description"].ToString()
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
		public async Task<IActionResult> BOMGenerate(EmployeeBOMModel model)
		{

			string location = model.Latitude + "," + model.Longtitude;
			Debug.WriteLine("Lat: " + model.Latitude);
			Debug.WriteLine("Lon: " + model.Longtitude);
			Debug.WriteLine("Loc: " + location);

			int noOfStories = model.NumberOfStoreys;
			 double heightOfFloors = model.FloorHeight,
								lengthOfBuilding = model.BuildingLength,
								widthOfBuilding = model.BuildingWidth,
								sqmOfBuilding = heightOfFloors * lengthOfBuilding,
								π = 3.14159

				;
			double floorThickness = 0,
								wallThickness = 0,
			/*new*/             rebarPercentage = 0,
			/*new*/             rebarDiameter = 0,
								nailConstant = 0,
								hollowBlockLength = 0,
								hollowBlockWidth = 0,
								hollowBlockHeight = 0,
								hollowBlockVolume = 0,
								hollowBlockConstant = 0,
								supportBeamLength = 0,
								supportBeamWidth = 0,
								supportBeamArea = 0,
								supportBeamSpace = 0,
								supportBeamVolume = 0,
								supportBeamsNeeded = 0,
								concreteFormulaCement = 0,
								concreteFormulaSand = 0,
								concreteFormulaAggregate = 0,
								plywoodLength = 0,
								plywoodWidth = 0,
								plywoodArea = 0,
								plywoodSheetsPerSqm = 0,
								riserHeight = 0,
								threadDepth = 0,
								stairWidth = 0,
								numberOfSteps = 0,
				 /*new*/        wastage = 0,
				 /*new*/        provisions = 0;

			using (SqlConnection conn = new SqlConnection(connectionstring))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("SELECT * FROM employee_templates a " +
					"INNER JOIN template_formula_constants b ON a.formula_constants_id = b.formula_constants_id " +
					"WHERE template_id = @template_id;"))
				{
					command.Parameters.AddWithValue("@template_id", Convert.ToInt32(model.TemplateID));
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.FormulaID = Convert.ToInt32(sdr["formula_constants_id"]);
							// measurements that are changeable
							floorThickness = Convert.ToDouble(sdr["floor_thickness"]);
							wallThickness = Convert.ToDouble(sdr["wall_thickness"]);
							/*new*/
							rebarPercentage = 0.1; //percentage of reinforcement standard 0.08 ~ 0.12
							/*new*/
							rebarDiameter = Convert.ToDouble(sdr["rebar_thickness"]);
							nailConstant = Convert.ToDouble(sdr["nail_interval"]);
							hollowBlockLength = 0.2;
							hollowBlockWidth = 0.2;
							hollowBlockHeight = 0.2;
							hollowBlockVolume = hollowBlockHeight * hollowBlockLength * hollowBlockWidth;
							hollowBlockConstant = Convert.ToDouble(sdr["hollow_block_constant"]);
							supportBeamLength = Convert.ToDouble(sdr["support_beam_length"]);
							supportBeamWidth = Convert.ToDouble(sdr["support_beam_width"]);
							supportBeamArea = supportBeamLength * supportBeamWidth;
							supportBeamSpace = Convert.ToDouble(sdr["support_beam_interval"]);
							supportBeamVolume = supportBeamArea * heightOfFloors;
							supportBeamsNeeded = sqmOfBuilding / supportBeamSpace;
							concreteFormulaCement = Convert.ToDouble(sdr["concrete_ratio_cement"]);
							concreteFormulaSand = Convert.ToDouble(sdr["concrete_ratio_sand"]);
							concreteFormulaAggregate = Convert.ToDouble(sdr["concrete_ratio_aggregate"]);
								plywoodLength = Convert.ToDouble(sdr["plywood_length"]);
							plywoodWidth = Convert.ToDouble(sdr["plywood_width"]);
							plywoodArea = plywoodLength * plywoodWidth;
								plywoodSheetsPerSqm = (double)Math.Ceiling(10764 / plywoodArea);
							riserHeight = Convert.ToDouble(sdr["riser_height"]);
								threadDepth = Convert.ToDouble(sdr["thread_depth"]);
							stairWidth = 0.91;
								numberOfSteps = (double)Math.Ceiling(heightOfFloors / riserHeight);
							/*new*/
							wastage = Convert.ToDouble(sdr["wastage"]);
							/*new*/
							provisions = Convert.ToDouble(sdr["provisions"]);

				;
						}
					}
				}
			}
			// prices, are changeable
			MaterialsCostComparisonItem
				Cost_rebar = GetBestPrice(5, location),
				Cost_Hollowblock = GetBestPrice(6, location),
				Cost_Cement = GetBestPrice(2, location),
				Cost_Sand = GetBestPrice(3, location),
				Cost_Aggregate = GetBestPrice(4, location),
				Cost_Plywood = GetBestPrice(1, location);
			double rebarPrice = Cost_rebar.Price, // per piece. Rebars * rebarPrice
								hollowBlockPrice = Cost_Hollowblock.Price, // per piece. NoOfHollowBlock * hollowBlockPrice
								cementPrice = Cost_Cement.Price, // per cubic meter. CementCubicMeters * cementPrice,
								sandPrice = Cost_Sand.Price, // per cubic meter
								sandReducedPrice = (1 / 3) * sandPrice,
								sandBigPrice = 2 * sandPrice,
								aggregatePrice = Cost_Aggregate.Price, // per cubic meter
								aggregateReducedPrice = (1 / 2) * aggregatePrice,
								aggregateBigPrice = 3 * aggregatePrice,
								concretePrice = cementPrice + sandPrice + aggregatePrice,
								concreteReducedPrice = cementPrice + sandReducedPrice + aggregateReducedPrice,
								concreteBigPrice = cementPrice + sandBigPrice + aggregateBigPrice,
								plywoodPrice = Cost_Plywood.Price, // per piece 1/4  
								plywoodPricePerSqm = plywoodPrice * plywoodSheetsPerSqm

				;
			int RebarCostID = Cost_rebar.SupplierMaterialID,
				HollowBlockCostID = Cost_Hollowblock.SupplierMaterialID,
				CementCostID = Cost_Cement.SupplierMaterialID,
				SandCostID = Cost_Sand.SupplierMaterialID,
				AggregateCostID = Cost_Aggregate.SupplierMaterialID,
				PlywoodCostID = Cost_Plywood.SupplierMaterialID;



			double concreteTotalRatio = concreteFormulaCement + concreteFormulaSand + concreteFormulaAggregate;
			double cementRatio = concreteFormulaCement / concreteTotalRatio;
			double sandRatio = concreteFormulaSand / concreteTotalRatio;
			double aggregateRatio = concreteFormulaAggregate / concreteTotalRatio;

			// foundation
			double foundationHeight = heightOfFloors * noOfStories + (noOfStories * floorThickness),
								foundationVolume = foundationHeight * sqmOfBuilding,
								foundationPerimeter = 2 * (lengthOfBuilding + widthOfBuilding),
								foundationWallArea = 4 * foundationPerimeter * foundationHeight,
								foundationNoOfHollowBlock = foundationWallArea * hollowBlockConstant,
			/*new*/             foundationRebar = (lengthOfBuilding * widthOfBuilding * rebarPercentage) * foundationHeight,
								foundationConcrete = foundationVolume
				;
			model.lists = new List<Employee_BOM_Materials_Lists>();
			model.lists.Add(new Employee_BOM_Materials_Lists()
			{
				Description = $"Foundation",
				ListNumber = 1,
				Items = new List<Employee_BOM_Materials_Items>()
			});
			model.lists[0].Items.Add(new Employee_BOM_Materials_Items()
			{
				Description = $"Foundation",
				Subitems = new List<Employee_BOM_Materials_Subitems>()
			});
			int foundationConcreteCement = (int)Math.Ceiling(foundationConcrete * cementRatio);
			model.lists[0].Items[0].Subitems.Add(GetMaterial(2, foundationConcreteCement, location, 1, cementPrice, wastage, provisions, CementCostID));

			int foundationConcreteSand = (int)Math.Ceiling(foundationConcrete * sandRatio);
			model.lists[0].Items[0].Subitems.Add(GetMaterial(3, foundationConcreteSand, location, 2, sandPrice, wastage, provisions, SandCostID));

			int foundationConcreteAggregate = (int)Math.Ceiling(foundationConcrete * aggregateRatio);
			model.lists[0].Items[0].Subitems.Add(GetMaterial(4, foundationConcreteAggregate, location, 3, aggregatePrice, wastage, provisions, AggregateCostID));

			int foundationRebarAmount = (int)Math.Ceiling(foundationRebar);
			model.lists[0].Items[0].Subitems.Add(GetMaterial(5, foundationRebarAmount, location, 4, rebarPrice, wastage, provisions, RebarCostID));

			int foundationHollowBlock = (int)Math.Ceiling(foundationNoOfHollowBlock);
			model.lists[0].Items[0].Subitems.Add(GetMaterial(6, foundationHollowBlock, location, 5, hollowBlockPrice, wastage, provisions, HollowBlockCostID));

			for (int i = 1; i <= model.NumberOfStoreys; i++)
			{
				model.lists.Add(new Employee_BOM_Materials_Lists()
				{
					Description = $"Storey {i}",
					ListNumber = i + 1,
					Items = new List<Employee_BOM_Materials_Items>()
				});
				model.lists[i].Items.Add(new Employee_BOM_Materials_Items()
				{
					Description = $"Storey {i} floor",
					Subitems = new List<Employee_BOM_Materials_Subitems>()
				});
				model.lists[i].Items.Add(new Employee_BOM_Materials_Items()
				{
					Description = $"Storey {i} walls",
					Subitems = new List<Employee_BOM_Materials_Subitems>()
				});
				model.lists[i].Items.Add(new Employee_BOM_Materials_Items()
				{
					Description = $"Storey {i} support beams",
					Subitems = new List<Employee_BOM_Materials_Subitems>()
				});
				model.lists[i].Items.Add(new Employee_BOM_Materials_Items()
				{
					Description = $"Storey {i} stairs",
					Subitems = new List<Employee_BOM_Materials_Subitems>()
				});
				// stories. repeat for every storey the building has
				double storeyHeight = heightOfFloors + floorThickness,
								storeyPerimeter = 2 * (lengthOfBuilding + widthOfBuilding),
								storeyWallVolume = 4 * storeyPerimeter * storeyHeight,
								storeyFloorVolume = sqmOfBuilding * floorThickness,
								// floors
								storeyFloorPlywood = plywoodSheetsPerSqm * sqmOfBuilding,
								storeyFloorNails = storeyFloorPlywood * nailConstant,
								storeyFloorConcrete = storeyFloorVolume,
			 /*new*/            storeyFloorRebar = (lengthOfBuilding * widthOfBuilding * rebarPercentage) * floorThickness
				,


								// support beams
								storeySupportBeamsNeeded = supportBeamsNeeded,
								storeySupportBeamsConcrete = supportBeamVolume,
			 /*new*/            storeySupportBeamsRebar = (supportBeamLength * supportBeamWidth * rebarPercentage) * heightOfFloors,

								// walls
								storeyWallConcrete = storeyWallVolume,
			  /*new*/           storeyWallRebar = ((lengthOfBuilding * wallThickness * rebarPercentage) * heightOfFloors) * 4
				,

								// stairs
								stairsVolume = numberOfSteps * (riserHeight * threadDepth * stairWidth),
								stairsConcrete = stairsVolume,
			   /*new*/          stairsRebar = ((stairWidth * threadDepth * rebarPercentage) * riserHeight) * numberOfSteps


				;

				int floorConcreteCement = (int)Math.Ceiling(storeyFloorConcrete * cementRatio);
				model.lists[i].Items[0].Subitems.Add(GetMaterial(2, floorConcreteCement, location, 1, cementPrice, wastage, provisions, CementCostID));

				int floorConcreteSand = (int)Math.Ceiling(storeyFloorConcrete * sandRatio);
				model.lists[i].Items[0].Subitems.Add(GetMaterial(3, floorConcreteSand, location, 2, sandPrice, wastage, provisions, SandCostID));

				int floorConcreteAggregate = (int)Math.Ceiling(storeyFloorConcrete * aggregateRatio);
				model.lists[i].Items[0].Subitems.Add(GetMaterial(4, floorConcreteAggregate, location, 3, aggregatePrice, wastage, provisions,AggregateCostID));

				int floorRebarAmount = (int)Math.Ceiling(storeyFloorRebar);
				model.lists[i].Items[0].Subitems.Add(GetMaterial(5, floorRebarAmount, location, 4, rebarPrice, wastage, provisions, RebarCostID));


				//wall
				int wallConcreteCement = (int)Math.Ceiling(storeyWallConcrete * cementRatio);
				model.lists[i].Items[1].Subitems.Add(GetMaterial(2, wallConcreteCement, location, 1, cementPrice, wastage, provisions, CementCostID));
				int wallConcreteSand = (int)Math.Ceiling(storeyWallConcrete * sandRatio);
				model.lists[i].Items[1].Subitems.Add(GetMaterial(3, wallConcreteSand, location, 2, sandPrice, wastage, provisions, SandCostID));
				int wallConcreteAggregate = (int)Math.Ceiling(storeyWallConcrete * aggregateRatio);
				model.lists[i].Items[1].Subitems.Add(GetMaterial(4, wallConcreteAggregate, location, 3, aggregatePrice, wastage, provisions, AggregateCostID));

				int wallRebarAmount = (int)Math.Ceiling(storeyWallRebar);
				model.lists[i].Items[1].Subitems.Add(GetMaterial(5, wallRebarAmount, location, 4, rebarPrice, wastage, provisions, RebarCostID));

				//beam
				int beamConcreteCement = (int)Math.Ceiling(storeySupportBeamsConcrete * cementRatio);
				model.lists[i].Items[2].Subitems.Add(GetMaterial(2, beamConcreteCement, location, 1, cementPrice, wastage, provisions, CementCostID));
				int beamConcreteSand = (int)Math.Ceiling(storeySupportBeamsConcrete * sandRatio);
				model.lists[i].Items[2].Subitems.Add(GetMaterial(3, beamConcreteSand, location, 2, sandPrice, wastage, provisions, SandCostID));
				int beamConcreteAggregate = (int)Math.Ceiling(storeySupportBeamsConcrete * aggregateRatio);
				model.lists[i].Items[2].Subitems.Add(GetMaterial(4, beamConcreteAggregate, location, 3, aggregatePrice, wastage, provisions, AggregateCostID));

				int beamRebarAmount = (int)Math.Ceiling(storeySupportBeamsRebar);
				model.lists[i].Items[2].Subitems.Add(GetMaterial(5, beamRebarAmount, location, 4, rebarPrice, wastage, provisions, RebarCostID));

				//stair
				int stairsConcreteCement = (int)Math.Ceiling(stairsConcrete * cementRatio);
				model.lists[i].Items[3].Subitems.Add(GetMaterial(2, stairsConcreteCement, location, 1, cementPrice, wastage, provisions, CementCostID));
				int stairsConcreteSand = (int)Math.Ceiling(stairsConcrete * sandRatio);
				model.lists[i].Items[3].Subitems.Add(GetMaterial(3, stairsConcreteSand, location, 2, sandPrice, wastage, provisions, SandCostID));
				int stairsConcreteAggregate = (int)Math.Ceiling(stairsConcrete * aggregateRatio);
				model.lists[i].Items[3].Subitems.Add(GetMaterial(4, stairsConcreteAggregate, location, 3, aggregatePrice, wastage, provisions, AggregateCostID));

				int stairsRebarAmount = (int)Math.Ceiling(stairsRebar);
				model.lists[i].Items[3].Subitems.Add(GetMaterial(5, stairsRebarAmount, location, 4, rebarPrice, wastage, provisions, RebarCostID));
			}

			model.totalCost = 0;
			foreach (Employee_BOM_Materials_Lists x in model.lists)
			{
				foreach (Employee_BOM_Materials_Items y in x.Items)
				{
					foreach (Employee_BOM_Materials_Subitems z in y.Subitems)
					{
						model.totalCost += (z.MaterialCost * Math.Ceiling((double)z.MaterialQuantity));
					}
				}
			}

			TempData["BOMGenerateData"] = JsonConvert.SerializeObject(model);
			return RedirectToAction("BOMAdd");
		}


		public IActionResult BOMAdd()
		{
			EmployeeBOMModel model = JsonConvert.DeserializeObject<EmployeeBOMModel>(TempData["BOMGenerateData"].ToString());
			TempData["BOMGenerateData"] = JsonConvert.SerializeObject(model);
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> BOMAdd(EmployeeBOMModel model)
		{
			TempData["BOMGenerateData"] = JsonConvert.SerializeObject(model);
			Debug.WriteLine("boolet");
			using (SqlConnection conn = new SqlConnection(connectionstring))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					int bom_id = 0;
					using (SqlCommand command = new SqlCommand("INSERT INTO bom (bom_creation_date, project_id, bom_formula_id) VALUES ( " +
						"@bom_creation_date, @project_id, @bom_formula_id);" +
						" SELECT SCOPE_IDENTITY() FROM bom;"))
					{
						command.Parameters.AddWithValue("@bom_creation_date", DateTime.Today.ToString("yyyy-MM-dd"));
						command.Parameters.AddWithValue("@project_id", (int)model.ProjectID);
						command.Parameters.AddWithValue("@bom_formula_id", (int)model.FormulaID);
						command.Connection = conn;
						command.Transaction = trans;
						bom_id = Convert.ToInt32(command.ExecuteScalar());
					}

					foreach (Employee_BOM_Materials_Lists list in model.lists)
					{
						SqlCommand command2 = new SqlCommand(
							"INSERT INTO bom_lists (bom_list_desc,bom_id) VALUES " +
							"(@list_desc," +
							"@bom_id);" +
							"SELECT SCOPE_IDENTITY() FROM bom_lists;");

						command2.Parameters.AddWithValue("@list_desc", list.Description);
						command2.Parameters.AddWithValue("@bom_id", bom_id);

						command2.Connection = conn;
						command2.Transaction = trans;
						int bom_list_id = Convert.ToInt32(command2.ExecuteScalar());
						command2.Dispose();
						Debug.WriteLine("LIST");

						foreach (Employee_BOM_Materials_Items item in list.Items)
						{
							SqlCommand command3 = new SqlCommand(
							"INSERT INTO bom_items (bom_list_id,bom_list_desc) VALUES " +
							"(@bom_list_id," +
							"@bom_list_desc);" +
							"SELECT SCOPE_IDENTITY() FROM bom_items;");

							command3.Parameters.AddWithValue("@bom_list_id", bom_list_id);
							command3.Parameters.AddWithValue("@bom_list_desc", item.Description);

							command3.Connection = conn;
							command3.Transaction = trans;
							int bom_list_item_id = Convert.ToInt32(command3.ExecuteScalar());
							command3.Dispose();
							Debug.WriteLine("ITEM");

							foreach (Employee_BOM_Materials_Subitems subitem in item.Subitems)
							{
								SqlCommand command4 = new SqlCommand(
									"INSERT INTO bom_subitems (material_id,bom_subitem_quantity,bom_item_id, supplier_material_id) VALUES " +
									"(@item_id," +
									"@item_quantity," +
									"@bom_item_id, " +
									"@supplier_material_id);");

								command4.Parameters.AddWithValue("@item_id", subitem.MaterialID);
								command4.Parameters.AddWithValue("@item_quantity", subitem.MaterialQuantity);
								command4.Parameters.AddWithValue("@bom_item_id", bom_list_item_id);
								command4.Parameters.AddWithValue("@supplier_material_id", subitem.SupplierMaterialID);

								command4.Connection = conn;
								command4.Transaction = trans;
								command4.ExecuteNonQuery();
								command4.Dispose();
								Debug.WriteLine("SUBITEM");
							}
						}
					}
					trans.Commit();
					Debug.WriteLine("Saved?");
				}
			}

			return View(model);
		}

		public IActionResult BOMView(int? id)
		{
			EmployeeBOMModel model = new EmployeeBOMModel();
			model.templates = new List<Employee_BOM_Template_List>();
			using (SqlConnection conn = new SqlConnection(connectionstring))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("SELECT * FROM bom a INNER JOIN projects b ON a.project_id = b.project_id " +
					"INNER JOIN template_formula_constants c ON a.bom_formula_id = formula_constants_id " +
					"INNER JOIN building_types d ON b.building_types_id = d.building_types_id " +
					"WHERE a.bom_id = @id;"))
				{
					
					command.Parameters.AddWithValue("@id", (int)id);
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.Title = sdr["project_title"].ToString();
							model.ClientName = sdr["project_client_name"].ToString();
							model.ClientContact = sdr["project_client_contact"].ToString();
							model.Address = sdr["project_address"].ToString();
							model.City = sdr["project_city"].ToString();
							model.Region = sdr["project_region"].ToString();
							model.Country = sdr["project_country"].ToString();
							model.Date = DateTime.Parse(sdr["project_date"].ToString());
							model.BuildingType = sdr["description"].ToString();
							model.NumberOfStoreys = Convert.ToInt32(sdr["project_building_storeys"]);
							model.FloorHeight = Convert.ToDouble(sdr["project_building_floorheight"]);
							model.BuildingLength = Convert.ToDouble(sdr["project_building_length"]);
							model.BuildingWidth = Convert.ToDouble(sdr["project_building_width"]);
							model.Longtitude = sdr["project_longtitude"].ToString();
							model.Latitude = sdr["project_latitude"].ToString();
							model.FormulaID = Convert.ToInt32(sdr["bom_formula_id"]);
							model.BOMCreationDate = DateTime.Parse(sdr["bom_creation_date"].ToString());
							model.Wastage = Convert.ToDouble(sdr["wastage"]);
							model.Provisions = Convert.ToDouble(sdr["provisions"]);
						}
					}
				}
				model.lists = new List<Employee_BOM_Materials_Lists>();
				using (SqlCommand command = new SqlCommand("SELECT * FROM bom_lists WHERE bom_id = @id;"))
				{
					command.Parameters.AddWithValue("@id", (int)id);
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						int num = 1;
						while (sdr.Read())
						{
							model.lists.Add(new Employee_BOM_Materials_Lists()
							{
								Description = sdr["bom_list_desc"].ToString(),
								ListID = Convert.ToInt32(sdr["bom_list_id"]),
								ListNumber = num,
								Items = new List<Employee_BOM_Materials_Items>()
							});
							num++;
						}
					}
				}
				for (int x = 0; x < model.lists.Count; x++)
				{
					using (SqlCommand command = new SqlCommand("SELECT * FROM bom_items WHERE bom_list_id = @id;"))
					{
						command.Parameters.AddWithValue("@id", (int)model.lists[x].ListID);
						command.Connection = conn;
						using (SqlDataReader sdr = command.ExecuteReader())
						{
							int num = 1;
							while (sdr.Read())
							{
								model.lists[x].Items.Add(new Employee_BOM_Materials_Items()
								{
									Description = sdr["bom_list_desc"].ToString(),
									ItemID = Convert.ToInt32(sdr["bom_item_id"]),
									ItemNumber = num,
									Subitems = new List<Employee_BOM_Materials_Subitems>()
								});
								num++;
							}
						}
					}
					for (int y = 0; y < model.lists[x].Items.Count; y++)
					{
						using (SqlCommand command = new SqlCommand("SELECT * FROM bom_subitems a " +
							"INNER JOIN materials b ON a.material_id = b.material_id " +
							"INNER JOIN supplier_materials c ON a.supplier_material_id = c.supplier_material_id " +
							"INNER JOIN measurement_units d ON b.measurement_unit_id = d.measurement_unit_id " +
							"INNER JOIN supplier_info e ON e.supplier_id = c.supplier_id " +
							"WHERE bom_item_id = @id;"))
						{
							command.Parameters.AddWithValue("@id", (int)model.lists[x].Items[y].ItemID);
							command.Connection = conn;
							using (SqlDataReader sdr = command.ExecuteReader())
							{
								int num = 1;
								while (sdr.Read())
								{
									model.lists[x].Items[y].Subitems.Add(new Employee_BOM_Materials_Subitems()
									{
										SubitemNumber = num,
										MaterialID = Convert.ToInt32(sdr["material_id"]),
										MaterialDesc = sdr["material_desc"].ToString(),
										MaterialUoM = sdr["measurement_unit_desc_short"].ToString(),
										MaterialQuantity = Convert.ToInt32(sdr["bom_subitem_quantity"]),
										MaterialQuantityWastage = (int)Math.Ceiling(Convert.ToInt32(sdr["bom_subitem_quantity"]) * model.Wastage),
										MaterialQuantityProvisions = (int)Math.Ceiling(Math.Ceiling(Convert.ToInt32(sdr["bom_subitem_quantity"]) * model.Wastage) * model.Provisions),
										MaterialCost = Convert.ToDouble(sdr["supplier_material_price"]) / 100,
										MaterialAmount = Math.Round((int)Math.Ceiling(Math.Ceiling(Convert.ToInt32(sdr["bom_subitem_quantity"]) * model.Wastage) * model.Provisions) * (Convert.ToDouble(sdr["supplier_material_price"]) / 100), 2),
										SupplierMaterialID = Convert.ToInt32(sdr["supplier_material_id"]),
										Supplier = sdr["supplier_desc"].ToString()
									});
									num++;
								}
							}
						}
					}
				}
			}

			model.totalCost = 0;
			foreach (Employee_BOM_Materials_Lists x in model.lists)
			{
				foreach (Employee_BOM_Materials_Items y in x.Items)
				{
					foreach (Employee_BOM_Materials_Subitems z in y.Subitems)
					{
						model.totalCost += (z.MaterialCost * Math.Ceiling((double)z.MaterialQuantity));
					}
				}
			}
			return View(model);
		}

		public IActionResult BOMViewClient(int? id)
		{
			EmployeeBOMModel model = new EmployeeBOMModel();
			model.templates = new List<Employee_BOM_Template_List>();
			using (SqlConnection conn = new SqlConnection(connectionstring))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("SELECT * FROM bom a INNER JOIN projects b ON a.project_id = b.project_id " +
					"INNER JOIN template_formula_constants c ON a.bom_formula_id = formula_constants_id " +
					"INNER JOIN building_types d ON b.building_types_id = d.building_types_id " +
					"WHERE a.bom_id = @id;"))
				{

					command.Parameters.AddWithValue("@id", (int)id);
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.Title = sdr["project_title"].ToString();
							model.ClientName = sdr["project_client_name"].ToString();
							model.ClientContact = sdr["project_client_contact"].ToString();
							model.Address = sdr["project_address"].ToString();
							model.City = sdr["project_city"].ToString();
							model.Region = sdr["project_region"].ToString();
							model.Country = sdr["project_country"].ToString();
							model.Date = DateTime.Parse(sdr["project_date"].ToString());
							model.BuildingType = sdr["description"].ToString();
							model.NumberOfStoreys = Convert.ToInt32(sdr["project_building_storeys"]);
							model.FloorHeight = Convert.ToDouble(sdr["project_building_floorheight"]);
							model.BuildingLength = Convert.ToDouble(sdr["project_building_length"]);
							model.BuildingWidth = Convert.ToDouble(sdr["project_building_width"]);
							model.Longtitude = sdr["project_longtitude"].ToString();
							model.Latitude = sdr["project_latitude"].ToString();
							model.FormulaID = Convert.ToInt32(sdr["bom_formula_id"]);
							model.BOMCreationDate = DateTime.Parse(sdr["bom_creation_date"].ToString());
							model.Wastage = Convert.ToDouble(sdr["wastage"]);
							model.Provisions = Convert.ToDouble(sdr["provisions"]);
						}
					}
				}
				model.lists = new List<Employee_BOM_Materials_Lists>();
				using (SqlCommand command = new SqlCommand("SELECT * FROM bom_lists WHERE bom_id = @id;"))
				{
					command.Parameters.AddWithValue("@id", (int)id);
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						int num = 1;
						while (sdr.Read())
						{
							model.lists.Add(new Employee_BOM_Materials_Lists()
							{
								Description = sdr["bom_list_desc"].ToString(),
								ListID = Convert.ToInt32(sdr["bom_list_id"]),
								ListNumber = num,
								Items = new List<Employee_BOM_Materials_Items>()
							});
							num++;
						}
					}
				}
				for (int x = 0; x < model.lists.Count; x++)
				{
					using (SqlCommand command = new SqlCommand("SELECT * FROM bom_items WHERE bom_list_id = @id;"))
					{
						command.Parameters.AddWithValue("@id", (int)model.lists[x].ListID);
						command.Connection = conn;
						using (SqlDataReader sdr = command.ExecuteReader())
						{
							int num = 1;
							while (sdr.Read())
							{
								model.lists[x].Items.Add(new Employee_BOM_Materials_Items()
								{
									Description = sdr["bom_list_desc"].ToString(),
									ItemID = Convert.ToInt32(sdr["bom_item_id"]),
									ItemNumber = num,
									Subitems = new List<Employee_BOM_Materials_Subitems>()
								});
								num++;
							}
						}
					}
					for (int y = 0; y < model.lists[x].Items.Count; y++)
					{
						using (SqlCommand command = new SqlCommand("SELECT * FROM bom_subitems a " +
							"INNER JOIN materials b ON a.material_id = b.material_id " +
							"INNER JOIN supplier_materials c ON a.supplier_material_id = c.supplier_material_id " +
							"INNER JOIN measurement_units d ON b.measurement_unit_id = d.measurement_unit_id " +
							"INNER JOIN supplier_info e ON e.supplier_id = c.supplier_id " +
							"WHERE bom_item_id = @id;"))
						{
							command.Parameters.AddWithValue("@id", (int)model.lists[x].Items[y].ItemID);
							command.Connection = conn;
							using (SqlDataReader sdr = command.ExecuteReader())
							{
								int num = 1;
								while (sdr.Read())
								{
									model.lists[x].Items[y].Subitems.Add(new Employee_BOM_Materials_Subitems()
									{
										SubitemNumber = num,
										MaterialID = Convert.ToInt32(sdr["material_id"]),
										MaterialDesc = sdr["material_desc"].ToString(),
										MaterialUoM = sdr["measurement_unit_desc_short"].ToString(),
										MaterialQuantity = Convert.ToInt32(sdr["bom_subitem_quantity"]),
										MaterialQuantityWastage = (int)Math.Ceiling(Convert.ToInt32(sdr["bom_subitem_quantity"]) * model.Wastage),
										MaterialQuantityProvisions = (int)Math.Ceiling(Math.Ceiling(Convert.ToInt32(sdr["bom_subitem_quantity"]) * model.Wastage) * model.Provisions),
										MaterialCost = Convert.ToDouble(sdr["supplier_material_price"]) / 100,
										MaterialAmount = Math.Round((int)Math.Ceiling(Math.Ceiling(Convert.ToInt32(sdr["bom_subitem_quantity"]) * model.Wastage) * model.Provisions) * (Convert.ToDouble(sdr["supplier_material_price"]) / 100), 2),
										SupplierMaterialID = Convert.ToInt32(sdr["supplier_material_id"]),
										Supplier = sdr["supplier_desc"].ToString()
									});
									num++;
								}
							}
						}
					}
				}
			}

			model.totalCost = 0;
			foreach (Employee_BOM_Materials_Lists x in model.lists)
			{
				foreach (Employee_BOM_Materials_Items y in x.Items)
				{
					foreach (Employee_BOM_Materials_Subitems z in y.Subitems)
					{
						model.totalCost += (z.MaterialCost * Math.Ceiling((double)z.MaterialQuantity));
					}
				}
			}
			return View(model);
		}

		public IActionResult BOMList()
		{
			List<Employee_BOM_List_Item> projects = new List<Employee_BOM_List_Item>();

			using (SqlConnection conn = new SqlConnection(connectionstring))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("SELECT *, c.mce_id IS NOT NULL AS HasReference FROM bom a " +
					"INNER JOIN projects b ON a.project_id = b.project_id " +
					"LEFT JOIN mce c ON a.bom_id = c.bom_id " +
					"WHERE b.project_engineer_id = @project_engineer_id;"))
				{
					command.Parameters.AddWithValue("@project_engineer_id", HttpContext.Session.GetInt32("EmployeeID"));
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							projects.Add(new Employee_BOM_List_Item()
							{
								ID = Convert.ToInt32(sdr["bom_id"]),
								Title = sdr["project_title"].ToString(),
								ClientName = sdr["project_client_name"].ToString(),
								Date = DateTime.Parse(sdr["bom_creation_date"].ToString()),
								MCEExists = Convert.ToBoolean(sdr["HasReference"])
							});
						}
					}
				}
			}
			return View(projects);
		}

		public IActionResult ToMCEAdd(int? id)
		{
			EmployeeBOMModel model = new EmployeeBOMModel();
			model.templates = new List<Employee_BOM_Template_List>();
			using (SqlConnection conn = new SqlConnection(connectionstring))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("SELECT * FROM bom a INNER JOIN projects b ON a.project_id = b.project_id " +
					"INNER JOIN template_formula_constants c ON a.bom_formula_id = formula_constants_id " +
					"INNER JOIN building_types d ON b.building_types_id = d.building_types_id " +
					"WHERE a.bom_id = @id;"))
				{

					command.Parameters.AddWithValue("@id", (int)id);
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							model.Title = sdr["project_title"].ToString();
							model.ClientName = sdr["project_client_name"].ToString();
							model.ClientContact = sdr["project_client_contact"].ToString();
							model.Address = sdr["project_address"].ToString();
							model.City = sdr["project_city"].ToString();
							model.Region = sdr["project_region"].ToString();
							model.Country = sdr["project_country"].ToString();
							model.Date = DateTime.Parse(sdr["project_date"].ToString());
							model.BuildingType = sdr["description"].ToString();
							model.NumberOfStoreys = Convert.ToInt32(sdr["project_building_storeys"]);
							model.FloorHeight = Convert.ToDouble(sdr["project_building_floorheight"]);
							model.BuildingLength = Convert.ToDouble(sdr["project_building_length"]);
							model.BuildingWidth = Convert.ToDouble(sdr["project_building_width"]);
							model.Longtitude = sdr["project_longtitude"].ToString();
							model.Latitude = sdr["project_latitude"].ToString();
							model.FormulaID = Convert.ToInt32(sdr["bom_formula_id"]);
							model.BOMCreationDate = DateTime.Parse(sdr["bom_creation_date"].ToString());
							model.Wastage = Convert.ToDouble(sdr["wastage"]);
							model.Provisions = Convert.ToDouble(sdr["provisions"]);
							model.ProjectID = Convert.ToInt32(sdr["project_id"]);
						}
					}
				}
				model.lists = new List<Employee_BOM_Materials_Lists>();
				using (SqlCommand command = new SqlCommand("SELECT * FROM bom_lists WHERE bom_id = @id;"))
				{
					command.Parameters.AddWithValue("@id", (int)id);
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						int num = 1;
						while (sdr.Read())
						{
							model.lists.Add(new Employee_BOM_Materials_Lists()
							{
								Description = sdr["bom_list_desc"].ToString(),
								ListID = Convert.ToInt32(sdr["bom_list_id"]),
								ListNumber = num,
								Items = new List<Employee_BOM_Materials_Items>()
							});
							num++;
						}
					}
				}
				for (int x = 0; x < model.lists.Count; x++)
				{
					using (SqlCommand command = new SqlCommand("SELECT * FROM bom_items WHERE bom_list_id = @id;"))
					{
						command.Parameters.AddWithValue("@id", (int)model.lists[x].ListID);
						command.Connection = conn;
						using (SqlDataReader sdr = command.ExecuteReader())
						{
							int num = 1;
							while (sdr.Read())
							{
								model.lists[x].Items.Add(new Employee_BOM_Materials_Items()
								{
									Description = sdr["bom_list_desc"].ToString(),
									ItemID = Convert.ToInt32(sdr["bom_item_id"]),
									ItemNumber = num,
									Subitems = new List<Employee_BOM_Materials_Subitems>()
								});
								num++;
							}
						}
					}
					for (int y = 0; y < model.lists[x].Items.Count; y++)
					{
						using (SqlCommand command = new SqlCommand("SELECT * FROM bom_subitems a " +
							"INNER JOIN materials b ON a.material_id = b.material_id " +
							"INNER JOIN supplier_materials c ON a.supplier_material_id = c.supplier_material_id " +
							"INNER JOIN measurement_units d ON b.measurement_unit_id = d.measurement_unit_id " +
							"INNER JOIN supplier_info e ON e.supplier_id = c.supplier_id " +
							"WHERE bom_item_id = @id;"))
						{
							command.Parameters.AddWithValue("@id", (int)model.lists[x].Items[y].ItemID);
							command.Connection = conn;
							using (SqlDataReader sdr = command.ExecuteReader())
							{
								int num = 1;
								while (sdr.Read())
								{
									model.lists[x].Items[y].Subitems.Add(new Employee_BOM_Materials_Subitems()
									{
										SubitemNumber = num,
										MaterialID = Convert.ToInt32(sdr["material_id"]),
										MaterialDesc = sdr["material_desc"].ToString(),
										MaterialUoM = sdr["measurement_unit_desc_short"].ToString(),
										MaterialQuantity = Convert.ToInt32(sdr["bom_subitem_quantity"]),
										MaterialQuantityWastage = (int)Math.Ceiling(Convert.ToInt32(sdr["bom_subitem_quantity"]) * model.Wastage),
										MaterialQuantityProvisions = (int)Math.Ceiling(Math.Ceiling(Convert.ToInt32(sdr["bom_subitem_quantity"]) * model.Wastage) * model.Provisions),
										MaterialCost = Convert.ToDouble(sdr["supplier_material_price"]) / 100,
										MaterialAmount = Math.Round((int)Math.Ceiling(Math.Ceiling(Convert.ToInt32(sdr["bom_subitem_quantity"]) * model.Wastage) * model.Provisions) * (Convert.ToDouble(sdr["supplier_material_price"]) / 100), 2),
										SupplierMaterialID = Convert.ToInt32(sdr["supplier_material_id"]),
										Supplier = sdr["supplier_desc"].ToString()
									});
									num++;
								}
							}
						}
					}
				}
			}

			model.totalCost = 0;
			foreach (Employee_BOM_Materials_Lists x in model.lists)
			{
				foreach (Employee_BOM_Materials_Items y in x.Items)
				{
					foreach (Employee_BOM_Materials_Subitems z in y.Subitems)
					{
						model.totalCost += (z.MaterialCost * Math.Ceiling((double)z.MaterialQuantity));
					}
				}
			}

			TempData["MCEData"] = JsonConvert.SerializeObject(model);
			return RedirectToAction("MCEAdd");
		}

		public IActionResult MCEAdd()
		{

			EmployeeBOMModel model = JsonConvert.DeserializeObject<EmployeeBOMModel>(TempData["MCEData"].ToString());
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> MCEAdd(EmployeeBOMModel model, string submitButton)
		{
			Debug.WriteLine(model == null);
			Debug.WriteLine("Menshevik");
			Debug.WriteLine(model.ProjectID);
			
			if (submitButton == "Update")
			{
				for (int x = 0; x < model.lists.Count; x++)
				{
					for (int y = 0; y < model.lists[x].Items.Count; y++)
					{
						for (int z = 0; z < model.lists[x].Items[y].Subitems.Count; z++)
						{
							Debug.WriteLine("Bolshevik");
							Debug.WriteLine(Math.Round(((double)model.Markup / 100.00) + 1.00, 2));
							model.lists[x].Items[y].Subitems[z].MarkedUpCost =
								Math.Ceiling(
								model.lists[x].Items[y].Subitems[z].MaterialCost *
								Math.Round((model.Markup / 100) + 1, 2));
							model.lists[x].Items[y].Subitems[z].TotalUnitRate =
								Math.Ceiling(
								model.lists[x].Items[y].Subitems[z].MarkedUpCost +
								model.lists[x].Items[y].Subitems[z].LabourCost);
							model.lists[x].Items[y].Subitems[z].MaterialAmount =
								Math.Ceiling(
								model.lists[x].Items[y].Subitems[z].TotalUnitRate *
								model.lists[x].Items[y].Subitems[z].MaterialQuantityProvisions);
						}
					}
				}

				model.totalCost = 0;
				foreach (Employee_BOM_Materials_Lists x in model.lists)
				{
					foreach (Employee_BOM_Materials_Items y in x.Items)
					{
						foreach (Employee_BOM_Materials_Subitems z in y.Subitems)
						{
							model.totalCost += (z.TotalUnitRate * Math.Ceiling((double)z.MaterialQuantityProvisions));
						}
					}
				}
				return View(model);
			}
			else if (submitButton == "Submit")
			{
				using (SqlConnection conn = new SqlConnection(connectionstring))
				{
					conn.Open();
					using (SqlTransaction trans = conn.BeginTransaction())
					{
						//using (SqlCommand())
					}
				}
				return View(model);
			}
			
			return View(model);
		}





		public Employee_BOM_Materials_Subitems GetMaterial(int material_id, double Quantity, string destination, int subitem_num, double cost, double wastage, double provisions, int SupplierMaterialID)
		{
			Employee_BOM_Materials_Subitems item = new Employee_BOM_Materials_Subitems();
			using (SqlConnection conn = new SqlConnection(connectionstring))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("SELECT a.material_id, a.material_desc, b.measurement_unit_desc_short FROM materials a " +
					" INNER JOIN measurement_units b " +
					" ON a.measurement_unit_id = b.measurement_unit_id " +
					" WHERE material_id = @material_id;"))
				{
					command.Parameters.AddWithValue("@material_id", material_id);
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							item.SubitemNumber = subitem_num;
							item.MaterialID = Convert.ToInt32(sdr["material_id"]);
							item.MaterialDesc = sdr["material_desc"].ToString();
							item.MaterialUoM = sdr["measurement_unit_desc_short"].ToString();
							item.MaterialQuantity = (int)Math.Ceiling(Quantity);
							item.MaterialQuantityWastage = (int)Math.Ceiling(Math.Ceiling(Quantity) * wastage);
							item.MaterialQuantityProvisions = (int)Math.Ceiling(Math.Ceiling(Math.Ceiling(Quantity) * wastage)*provisions);
							item.MaterialCost = cost;
							item.MaterialAmount = Math.Ceiling(Math.Ceiling(Math.Ceiling(Quantity) * wastage) * provisions) * cost;
							item.SupplierMaterialID = SupplierMaterialID;
						}
					}
				}
			}
			return item;
		}

		private MaterialsCostComparisonItem GetBestPrice(int MaterialID, string destination)
		{
			string _apikey = "ApFkiZUGSuNuTphyHstPFnkvL0IGwOKelabezyQVt4RwYTD-yE5n5dMgmeHugQgN";

			List<MaterialsCostComparisonItem> MaterialsCosts = new List<MaterialsCostComparisonItem>();
			using (SqlConnection conn = new SqlConnection(connectionstring))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand("SELECT c.material_id AS MaterialID, c.material_desc_long AS Material, a.supplier_material_id AS SupplierMaterialID, " +
					"a.supplier_material_price AS Price, d.supplier_id AS SupplierID, d.supplier_desc AS Supplier, d.employee_id AS Employee, " +
					"CONCAT(d.supplier_coordinates_latitude, ',', d.supplier_coordinates_longtitude) AS Coordinates " +
					"FROM supplier_materials a JOIN (" +
					"SELECT MIN(b.supplier_material_price) AS min_value FROM supplier_materials b WHERE b.material_id = @id AND b.supplier_material_availability = 1  AND b.supplier_material_archived = 0) min_table " +
					"ON a.supplier_material_price = min_table.min_value " +
					"INNER JOIN materials c ON a.material_id = c.material_id " +
					"INNER JOIN supplier_info d ON a.supplier_id = d.supplier_id " +
					"WHERE a.material_id = @id AND d.employee_id = @employee_id  " +
					"AND a.supplier_material_availability = 1 AND a.supplier_material_archived = 0 ;"))
				{
					command.Parameters.AddWithValue("@id", MaterialID);
					command.Parameters.AddWithValue("@employee_id", (int)HttpContext.Session.GetInt32("EmployeeID"));
					command.Connection = conn;
					using (SqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
							MaterialsCosts.Add(new MaterialsCostComparisonItem()
							{
								MaterialID = Convert.ToInt32(sdr["MaterialID"]),
								Description_Long = sdr["Material"].ToString(),
								Price = Convert.ToDouble(sdr["Price"]) / 100,
								SupplierID = Convert.ToInt32(sdr["SupplierID"]),
								SupplierDesc = sdr["Supplier"].ToString(),
								SupplierCoords = sdr["Coordinates"].ToString(),
								SupplierMaterialID = Convert.ToInt32(sdr["SupplierMaterialID"])
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
	}
}
