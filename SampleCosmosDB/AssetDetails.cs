using Newtonsoft.Json;

namespace SampleCosmosDB;

class AssetDetails
{
	public AssetDetails()
	{
	}

	public AssetDetails(Guid organisationId, Guid companyId, Guid id)
	{
		OrganisationId = organisationId;
		CompanyId = companyId;
		Id = id.ToString();
		PartitionKey = CreatePartitionKey(organisationId, companyId);
		OverviewStatus = "Active";
		Amount = 1000.00M;
	}

	public Guid OrganisationId { get; set; }
	public Guid CompanyId { get; set; }

	[JsonProperty(PropertyName = "id")]
	public string Id { get; set; } = string.Empty;
	public string PartitionKey { get; set; } = string.Empty;
	public string OverviewStatus { get; set; } = string.Empty;
	public decimal Amount { get; set; }

	static string CreatePartitionKey(Guid organisationId, Guid companyId)
	{
		return $"{organisationId}:{companyId}";
	}
}