namespace MasterIdiomas.Helper
{
    // Interface para o serviço de envio de e-mails
    public interface IEmail
    {
        // Método para enviar um e-mail com os parâmetros: destinatário, assunto e mensagem
        bool Enviar(string email, string assunto, string mensagem);
    }
}