namespace TestApplication.Objects
{
    public class AccessTokenRequest
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }
    public class AccessTokenResponse
    {
        public string Accesstoken { get; set; } = string.Empty;
        public bool Valid { get; set; }
        public string StatusCode { get; set; } = string.Empty;

        public string validTime { get; set; } = string.Empty;
        public string StatusDescription { get; set; } = string.Empty;

    }
    public class ErrorResponse
    {
        public string StatusCode { get; set; } = string.Empty;
        public string StatusDescription { get; set; } = string.Empty;
    }
}
