using Microsoft.Extensions.Configuration;
using SampleCosmosDB;

var config = new ConfigurationBuilder()
	.SetBasePath(AppContext.BaseDirectory)
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.Build();

var cosmosDbSettings = config.GetSection("CosmosDb").Get<CosmosDbSettings>() ?? new CosmosDbSettings();

try
{
	// Control Plane actions
	var cosmosDbAccount = new CosmosDbAccount(new ClientFactory(cosmosDbSettings), cosmosDbSettings);
	var controlPlaneAction = new ControlPlaneAction(cosmosDbAccount);
	await controlPlaneAction.PerformControlPlaneAction();

	// Prepare data
	var assetDetails = new AssetDetails(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

	// Data Plane actions
	var dataPlaneAction = new DataPlaneAction(new ClientFactory(cosmosDbSettings), cosmosDbSettings);
	await dataPlaneAction.CreateItemAsync(assetDetails);
	await dataPlaneAction.ReadItemAsync(assetDetails.Id, assetDetails.PartitionKey);
	assetDetails.Amount = 2000.00M;
	await dataPlaneAction.UpsertItemAsync(assetDetails);
	await dataPlaneAction.QueryItemsAsync(
		$"SELECT * FROM c WHERE c.CompanyId = '{assetDetails.CompanyId}' AND c.OverviewStatus = '{assetDetails.OverviewStatus}'");
	await dataPlaneAction.DeleteItemAsync(assetDetails.Id, assetDetails.PartitionKey);
}
catch (Exception e)
{
	Console.WriteLine(e);
}

Console.ReadLine();
