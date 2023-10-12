using System.ComponentModel.DataAnnotations;

namespace Frilou_UI_V2.Models
{
	public class AdminFormulaDebug
	{

		public int noOfStories { get; set; }
		public double heightOfFloors { get; set; }
		public double lengthOfBuilding { get; set; }
		public double widthOfBuilding { get; set; }
		public double sqmOfBuilding { get; set; }
		public double π = 3.14159f;

		public double floorThickness { get; set; }
		public double wallThickness { get; set; }
		public double rebarPercentage { get; set; }
		public double rebarDiameter { get; set; }
		public double nailConstant { get; set; }
		public double hollowBlockLength { get; set; }
		public double hollowBlockWidth { get; set; }
		public double hollowBlockHeight { get; set; }
		public double hollowBlockVolume { get; set; }
		public double hollowBlockConstant { get; set; }
		public double supportBeamLength { get; set; }
		public double supportBeamWidth { get; set; }
		public double supportBeamArea { get; set; }
		public double supportBeamSpace { get; set; }
		public double supportBeamVolume { get; set; }
		public double supportBeamsNeeded { get; set; }
		public double concreteRatioCement { get; set; }
		public double concreteRatioSand { get; set; }
		public double concreteRatioAggregate { get; set; }
		public double plywoodLength { get; set; }
		public double plywoodWidth { get; set; }
		public double plywoodArea { get; set; }
		public double plywoodSheetsPerSqm { get; set; }
		public double riserHeight { get; set; }
		public double threadDepth { get; set; }
		public double stairWidth { get; set; }
		public double numberOfSteps { get; set; }
		public double wasteage { get; set; }
		public double provisions { get; set; }


		public double rebarPrice { get; set; }
		public double hollowBlockPrice { get; set; }
		public double cementPrice { get; set; }
		public double sandPrice { get; set; }
		public double aggregatePrice { get; set; }
		public double concretePrice { get; set; }
		public double plywoodPrice { get; set; }
		public double plywoodPricePerSqm { get; set; }


		public double foundationHeight { get; set; }
		public double foundationVolume { get; set; }
		public double foundationPerimeter { get; set; }
		public double foundationWallArea  { get; set; }
		public double foundationNoOfHollowBlock { get; set; }
		public double foundationRebar { get; set; }
		public double foundationConcrete { get; set; }


		public double storeyHeight { get; set; }
		public double storeyPerimeter  { get; set; }
		public double storeyWallVolume { get; set; }
		public double storeyFloorVolume  { get; set; }
		public double storeyFloorPlywood { get; set; }
		public double storeyFloorNails { get; set; }
		public double storeyFloorConcrete { get; set; }
		public double storeyFloorRebar { get; set; }


		public double storeySupportBeamsNeeded { get; set; }
		public double storeySupportBeamsConcrete { get; set; }
		public double storeySupportBeamsRebar { get; set; }


		public double storeyWallConcrete { get; set; }
		public double storeyWallRebar { get; set; }


		public double stairsVolume { get; set; }
		public double stairsConcrete { get; set; }
		public double stairsRebar { get; set; }
	}

	public class EngineerItem
	{
		public string ID { get; set; }
		public string Description { get; set; }
	}

	public class ProjectModel
	{
		public List<EngineerItem> EngineerList { get; set; }
		public List<EngineerItem> BuildingList { get; set; }

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
		[Display(Name = "Engineer")]
		public string EngineerID { get; set; }

		[Required]
		[Display(Name = "Building Type")]
		public string BuildingID { get; set; }

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

	public class ProjectViewModel
	{
		public string	Title			{ get; set; }
		public string	ClientName		{ get; set; }
		public string	ClientContact	{ get; set; }
		public DateTime Date			{ get; set; }
		public string	Address			{ get; set; }
		public string	City			{ get; set; }
		public string	Region			{ get; set; }
		public string	Country			{ get; set; }
		public string	Longtitude		{ get; set; }
		public string	Latitude		{ get; set; }
		public string	Engineer		{ get; set; }
		public string	BuildingType	{ get; set; }
		public int		NumberOfStoreys { get; set; }
		public double	FloorHeight		{ get; set; }
		public double	BuildingLength	{ get; set; }
		public double	BuildingWidth	{ get; set; }
		public bool		bomCreated		{ get; set; }
	}
}
