namespace rsa_application.Dtos
{
    public class RsaInfo
    {
        public int KeySize { get; internal set; }
        public string PrivateKey { get; internal set; }
        public string PublicaKey { get; internal set; }
        public string FormatName { get; internal set; }
    }
}
