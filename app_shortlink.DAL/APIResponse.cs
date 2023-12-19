using System.Net;

namespace app_shortlink.DAL;

public class APIResponse
{

    public APIResponse()
    {
        ErrorMessages = new List<string>();
    }
    public HttpStatusCode StatusCode { get; set; }

    public bool IsSucces { get; set; } = true;
    
    public List<string> ErrorMessages { get; set; }
    
    public object Result { get; set; }

}