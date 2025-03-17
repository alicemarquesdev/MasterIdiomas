namespace MasterIdiomas.Helper
{
    // Classe para armazenar as configurações de e-mail
    public class EmailSettings
    {
        // Propriedade para armazenar o endereço do servidor SMTP
        public string SmtpServer { get; set; }

        // Propriedade para armazenar a porta do servidor SMTP
        public int SmtpPort { get; set; }

        // Propriedade para armazenar o e-mail do remetente
        public string SenderEmail { get; set; }

        // Propriedade para armazenar a senha do e-mail do remetente
        public string SenderPassword { get; set; }
    }
}
