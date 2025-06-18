namespace MasterIdiomas.Helper
{
    // Interface para o serviço de envio de e-mails
    public interface IEmail
    {
        // Método para enviar um e-mail com os parâmetros: destinatário, assunto e mensagem
        Task<bool> EnviarEmailAsync(string destinatario, string assunto, string mensagem);
    }
}