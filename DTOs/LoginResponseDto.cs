namespace asp_net_web_api.API.DTO
{
    public class LoginResponseDto
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public string AccessToken {get;set;}
        public string RefreshToken {get;set;}
    }
}