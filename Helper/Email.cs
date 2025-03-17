using System.Net.Mail;  // Namespace para lidar com o envio de e-mails.
using System.Net;  // Namespace para autenticação de rede (credentials).
using Microsoft.Extensions.Options;  // Namespace para acessar configurações injetadas.

namespace MasterIdiomas.Helper
{
    // Classe responsável pelo envio de e-mails
    public class Email : IEmail
    {
        // Definindo variáveis para armazenar as configurações do servidor SMTP e credenciais de envio
        private readonly string _smtpServer;
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly int _smtpPort;

        // Construtor da classe, recebe as configurações de e-mail (EmailSettings) e as armazena nas variáveis de instância.
        public Email(IOptions<EmailSettings> emailSettings)
        {
            _smtpServer = emailSettings.Value.SmtpServer;  // Configuração do servidor SMTP
            _senderEmail = emailSettings.Value.SenderEmail;  // E-mail do remetente
            _senderPassword = emailSettings.Value.SenderPassword;  // Senha do remetente
            _smtpPort = emailSettings.Value.SmtpPort;  // Porta do servidor SMTP
        }

        // Método assíncrono para enviar um e-mail
        public async Task<bool> EnviarEmailAsync(string destinatario, string assunto, string mensagem)
        {
            // Recolhe as configurações do servidor SMTP e credenciais para enviar o e-mail
            var smtpServer = _smtpServer;
            var smtpPort = _smtpPort;
            var senderEmail = _senderEmail;
            var senderPassword = _senderPassword;

            // Usando o SmtpClient para configurar e enviar o e-mail
            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);  // Configura as credenciais de autenticação
                client.EnableSsl = true;  // Habilita SSL para segurança durante a comunicação com o servidor SMTP

                // Criação do objeto MailMessage para configurar os dados do e-mail
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),  // E-mail do remetente
                    Subject = assunto,  // Assunto do e-mail
                    Body = mensagem,  // Corpo do e-mail
                    IsBodyHtml = true  // Indica que o corpo do e-mail é em HTML
                };

                mailMessage.To.Add(destinatario);  // Adiciona o destinatário ao e-mail

                // Envia o e-mail de forma assíncrona
                await client.SendMailAsync(mailMessage);

                return true;  // Retorna true para indicar que o e-mail foi enviado com sucesso
            }
        }

    }
}
