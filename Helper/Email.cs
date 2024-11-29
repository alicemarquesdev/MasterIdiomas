using System.Net;
using System.Net.Mail;

namespace MasterIdiomas.Helper
{
    public class Email : IEmail
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Email> _logger;

        public Email(IConfiguration configuration, ILogger<Email> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public bool Enviar(string email, string assunto, string mensagem)
        {
            try
            {
                string host = _configuration.GetValue<string>("SMTP:Host") ?? throw new ArgumentNullException("SMTP:Host");
                string nome = _configuration.GetValue<string>("SMTP:Nome") ?? throw new ArgumentNullException("SMTP:Nome");
                string username = _configuration.GetValue<string>("SMTP:UserName") ?? throw new ArgumentNullException("SMTP:UserName");
                string senha = _configuration.GetValue<string>("SMTP:Senha") ?? throw new ArgumentNullException("SMTP:Senha");
                int porta = _configuration.GetValue<int?>("SMTP:Porta") ?? throw new ArgumentNullException("SMTP:Porta");

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(username, nome),
                    Subject = assunto,
                    Body = mensagem,
                    IsBodyHtml = true, // Configurável conforme necessidade
                    Priority = MailPriority.High // Alterar para Normal se necessário
                };

                mail.To.Add(email);

                using (SmtpClient smtp = new SmtpClient(host, porta))
                {
                    smtp.Credentials = new NetworkCredential(username, senha);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    return true;
                }
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Erro ao enviar o e-mail via SMTP.");
                return false;
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Erro de formato ao enviar o e-mail.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro desconhecido ao enviar o e-mail.");
                return false;
            }
        }
    }
}