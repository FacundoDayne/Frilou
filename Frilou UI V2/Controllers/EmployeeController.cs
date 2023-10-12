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


namespace Frilou_UI_V2.Controllers
{
	public class EmployeeController : Controller
	{
		private readonly string connectionstring = "Data Source=localhost;port=3306;Initial Catalog=bom_mce_db;User Id=root;password=password123;";
		
		public IActionResult Index()
		{
			EmployeeDashboardModel model = new EmployeeDashboardModel();
			model.projects = GetNewProjects(HttpContext.Session.GetInt32("EmployeeID"));
			return View(model);
		}

		public List<EmployeeNewProject> GetNewProjects(int? id)
		{
			List<EmployeeNewProject> projects = new List<EmployeeNewProject>();

			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				conn.Open();
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM projects a " +
					"LEFT JOIN bom b " +
					"ON a.project_id = b.project_id " +
					"WHERE b.project_id IS NULL " +
					"AND project_engineer_id = @project_engineer_id;"))
				{
					command.Parameters.AddWithValue("@project_engineer_id", (int)id);
					command.Connection = conn;
					using (MySqlDataReader sdr = command.ExecuteReader())
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

			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				conn.Open();
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM employee_templates WHERE employee_id = @employee_id;"))
				{
					command.Parameters.AddWithValue("@employee_id", HttpContext.Session.GetInt32("EmployeeID"));
					command.Connection = conn;
					using (MySqlDataReader sdr = command.ExecuteReader())
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
			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				conn.Open();
				using (MySqlTransaction transaction = conn.BeginTransaction())
				{
					using (MySqlCommand command = new MySqlCommand("INSERT INTO `bom_mce_db`.`template_formula_constants` " +
						"(`floor_thickness`,`wall_thickness`,`rebar_thickness`,`nail_interval`,`hollow_block_constant`,`support_beam_length`, " +
						"`support_beam_width`,`support_beam_interval`,`concrete_ratio_cement`,`concrete_ratio_sand`,`concrete_ratio_aggregate`, " +
						"`plywood_length`,`plywood_width`,`riser_height`,`thread_depth`,`wastage`,`provisions`) VALUES( " +
						"@floor_thickness,@wall_thickness,@rebar_thickness,@nail_interval,@hollow_block_constant,@support_beam_length, " +
						"@support_beam_width,@support_beam_interval,@concrete_ratio_cement,@concrete_ratio_sand,@concrete_ratio_aggregate, " +
						"@plywood_length,@plywood_width,@riser_height,@thread_depth,@wastage,@provisions); " +
						"SELECT last_insert_id() FROM `bom_mce_db`.`template_formula_constants`;"))
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
					using (MySqlCommand command = new MySqlCommand("INSERT INTO `bom_mce_db`.`employee_templates` " +
						"(`employee_id`, `formula_constants_id`, `template_description`, `template_descritpion_long`) VALUES ( " +
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
			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				conn.Open();
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM employee_templates a " +
					"INNER JOIN template_formula_constants b ON a.formula_constants_id = b.formula_constants_id " +
					"WHERE template_id = @template_id;"))
				{
					command.Parameters.AddWithValue("@template_id", (uint)id);
					command.Connection = conn;
					using (MySqlDataReader sdr = command.ExecuteReader())
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

			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				conn.Open();
				using (MySqlCommand command = new MySqlCommand("UPDATE `bom_mce_db`.`employee_templates` SET " +
					"`template_description` = @template_description, `template_descritpion_long` = @template_descritpion_long " +
					"WHERE `template_id` = @template_id;" +
					"UPDATE `bom_mce_db`.`template_formula_constants` SET " +
					"`floor_thickness` = @floor_thickness, `wall_thickness` = @wall_thickness, `rebar_thickness` = @rebar_thickness," +
					"`nail_interval` = @nail_interval, `hollow_block_constant` = @hollow_block_constant, `support_beam_length` = @support_beam_length, " +
					"`support_beam_width` = @support_beam_width, `support_beam_interval` = @support_beam_interval, `concrete_ratio_cement` = @concrete_ratio_cement," +
					"`concrete_ratio_sand` = @concrete_ratio_sand, `concrete_ratio_aggregate` = @concrete_ratio_aggregate, `plywood_length` = @plywood_length, " +
					"`plywood_width` = @plywood_width, `riser_height` = @riser_height, `thread_depth` = @thread_depth, `wastage` = @wastage, `provisions` = @provisions " +
					"WHERE `formula_constants_id` = @formula_constants_id;"))
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


			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				conn.Open();
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM projects a " +
					"INNER JOIN employee_info b ON a.project_engineer_id = b.employee_info_id " +
					"INNER JOIN building_types c ON a.building_types_id = c.building_types_id " +
					"WHERE a.project_id = @project_id"))
				{
					command.Parameters.AddWithValue("@project_id", id);
					command.Connection = conn;
					using (MySqlDataReader sdr = command.ExecuteReader())
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
						}
					}
				}
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM employee_templates WHERE employee_id = @employee_id;"))
				{
					command.Parameters.AddWithValue("@employee_id", HttpContext.Session.GetInt32("EmployeeID"));
					command.Connection = conn;
					using (MySqlDataReader sdr = command.ExecuteReader())
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

			using (MySqlConnection conn = new MySqlConnection(connectionstring))
			{
				conn.Open();
				using (MySqlCommand command = new MySqlCommand("SELECT * FROM employee_templates a " +
					"INNER JOIN template_formula_constants b ON a.formula_constants_id = b.formula_constants_id " +
					"WHERE template_id = @template_id;"))
				{
					command.Parameters.AddWithValue("@template_id", Convert.ToUInt32(model.TemplateID));
					command.Connection = conn;
					using (MySqlDataReader sdr = command.ExecuteReader())
					{
						while (sdr.Read())
						{
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
			double rebarPrice = GetBestPrice(5, location).Price, // per piece. Rebars * rebarPrice
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
								foundationNoOfHollowBlock = foundationWallArea * hollowBlockConstant,
			/*new*/             foundationRebar = (lengthOfBuilding * widthOfBuilding * rebarPercentage) * foundationHeight,
								foundationConcrete = foundationVolume
				;

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
			model.lists[0].Items[0].Subitems.Add(GetMaterial(2, foundationConcreteCement, location, 1, cementPrice));

			int foundationConcreteSand = (int)Math.Ceiling(foundationConcrete * sandRatio);
			model.lists[0].Items[0].Subitems.Add(GetMaterial(3, foundationConcreteSand, location, 2, sandPrice));

			int foundationConcreteAggregate = (int)Math.Ceiling(foundationConcrete * aggregateRatio);
			model.lists[0].Items[0].Subitems.Add(GetMaterial(4, foundationConcreteAggregate, location, 3, aggregatePrice));

			int foundationRebarAmount = (int)Math.Ceiling(foundationRebar);
			model.lists[0].Items[0].Subitems.Add(GetMaterial(5, foundationRebarAmount, location, 4, rebarPrice));

			int foundationHollowBlock = (int)Math.Ceiling(foundationNoOfHollowBlock);
			model.lists[0].Items[0].Subitems.Add(GetMaterial(6, foundationHollowBlock, location, 5, hollowBlockPrice));

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
				model.lists[i].Items[0].Subitems.Add(GetMaterial(2, floorConcreteCement, location, 1, cementPrice));

				int floorConcreteSand = (int)Math.Ceiling(storeyFloorConcrete * sandRatio);
				model.lists[i].Items[0].Subitems.Add(GetMaterial(3, floorConcreteSand, location, 2, sandPrice));

				int floorConcreteAggregate = (int)Math.Ceiling(storeyFloorConcrete * aggregateRatio);
				model.lists[i].Items[0].Subitems.Add(GetMaterial(4, floorConcreteAggregate, location, 3, aggregatePrice));

				int floorRebarAmount = (int)Math.Ceiling(storeyFloorRebar);
				model.lists[i].Items[0].Subitems.Add(GetMaterial(5, floorRebarAmount, location, 4, rebarPrice));


				//wall
				int wallConcreteCement = (int)Math.Ceiling(storeyWallConcrete * cementRatio);
				model.lists[i].Items[1].Subitems.Add(GetMaterial(2, wallConcreteCement, location, 1, cementPrice));
				int wallConcreteSand = (int)Math.Ceiling(storeyWallConcrete * sandRatio);
				model.lists[i].Items[1].Subitems.Add(GetMaterial(3, wallConcreteSand, location, 2, sandPrice));
				int wallConcreteAggregate = (int)Math.Ceiling(storeyWallConcrete * aggregateRatio);
				model.lists[i].Items[1].Subitems.Add(GetMaterial(4, wallConcreteAggregate, location, 3, aggregatePrice));

				int wallRebarAmount = (int)Math.Ceiling(storeyWallRebar);
				model.lists[i].Items[1].Subitems.Add(GetMaterial(5, wallRebarAmount, location, 4, rebarPrice));

				//beam
				int beamConcreteCement = (int)Math.Ceiling(storeySupportBeamsConcrete * cementRatio);
				model.lists[i].Items[2].Subitems.Add(GetMaterial(2, beamConcreteCement, location, 1, cementPrice));
				int beamConcreteSand = (int)Math.Ceiling(storeySupportBeamsConcrete * sandRatio);
				model.lists[i].Items[2].Subitems.Add(GetMaterial(3, beamConcreteSand, location, 2, sandPrice));
				int beamConcreteAggregate = (int)Math.Ceiling(storeySupportBeamsConcrete * aggregateRatio);
				model.lists[i].Items[2].Subitems.Add(GetMaterial(4, beamConcreteAggregate, location, 3, aggregatePrice));

				int beamRebarAmount = (int)Math.Ceiling(storeySupportBeamsRebar);
				model.lists[i].Items[2].Subitems.Add(GetMaterial(5, beamRebarAmount, location, 4, rebarPrice));

				//stair
				int stairsConcreteCement = (int)Math.Ceiling(stairsConcrete * cementRatio);
				model.lists[i].Items[3].Subitems.Add(GetMaterial(2, stairsConcreteCement, location, 1, cementPrice));
				int stairsConcreteSand = (int)Math.Ceiling(stairsConcrete * sandRatio);
				model.lists[i].Items[3].Subitems.Add(GetMaterial(3, stairsConcreteSand, location, 2, sandPrice));
				int stairsConcreteAggregate = (int)Math.Ceiling(stairsConcrete * aggregateRatio);
				model.lists[i].Items[3].Subitems.Add(GetMaterial(4, stairsConcreteAggregate, location, 3, aggregatePrice));

				int stairsRebarAmount = (int)Math.Ceiling(stairsRebar);
				model.lists[i].Items[3].Subitems.Add(GetMaterial(5, stairsRebarAmount, location, 4, rebarPrice));
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
			return View(model);
		}

		public IActionResult BOMView()
		{

			return View();
		}





		public Employee_BOM_Materials_Subitems GetMaterial(int material_id, double Quantity, string destination, int subitem_num, double cost, double wastage, double provisions)
		{
			Employee_BOM_Materials_Subitems item = new Employee_BOM_Materials_Subitems();
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
							item.MaterialQuantityWastage = (int)Math.Ceiling(Math.Ceiling(Quantity) * wastage);
							item.MaterialQuantityProvisions = (int)Math.Ceiling(Math.Ceiling(Math.Ceiling(Quantity) * wastage)*provisions);
							item.MaterialCost = cost;
							item.MaterialAmount = Math.Ceiling(Math.Ceiling(Math.Ceiling(Quantity) * wastage) * provisions) * cost;
						}
					}
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
	}
}
