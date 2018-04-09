namespace Kachuwa.Web
{
    public class SmtpEmailSetting
    {
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseSSL { get; set; }
        public int Port { get; set; }
    }
}
