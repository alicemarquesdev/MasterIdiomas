using MasterIdiomas.Helper;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    // O LoginController é responsável por gerenciar as ações relacionadas ao login, criação de conta, redefinição de senha e logout no sistema.
    // Ele inclui métodos para exibir as páginas de login, criar uma conta, enviar e-mails para redefinição de senha e fazer o logout.

    // Métodos:

    // GET CriarConta() - Exibe a página para criar uma nova conta de usuário, se o usuário já estiver logado, redireciona para a página inicial.
    // GET Login() - Exibe a página de login para o usuário, redireciona para a página inicial caso o usuário já esteja logado.
    // GET RedefinirSenha() - Exibe a página para redefinir a senha do usuário, se o usuário já estiver logado, redireciona para a página inicial.
    // GET Sair() - Realiza o logout do usuário, removendo a sessão e redirecionando para a página de login.

    // POST CriarConta(UsuarioModel usuario) - Cria uma nova conta de usuário, caso os dados sejam válidos, redireciona para a página de login.
    // POST Entrar(LoginModel loginModel) - Realiza o login do usuário, verificando as credenciais inseridas.
    // POST EnviarLinkParaRedefinirSenha(string email) - Envia um e-mail com um link para redefinir a senha, caso o e-mail esteja registrado no sistema.

    public class LoginController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ISessao _sessao;
        private readonly IEmail _email;
        private readonly ILogger<LoginController> _logger;

        // O construtor recebe as dependências necessárias, e lança exceções caso qualquer uma delas seja nula.
        public LoginController(IUsuarioRepositorio usuarioRepositorio,
                                ISessao sessao,
                                IEmail email,
                                ILogger<LoginController> logger)
        {
            _usuarioRepositorio = usuarioRepositorio ?? throw new ArgumentNullException(nameof(usuarioRepositorio));
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao));
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Método GET para criar uma nova conta de usuário.
        public IActionResult CriarConta()
        {
            try
            {
                var usuario = _sessao.BuscarSessaoUsuario();

                if (usuario != null)
                {
                    return RedirectToAction("Index", "Home"); // Redireciona para a página inicial caso o usuário já esteja logado.
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar acessar a página de criar conta.");
                return View("Error", new { Message = "Ocorreu um erro inesperado ao acessar a página de criar conta." });
            }
        }

        // Método GET para exibir a página de login.
        public IActionResult Login()
        {
            try
            {
                var usuario = _sessao.BuscarSessaoUsuario();

                if (usuario != null)
                {
                    return RedirectToAction("Index", "Home"); // Redireciona para a página inicial caso o usuário já esteja logado.
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar acessar a página de login.");
                return View("Error", new { Message = "Ocorreu um erro inesperado ao acessar a página de login." });
            }
        }

        // Método GET para exibir a página de redefinir senha.
        public ActionResult RedefinirSenha()
        {
            try
            {
                var usuario = _sessao.BuscarSessaoUsuario();

                if (usuario != null)
                {
                    return RedirectToAction("Index", "Home"); // Redireciona para a página inicial caso o usuário já esteja logado.
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar acessar a página de redefinir senha.");
                return View("Error", new { Message = "Ocorreu um erro inesperado ao acessar a página de redefinir senha." });
            }
        }

        // Método GET para realizar o logout do usuário.
        public IActionResult Sair()
        {
            try
            {
                _sessao.RemoverSessaoUsuario(); // Remove a sessão do usuário, efetivamente fazendo logout.
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar realizar logout.");
                return RedirectToAction("Login", "Login");
            }
        }

        // Método POST para criar uma nova conta de usuário.
        [HttpPost]
        public async Task<IActionResult> CriarConta(UsuarioModel usuario)
        {
            try
            {
                if(usuario == null)
                {
                    throw new ArgumentNullException("Model usuário é null");
                }

                if (ModelState.IsValid)
                {
                    await _usuarioRepositorio.AddUsuarioAsync(usuario); // Cria um novo usuário no banco de dados.
                    TempData["MensagemSucesso"] = "A conta foi criada com sucesso! Agora você pode efetuar o login para acessar sua conta.";
                    return RedirectToAction("Login", "Login");
                }
                TempData["MensagemErro"] = "Por favor, verifique os dados inseridos."; // Exibe mensagem de erro caso os dados não sejam válidos.
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar criar uma conta.");
                TempData["MensagemErro"] = $"Ocorreu um erro: {ex.Message}";
                return View(usuario);
            }
        }

        // Método POST para realizar o login do usuário.
        [HttpPost]
        public async Task<IActionResult> Entrar(LoginModel loginModel)
        {
            try
            {
                if (loginModel == null)
                {
                    throw new ArgumentNullException("Model loginModel é null");
                }

                if (ModelState.IsValid)
                {
                    var usuario = await _usuarioRepositorio.VerificarUsuarioExistentePorEmailAsync(loginModel.Email);

                    if (usuario != null)
                    {
                        if (usuario.SenhaValida(loginModel.Senha))
                        {
                            _sessao.CriarSessaoUsuario(usuario); // Cria a sessão para o usuário logado.
                            return RedirectToAction("Index", "Home");
                        }
                        TempData["MensagemErro"] = "Senha do usuário é inválida, tente novamente.";
                    }
                    else
                    {
                        TempData["MensagemErro"] = "Email não existe.";
                    }
                }
                TempData["MensagemErro"] = "Verifique os dados inseridos"; // Exibe mensagem de erro caso as credenciais sejam inválidas.
                return View("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar realizar o login.");
                TempData["MensagemErro"] = $"Ops, não conseguimos realizar seu login, tente novamente. Detalhe do erro: {ex.Message}";
                return View("Login");
            }
        }

        // Método POST para enviar o link para redefinir a senha para o usuário.
        [HttpPost]
        public async Task<IActionResult> EnviarLinkParaRedefinirSenha(string email)
        {
            try
            {
                if(string.IsNullOrEmpty(email))
                {
                    throw new ArgumentNullException("Email é nulo");
                }

                var usuarioDb = await _usuarioRepositorio.VerificarUsuarioExistentePorEmailAsync(email);

                if (usuarioDb != null)
                {
                    string novaSenha = usuarioDb.GerarNovaSenha();

                    string mensagem = $"Olá {usuarioDb.Nome},<br><br>Sua nova senha é: <strong>{novaSenha}</strong><br><br>Altere sua senha assim que possível.";
                    var emailEnviado = await _email.EnviarEmailAsync(usuarioDb.Email, "Redefinição de Senha - MasterIdiomas", mensagem);

                    if (emailEnviado)
                    {
                        await _usuarioRepositorio.RedefinirSenha(usuarioDb.UsuarioId, novaSenha);
                        TempData["MensagemSucesso"] = "Enviamos para seu e-mail cadastrado uma nova senha.";
                    }
                    else
                    {
                        TempData["MensagemErro"] = "Não conseguimos enviar o e-mail. Por favor, tente novamente.";
                    }
                }
                else
                {
                    TempData["MensagemErro"] = "Os dados não são válidos, tente novamente.";
                }
                return RedirectToAction("RedefinirSenha", "Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar enviar o link para redefinir a senha.");
                TempData["MensagemErro"] = "Ocorreu um erro ao tentar redefinir sua senha. Tente novamente.";
                return RedirectToAction("RedefinirSenha", "Login");
            }
        }
    }
}
