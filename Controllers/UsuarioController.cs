using MasterIdiomas.Filters;
using MasterIdiomas.Helper;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    [UsuarioLogado]
    public class UsuarioController : Controller
    {
        // Dependências injetadas no controlador
        private readonly ISessao _sessao;

        private readonly IUsuarioRepositorio _usuarioRepositorio;

        // Construtor que recebe as dependências
        public UsuarioController(ISessao sessao,
                                 IUsuarioRepositorio usuarioRepositorio)
        {
            _sessao = sessao;
            _usuarioRepositorio = usuarioRepositorio;
        }

        public async Task<IActionResult> RemoverUsuario(int id)
        {
            // Recupera o usuário do repositório pelo ID
            UsuarioModel usuario = await _usuarioRepositorio.BuscarUsuarioPorIdAsync(id);

            if (usuario == null)
            {
                TempData["MensagemErro"] = "Usuário não encontrado.";
                return RedirectToAction("Index", "Home");
            }

            return View(usuario);
        }

        // Ação para exibir o formulário de alteração de senha
        public async Task<IActionResult> AlterarSenha(int id)
        {
            // Recupera o usuário do repositório pelo ID
            UsuarioModel usuario = await _usuarioRepositorio.BuscarUsuarioPorIdAsync(id);

            if (usuario == null)
            {
                TempData["MensagemErro"] = "Usuário não encontrado.";
                return RedirectToAction("Index", "Home");
            }

            // Cria o modelo AlterarSenhaModel com base no usuário encontrado
            AlterarSenhaModel alterarSenhaModel = new AlterarSenhaModel
            {
                Id = usuario.UsuarioId
            };

            return View(alterarSenhaModel);
        }

        // Ação para exibir o formulário de atualização de dados do usuário
        public async Task<IActionResult> AtualizarUsuario(int id)
        {
            // Recupera o usuário do repositório pelo ID
            UsuarioModel usuario = await _usuarioRepositorio.BuscarUsuarioPorIdAsync(id);

            if (usuario == null)
            {
                TempData["MensagemErro"] = "Usuário não encontrado.";
                return RedirectToAction("Index", "Home");
            }

            // Converte UsuarioModel para UsuarioSemSenhaModel
            UsuarioSemSenhaModel usuarioSemSenha = new UsuarioSemSenhaModel
            {
                Id = usuario.UsuarioId,
                Nome = usuario.Nome,
                Email = usuario.Email
            };

            return View(usuarioSemSenha); // Passa o modelo correto para a view
        }

        // Ação POST para alterar a senha do usuário
        [HttpPost]
        public async Task<IActionResult> AlterarSenha(AlterarSenhaModel alterarSenhaModel)
        {
            try
            {
                // Recupera o usuário logado da sessão
                UsuarioModel usuario = _sessao.BuscarSessaoUsuario();

                // Verifica se a sessão do usuário está válida
                if (usuario == null)
                {
                    TempData["MensagemErro"] = "Sessão do usuário não encontrada.";
                    return RedirectToAction("Index", "Home");
                    // Redireciona para o Index se o usuário não estiver logado
                }

                // Recupera o usuário do banco de dados
                UsuarioModel usuarioDb = await _usuarioRepositorio.BuscarUsuarioPorIdAsync(usuario.UsuarioId);

                if (usuarioDb == null)
                {
                    TempData["MensagemErro"] = "Usuário não encontrado.";
                    return RedirectToAction("Index", "Home"); // Redireciona para o Index se o usuário não for encontrado
                }

                // Define o ID do usuário logado no modelo de alteração de senha
                alterarSenhaModel.Id = usuarioDb.UsuarioId;

                // Verifica se o modelo é válido
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Chama o método para alterar a senha no repositório
                        await _usuarioRepositorio.AlterarSenhaAsync(alterarSenhaModel);

                        TempData["MensagemSucesso"] = "Senha alterada com sucesso!";
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        TempData["MensagemErro"] = ex.Message; // Mensagem de erro detalhada
                    }
                }
                else
                {
                    TempData["MensagemErro"] = "Os dados fornecidos são inválidos.";
                }

                // Retorna a view AlterarSenha com os erros de validação ou falhas
                return View(alterarSenhaModel);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Ocorreu um erro inesperado: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // Ação POST para atualizar os dados do usuário
        [HttpPost]
        public async Task<IActionResult> AtualizarUsuario(UsuarioSemSenhaModel usuarioSemSenha)
        {
            try
            {
                // Verifica se o modelo de dados está válido
                if (ModelState.IsValid)
                {
                    // Converte UsuarioSemSenhaModel para UsuarioModel
                    UsuarioModel usuarioModel = new UsuarioModel
                    {
                        UsuarioId = usuarioSemSenha.Id,
                        Nome = usuarioSemSenha.Nome,
                        Email = usuarioSemSenha.Email,
                    };

                    // Chama o repositório para atualizar os dados do usuário
                    await _usuarioRepositorio.AtualizarUsuarioAsync(usuarioModel);

                    // Mensagem de sucesso
                    TempData["MensagemSucesso"] = "Os dados foram atualizados com sucesso!";

                    // Buscar o usuário atualizado (para mostrar na view)
                    UsuarioSemSenhaModel usuarioAtualizado = new UsuarioSemSenhaModel
                    {
                        Id = usuarioModel.UsuarioId,
                        Nome = usuarioModel.Nome,
                        Email = usuarioModel.Email
                    };

                    // Retorna a view com o modelo atualizado
                    return View(usuarioAtualizado);
                }

                // Se os dados não estiverem válidos, retorna para a view com o modelo
                return View(usuarioSemSenha);
            }
            catch (Exception)
            {
                // Captura exceções e exibe mensagem de erro
                TempData["MensagemErro"] = "Desculpe. Não conseguimos atualizar seus dados, tente novamente.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoverUsuarioConfirmacao(int usuarioId)
        {
            try
            {
                _sessao.RemoverSessaoUsuario();
                await _usuarioRepositorio.RemoverUsuario(usuarioId);
                TempData["MensagemSucesso"] = "Conta encerrada com sucesso!";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Ocorreu um erro inesperado: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}