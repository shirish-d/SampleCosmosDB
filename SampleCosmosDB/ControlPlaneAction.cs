using Azure;
using Azure.ResourceManager.CosmosDB;
using Azure.ResourceManager.CosmosDB.Models;

namespace SampleCosmosDB;

class ControlPlaneAction(CosmosDbAccount cosmosDbAccount)
{
	const string NewDatabaseId = "Test-FixedAssetsProjections";
	const string NewContainerId = "TestOverviews";

	public async Task PerformControlPlaneAction()
	{
		await CreateDatabase();
		await CreateContainerAsync();
		await DeleteContainerAsync();
		await DeleteDatabaseAsync();
	}

	public async Task CreateDatabase()
	{
		var cosmosDbAccountResource = await cosmosDbAccount.GetAsync();
		var databaseCollection = GetCosmosDbSqlDatabases(cosmosDbAccountResource);

		var databaseData = new CosmosDBSqlDatabaseCreateOrUpdateContent(cosmosDbAccountResource.Data.Location,
			new CosmosDBSqlDatabaseResourceInfo(NewDatabaseId));

		await databaseCollection.CreateOrUpdateAsync(WaitUntil.Completed, NewDatabaseId, databaseData);

		Console.WriteLine($"Database '{NewDatabaseId}' created successfully.");
	}

	public async Task DeleteDatabaseAsync()
	{
		var cosmosDbAccountResource = await cosmosDbAccount.GetAsync();
		var database = await GetDatabase(cosmosDbAccountResource);
		await database.DeleteAsync(WaitUntil.Completed);

		Console.WriteLine($"Database '{NewDatabaseId}' deleted successfully.");
	}

	public async Task CreateContainerAsync()
	{
		var cosmosDbAccountResource = await cosmosDbAccount.GetAsync();
		var database = await GetDatabase(cosmosDbAccountResource);
		var containerCollection = database.GetCosmosDBSqlContainers();

		var cosmosDbSqlContainerResourceInfo = new CosmosDBSqlContainerResourceInfo(NewContainerId)
		{
			PartitionKey = new CosmosDBContainerPartitionKey
			{
				Kind = CosmosDBPartitionKind.Hash,
				Paths = { "/id" }
			}
		};

		var containerData = new CosmosDBSqlContainerCreateOrUpdateContent(cosmosDbAccountResource.Data.Location,
			cosmosDbSqlContainerResourceInfo);

		await containerCollection.CreateOrUpdateAsync(WaitUntil.Completed, NewContainerId, containerData);

		Console.WriteLine($"Container '{NewContainerId}' created successfully.");
	}

	public async Task DeleteContainerAsync()
	{
		var cosmosDbAccountResource = await cosmosDbAccount.GetAsync();
		var database = await GetDatabase(cosmosDbAccountResource);
		var container = await database.GetCosmosDBSqlContainers().GetAsync(NewContainerId);

		await container.Value.DeleteAsync(WaitUntil.Completed);

		Console.WriteLine($"Container '{NewContainerId}' deleted successfully.");
	}

	static CosmosDBSqlDatabaseCollection GetCosmosDbSqlDatabases(CosmosDBAccountResource cosmosDbAccount)
	{
		return cosmosDbAccount.GetCosmosDBSqlDatabases();
	}

	static async Task<CosmosDBSqlDatabaseResource> GetDatabase(CosmosDBAccountResource cosmosDbAccount)
	{
		var databaseCollection = GetCosmosDbSqlDatabases(cosmosDbAccount);
		var database = await databaseCollection.GetAsync(NewDatabaseId);
		return database.Value;
	}
}
