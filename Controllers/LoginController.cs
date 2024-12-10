﻿using MasterIdiomas.Helper;
using MasterIdiomas.Models;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MasterIdiomas.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ISessao _sessao;
        private readonly IEmail _email;

        public LoginController(IUsuarioRepositorio usuarioRepositorio,
                                ISessao sessao,
                                IEmail email)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
            _email = email;
        }

        public IActionResult CriarConta()
        {
            return View();
        }

        public IActionResult Index()
        {
            var usuario = _sessao.BuscarSessaoUsuario();

            if (usuario != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public IActionResult RedefinirSenha()
        {
            return View();
        }

        public IActionResult Sair()
        {
            _sessao.RemoverSessaoUsuario();
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> CriarConta(UsuarioModel usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _usuarioRepositorio.AddUsuarioAsync(usuario);
                    TempData["MensagemSucesso"] = "A conta foi criada com sucesso! Agora você pode efetuar o login para acessar sua conta.";

                    return RedirectToAction("Index", "Login");
                }
                TempData["MensagemErro"] = "Por favor, verifique os dados inseridos.";
                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"{ex.Message}";
                return View(usuario);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Entrar(LoginModel loginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioModel usuario = await _usuarioRepositorio.BuscarUsuarioExistenteAsync(loginModel.Email);

                    if (usuario != null)
                    {
                        if (usuario.SenhaValida(loginModel.Senha))
                        {
                            _sessao.CriarSessaoUsuario(usuario);

                            return RedirectToAction("Index", "Home");
                        }
                        TempData["MensagemErro"] = $"Senha do usuário é inválida, tente novamente.";
                    }
                    TempData["MensagemErro"] = $"Ops, não conseguimos realizar seu login, tente novamante.";
                }
                return View("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos realizar seu login, tente novamante, detalhe do erro: {erro.Message}";
                return View("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> LinkRedefinirSenha(RedefinirSenhaModel redefinirSenha)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioModel usuario = await _usuarioRepositorio.BuscarUsuarioExistenteAsync(redefinirSenha.Email);

                    if (usuario != null)
                    {
                        string novaSenha = usuario.GerarNovaSenha();
                        string mensagem = $"Sua nova senha é {novaSenha}";

                        bool emailEnviado = _email.Enviar(usuario.Email, "Master Idiomas - Nova Senha", mensagem);

                        if (emailEnviado)
                        {
                            await _usuarioRepositorio.AtualizarUsuarioAsync(usuario);
                            TempData["MensagemSucesso"] = $"Enviamos para seu e-mail cadastrado uma nova senha.";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["MensagemErro"] = $"Não conseguimos enviar o e-mail. Por favor, tente novamente.";
                        }

                        return View("RedefinirSenha", redefinirSenha);
                    }

                    TempData["MensagemErro"] = $"Usuário não encontrado. Por favor, verifique os dados informados.";
                }

                TempData["MensagemErro"] = $"Não conseguimos redefinir sua senha. Por favor, verifique os dados informados.";
                return View("RedefinirSenha", redefinirSenha);
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Desculpe, ocorreu um erro: {erro.Message}";
                return View("RedefinirSenha", redefinirSenha);
            }
        }
    }
}