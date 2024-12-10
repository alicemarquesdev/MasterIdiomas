using System.Security.Cryptography;
using System.Text;

namespace MasterIdiomas.Helper
{
    // Classe estática para realizar operações de criptografia
    public static class Criptografia
    {
        // Método de extensão para gerar um hash SHA1 a partir de uma string
        public static string GerarHash(this string valor)
        {
            // Cria uma instância do algoritmo SHA1
            using (var hash = SHA1.Create())
            {
                // Codifica a string de entrada para um array de bytes usando ASCII
                var encoding = new ASCIIEncoding();
                var array = encoding.GetBytes(valor);

                // Computa o hash
                array = hash.ComputeHash(array);

                // Converte o array de bytes em uma string hexadecimal
                var strHexa = new StringBuilder();

                foreach (var item in array)
                {
                    // Adiciona o valor hexadecimal de cada byte ao StringBuilder
                    strHexa.Append(item.ToString("x2"));
                }

                // Retorna a string hexadecimal gerada
                return strHexa.ToString();
            }
        }
    }
}