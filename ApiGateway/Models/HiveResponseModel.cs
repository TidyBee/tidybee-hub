namespace ApiGateway.Models
{
    public class HiveJsonResponse
    {
        public List<HiveResponseModel>? Responses { get; set; }
    }

    public class HiveResponseModel
    {
        public int StatusCode { get; set; }
        public string? Content { get; set; }

    }
}
