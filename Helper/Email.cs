using System.Net;
using System.Net.Mail;

namespace MasterIdiomas.Helper
{
    // Classe responsável pelo envio de e-mails
    public class Email : IEmail
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Email> _logger;

        // Construtor para injeção de dependências
        public Email(IConfiguration configuration, ILogger<Email> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // Método para enviar um e-mail
        public bool Enviar(string email, string assunto, string mensagem)
        {
            try
            {
                // Recupera as configurações de SMTP do arquivo de configuração
                string host = _configuration.GetValue<string>("SMTP:Host") ?? throw new ArgumentNullException("SMTP:Host");
                string nome = _configuration.GetValue<string>("SMTP:Nome") ?? throw new ArgumentNullException("SMTP:Nome");
                string username = _configuration.GetValue<string>("SMTP:UserName") ?? throw new ArgumentNullException("SMTP:UserName");
                string senha = _configuration.GetValue<string>("SMTP:Senha") ?? throw new ArgumentNullException("SMTP:Senha");
                int porta = _configuration.GetValue<int?>("SMTP:Porta") ?? throw new ArgumentNullException("SMTP:Porta");

                // Criação do objeto MailMessage para compor o e-mail
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(username, nome), // Definindo remetente
                    Subject = assunto, // Definindo assunto do e-mail
                    Body = mensagem, // Definindo corpo da mensagem
                    IsBodyHtml = true, // Define se o corpo do e-mail será em HTML
                    Priority = MailPriority.High // Define a prioridade do e-mail
                };

                // Adiciona o destinatário
                mail.To.Add(email);

                // Envio do e-mail utilizando o SmtpClient
                using (SmtpClient smtp = new SmtpClient(host, porta))
                {
                    smtp.Credentials = new NetworkCredential(username, senha); // Credenciais para autenticação no servidor SMTP
                    smtp.EnableSsl = false;
                    smtp.TargetName = "STARTTLS"; smtp.Send(mail); // Envia o e-mail
                    return true; // Retorna true se o envio for bem-sucedido
                }
            }
            // Tratamento de exceções específicas para problemas no envio de e-mail
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Erro ao enviar o e-mail via SMTP.");
                return false; // Retorna false se ocorrer erro no envio
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Erro de formato ao enviar o e-mail.");
                return false; // Retorna false se houver erro no formato de dados
            }
            // Captura qualquer outro erro inesperado
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro desconhecido ao enviar o e-mail.");
                return false; // Retorna false se ocorrer erro desconhecido
            }
        }
    }
}