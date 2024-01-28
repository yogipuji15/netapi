namespace mailservice.Models
{
    public class EmailConfiguration
    {
        public string SmtpServer { get; set; } = null;
        public int Port { get; set; }
        public string From { get; set; } = null;
        public string UserName { get; set; } = null;
        public string Password { get; set; } = null;
    }
}