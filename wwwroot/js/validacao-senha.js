// Ao criar o usuario, o script valida a senha e a confirmação de Senha

// Adiciona um evento de escuta ao formulário com o ID "formUsuarioSenha" para capturar o evento de envio (submit)
document.getElementById("formUsuarioSenha").addEventListener("submit", function (event) {

    // Obtém os valores dos campos de senha e confirmação de senha
    var senha = document.getElementById("Senha").value;
    var confirmarSenha = document.getElementById("ConfirmarSenha").value;

    // Obtém o elemento onde será exibida a mensagem de erro
    var errorMessage = document.getElementById("confirmarSenhaError");

    // Verifica se as senhas digitadas são diferentes
    if (senha !== confirmarSenha) {
        event.preventDefault();  // Impede o envio do formulário caso as senhas não coincidam
        errorMessage.textContent = "As senhas não correspondem."; // Exibe a mensagem de erro
        errorMessage.style.color = "red"; // Define a cor da mensagem de erro como vermelha
    } else {
        errorMessage.textContent = ""; // Limpa a mensagem de erro caso as senhas sejam iguais
    }
});
