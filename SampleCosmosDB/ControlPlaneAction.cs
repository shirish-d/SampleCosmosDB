using Azure;
using Azure.ResourceManager.CosmosDB;
using Azure.ResourceManager.CosmosDB.Models;

namespace SampleCosmosDB;

class ControlPlaneAction(CosmosDbAccount cosmosDbAccount, CosmosDbSettings settings)
{
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

		var response = await databaseCollection.GetIfExistsAsync(settings.DatabaseId);

		if (response.HasValue)
		{
			_ = response.Value;
			Console.WriteLine($"Database '{settings.DatabaseId}' exists");
		}
		else
			Console.WriteLine($"Database '{settings.DatabaseId}' does not exist");

		var databaseData = new CosmosDBSqlDatabaseCreateOrUpdateContent(cosmosDbAccountResource.Data.Location,
			new CosmosDBSqlDatabaseResourceInfo(settings.DatabaseId));

		await databaseCollection.CreateOrUpdateAsync(WaitUntil.Completed, settings.DatabaseId, databaseData);

		Console.WriteLine($"Database '{settings.DatabaseId}' created successfully.");
	}

	public async Task DeleteDatabaseAsync()
	{
		var cosmosDbAccountResource = await cosmosDbAccount.GetAsync();
		var database = await GetDatabaseAsync(cosmosDbAccountResource);
		await database.DeleteAsync(WaitUntil.Completed);

		Console.WriteLine($"Database '{settings.DatabaseId}' deleted successfully.");
	}

	public async Task CreateContainerAsync()
	{
		var cosmosDbAccountResource = await cosmosDbAccount.GetAsync();
		var database = await GetDatabaseAsync(cosmosDbAccountResource);
		var containerCollection = database.GetCosmosDBSqlContainers();

		var cosmosDbSqlContainerResourceInfo = new CosmosDBSqlContainerResourceInfo(settings.ContainerId)
		{
			PartitionKey = new CosmosDBContainerPartitionKey
			{
				Kind = CosmosDBPartitionKind.Hash,
				Paths = { "/PartitionKey" }
			}
		};

		var containerData = new CosmosDBSqlContainerCreateOrUpdateContent(cosmosDbAccountResource.Data.Location,
			cosmosDbSqlContainerResourceInfo);

		await containerCollection.CreateOrUpdateAsync(WaitUntil.Completed, settings.ContainerId, containerData);

		Console.WriteLine($"Container '{settings.ContainerId}' created successfully.");
	}

	public async Task DeleteContainerAsync()
	{
		var cosmosDbAccountResource = await cosmosDbAccount.GetAsync();
		var database = await GetDatabaseAsync(cosmosDbAccountResource);
		var container = await database.GetCosmosDBSqlContainers().GetAsync(settings.ContainerId);

		await container.Value.DeleteAsync(WaitUntil.Completed);

		Console.WriteLine($"Container '{settings.ContainerId}' deleted successfully.");
	}

	static CosmosDBSqlDatabaseCollection GetCosmosDbSqlDatabases(CosmosDBAccountResource cosmosDbAccount)
	{
		return cosmosDbAccount.GetCosmosDBSqlDatabases();
	}

	async Task<CosmosDBSqlDatabaseResource> GetDatabaseAsync(CosmosDBAccountResource cosmosDbAccount)
	{
		var databaseCollection = GetCosmosDbSqlDatabases(cosmosDbAccount);
		var database = await databaseCollection.GetAsync(settings.DatabaseId);
		return database.Value;
	}
}
