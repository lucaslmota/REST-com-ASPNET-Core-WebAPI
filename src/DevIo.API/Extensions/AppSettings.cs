namespace DevIo.API.Extensions
{
    public class AppSettings
    {
        public string Secret { get; set; } =string.Empty;
        public int ExpiracaoHoras { get; set; }
        public string? Emissor { get; set; }
        public string? ValidoEm { get; set; }
    }
}
