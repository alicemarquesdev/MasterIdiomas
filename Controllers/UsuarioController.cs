using MasterIdiomas.Filters;
using MasterIdiomas.Helper;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace MasterIdiomas.Controllers
{
    // UsuarioController gerencia as ações relacionadas ao usuário.
    // Métodos implementados:
    // - AlterarSenha: Exibe e processa a alteração de senha do usuário.
    // - AtualizarUsuario: Exibe e processa a atualização dos dados do usuário.
    // - RemoverUsuario: Exclui a conta do usuário.

    [UsuarioLogado] // Filtro que garante que o usuário precisa estar logado
    public class UsuarioController : Controller
    {
        // Dependências injetadas no controlador
        private readonly ISessao _sessao;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ILogger<UsuarioController> _logger; // Logger para registrar ações e erros

        // Construtor que recebe as dependências. Lança exceção caso alguma dependência não seja fornecida.
        public UsuarioController(ISessao sessao,
                                 IUsuarioRepositorio usuarioRepositorio,
                                 ILogger<UsuarioController> logger)
        {
            // Validações de parâmetros do construtor
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao), "Sessão não pode ser nula.");
            _usuarioRepositorio = usuarioRepositorio ?? throw new ArgumentNullException(nameof(usuarioRepositorio), "Repositório de usuário não pode ser nulo.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger não pode ser nulo.");
        }

        // Ação para exibir o formulário de alteração de senha
        public IActionResult AlterarSenha()
        {
            return View();
        }

        // Ação para exibir o formulário de atualização de dados do usuário
        public async Task<IActionResult> AtualizarUsuario(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id inválido.");
            }

            try
            {
                // Recupera o usuário do repositório pelo ID
                UsuarioModel usuario = await _usuarioRepositorio.BuscarUsuarioPorIdAsync(id);

                // Verifica se o usuário foi encontrado
                if (usuario == null)
                {
                    TempData["MensagemErro"] = "Usuário não encontrado.";
                    return RedirectToAction("Index", "Home");
                }

                // Converte o modelo de usuário para um modelo sem senha
                UsuarioSemSenhaModel usuarioSemSenha = new UsuarioSemSenhaModel
                {
                    Id = usuario.UsuarioId,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Genero = usuario.Genero,
                    DataNascimento = usuario.DataNascimento
                };

                return View(usuarioSemSenha); // Retorna a view com os dados do usuário
            }
            catch (Exception ex)
            {
                // Loga erro e exibe mensagem genérica
                _logger.LogError(ex, "Erro ao buscar usuário com ID {Id}", id);
                TempData["MensagemErro"] = "Ocorreu um erro ao buscar os dados do usuário.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Ação POST para alterar a senha do usuário
        [HttpPost]
        [ValidateAntiForgeryToken]  // Valida o Token Anti-Forgery
        public async Task<IActionResult> AlterarSenha(AlterarSenhaModel alterarSenhaModel)
        {
            try
            {
                UsuarioModel usuarioDb = _sessao.BuscarSessaoUsuario();

                // Verifica se a sessão do usuário está válida
                if (usuarioDb == null)
                {
                    throw new ArgumentException("Usuario não encontrado.");

                }

                alterarSenhaModel.Id = usuarioDb.UsuarioId;

                // Valida o modelo de dados
                if (ModelState.IsValid)
                {
                    // Chama o método para alterar a senha no repositório
                    await _usuarioRepositorio.AlterarSenhaAsync(alterarSenhaModel);
                    TempData["MensagemSucesso"] = "Senha alterada com sucesso!";
                    return RedirectToAction("AlterarSenha");
                }

                TempData["MensagemErro"] = "Os dados fornecidos são inválidos.";
                return View(alterarSenhaModel); // Retorna à view com os erros de validação
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar alterar senha do usuário.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao tentar  alterar senha do usuário, tente novamente.";
                }

                return RedirectToAction("AlterarSenha");
            }
        }

        // Ação POST para atualizar os dados do usuário
        [HttpPost]
        [ValidateAntiForgeryToken]  // Valida o Token Anti-Forgery
        public async Task<IActionResult> AtualizarUsuario(UsuarioSemSenhaModel usuarioSemSenha)
        {
            try
            {
                if (usuarioSemSenha == null)
                {
                    // Lança exceção se o modelo estiver nulo
                    throw new ArgumentException("Usuario é nulo.");
                }

                // Valida o modelo de dados
                if (ModelState.IsValid)
                {
                    // Chama o repositório para atualizar os dados do usuário
                    await _usuarioRepositorio.AtualizarUsuarioAsync(usuarioSemSenha);
                    TempData["MensagemSucesso"] = "Os dados foram atualizados com sucesso!";
                    return RedirectToAction("AtualizarUsuario", new { id = usuarioSemSenha.Id });
                }

                return View(usuarioSemSenha); // Retorna à view com os erros de validação
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar atualizar dados do usuário.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao tentar atualizar dados do usuário, tente novamente.";
                }

                return RedirectToAction("AtualizarUsuario", new { id = usuarioSemSenha.Id });
            }
        }

        // Ação POST para remover o usuário
        [HttpPost]
        [ValidateAntiForgeryToken]  // Valida o Token Anti-Forgery
        public async Task<IActionResult> RemoverUsuario(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Id inválido.");
                }

                // Chama o repositório para remover o usuário
                var usuarioRemovido = await _usuarioRepositorio.RemoverUsuario(id);

                // Remove a sessão do usuário
                _sessao.RemoverSessaoUsuario();
                TempData["MensagemSucesso"] = "Conta encerrada com sucesso!";
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar remover usuário.");

                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["MensagemErro"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao tentar remover usuário, tente novamente.";
                }

                return RedirectToAction("AtualizarUsuario", new { id = id });
            }
        }
    }
}
