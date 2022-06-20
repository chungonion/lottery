namespace Lottery.Models.Response;

public class ResponseModel
{
    public string ApiVersion { get; } = "0.0.1"; 
    public int Status { get; set; }
    public object Data { get; set; } = new object() { };
    public object Error { get; set; } = new object() { };
}