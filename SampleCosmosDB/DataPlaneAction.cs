using Microsoft.Azure.Cosmos;
using System.Text.Json;

namespace SampleCosmosDB;

class DataPlaneAction(ClientFactory clientFactory, CosmosDbSettings settings)
{
	readonly CosmosClient cosmosClient = clientFactory.CreateCosmosClientWithManagedIdentity();
	readonly string databaseId = settings.DatabaseId;
	readonly string containerId = settings.ContainerId;

	public async Task CreateItemAsync(AssetDetails item)
	{
		var container = GetContainer();
		var partitionKey = new PartitionKey(item.PartitionKey);

		await container.CreateItemAsync(item, partitionKey);

		Console.WriteLine("Item created successfully.");
	}

	public async Task ReadItemAsync(string id, string partitionKey)
	{
		var container = GetContainer();
		var readItem = await container.ReadItemAsync<AssetDetails>(id, new PartitionKey(partitionKey));

		Console.WriteLine($"Item read successfully: {JsonSerializer.Serialize(readItem.Resource)}");
	}

	public async Task DeleteItemAsync(string id, string partitionKey)
	{
		var container = GetContainer();
		await container.DeleteItemAsync<AssetDetails>(id, new PartitionKey(partitionKey));

		Console.WriteLine("Item deleted successfully.");
	}

	public async Task UpsertItemAsync(AssetDetails item)
	{
		var container = GetContainer();
		var result = await container.UpsertItemAsync(item, new PartitionKey(item.PartitionKey));

		Console.WriteLine($"Item upserted successfully.{JsonSerializer.Serialize(result.Resource)}");
	}

	// SELECT * FROM c WHERE c.CompanyId = 'your-company-guid-here' AND c.OverviewStatus = 'Active'
	public async Task QueryItemsAsync(string queryString)
	{
		var container = GetContainer();
		var queryDefinition = new QueryDefinition(queryString);
		var queryResultSetIterator = container.GetItemQueryIterator<AssetDetails>(queryDefinition);
		var results = new List<AssetDetails>();
		while (queryResultSetIterator.HasMoreResults)
		{
			var response = await queryResultSetIterator.ReadNextAsync();
			results.AddRange(response);
		}

		Console.WriteLine($"Query returned {results.Count} items.");
		foreach (var item in results)
		{
			Console.WriteLine(JsonSerializer.Serialize(item));
		}
	}

	// This method is will fail when using managed identity.
	public async Task CreateContainerAsync()
	{
		var database = cosmosClient.GetDatabase(databaseId);

		var containerProperties = new ContainerProperties
		{
			Id = "Overviews1",
			PartitionKeyPath = "/PartitionKey",
			DefaultTimeToLive = 10000
		};

		await database.CreateContainerAsync(containerProperties);
	}

	private Container GetContainer()
	{
		var container = cosmosClient.GetContainer(databaseId, containerId);
		return container;
	}
}
