using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.CosmosDB;

namespace SampleCosmosDB;

class CosmosDbAccount(ClientFactory clientFactory, CosmosDbSettings settings)
{
	readonly ArmClient armClient = clientFactory.CreateArmClientWithManagedIdentity();
	CosmosDBAccountResource? cosmosDbAccount;

	public async Task<CosmosDBAccountResource> GetAsync()
	{
		return cosmosDbAccount ??= await GetCosmosDbAccountAsync();
	}

	async Task<CosmosDBAccountResource> GetCosmosDbAccountAsync()
	{
		var resourceIdentifier = new ResourceIdentifier($"/subscriptions/{settings.SubscriptionId}");
		var subscription = await armClient.GetSubscriptionResource(resourceIdentifier)
			.GetAsync();
		var resourceGroup = await subscription.Value.GetResourceGroups().GetAsync(settings.ResourceGroup);
		var account = await resourceGroup.Value.GetCosmosDBAccounts().GetAsync(settings.AccountName);
		return account.Value;
	}

	async Task<CosmosDBAccountResource> GetCosmosDbAccountByResourceIdentifierAsync()
	{
		var resourceId =
			$"/subscriptions/{settings.SubscriptionId}/resourceGroups/{settings.ResourceGroup}" +
			$"/providers/Microsoft.DocumentDB/databaseAccounts/{settings.AccountName}";
		var resourceIdentifier = new ResourceIdentifier(resourceId);
		var account = await armClient.GetCosmosDBAccountResource(resourceIdentifier).GetAsync(CancellationToken.None);
		return account.Value;
	}
}
