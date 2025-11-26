using Azure.Identity;
using Azure.ResourceManager;
using Microsoft.Azure.Cosmos;

namespace SampleCosmosDB;

class ClientFactory(CosmosDbSettings settings)
{
	public CosmosClient CreateCosmosClientWithManagedIdentity()
	{
		string accountEndpoint = $"https://{settings.AccountName}.documents.azure.com:443/";
		var options = new CosmosClientOptions { ConnectionMode = ConnectionMode.Gateway };

		var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
		{
			ManagedIdentityClientId = settings.AzureClientId
		});

		Console.WriteLine($"Cosmos client created with managed identity. Azure client Id: {settings.AzureClientId}");

		return new CosmosClient(accountEndpoint, credential, options);
	}

	public ArmClient CreateArmClientWithManagedIdentity()
	{
		var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
		{
			ManagedIdentityClientId = settings.AzureClientId
		});

		Console.WriteLine($"Arm client created with default managed identity. Azure client Id: {settings.AzureClientId}");

		return new ArmClient(credential);
	}
}
