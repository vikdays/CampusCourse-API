public class Response
{
    public Response(string status, string message) 
    {
        Status = status;
        Message = message;
    }

    public string? Status { get; set; }

    public string? Message { get; set; }

}
