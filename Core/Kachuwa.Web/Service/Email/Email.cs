namespace Kachuwa.Web
{
    public class Email
    {
        public string Subject { get; set; }

        public string MessageText { get; set; }

        public string MessageHtml { get; set; }

        public EmailAddress[] To { get; set; }

        public EmailAddress From { get; set; }
    }
}
