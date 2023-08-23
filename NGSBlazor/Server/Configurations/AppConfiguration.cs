namespace NGSBlazor.Server.Configurations
{
    public class AppConfiguration
    {
        public required string JWTSecret { get; set; }
        public required bool Debug { get; set; }
    }
}
