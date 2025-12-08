namespace SampleCosmosDB;

class CosmosDbSettings
{
	public string AccountName { get; set; } = string.Empty;
	public string AzureClientId { get; set; } = string.Empty;
	public string ContainerId { get; set; } = string.Empty;
	public string DatabaseId { get; set; } = string.Empty;
	public bool EnableManagedIdentity { get; set; }
	public string ResourceGroup { get; set; } = string.Empty;
	public string SubscriptionId { get; set; } = string.Empty;
}
