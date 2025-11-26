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
		var subscription = await armClient.GetDefaultSubscriptionAsync();
		var resourceGroup = await subscription.GetResourceGroups().GetAsync(settings.ResourceGroup);
		var account = await resourceGroup.Value.GetCosmosDBAccounts().GetAsync(settings.AccountName);
		return account.Value;
	}
}
