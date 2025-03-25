// O script é responsável pelo metodo de Adicionar e Remover o Aluno do Curso sem redirecionar toda a página
// Quando efetuado com sucesso o botão some, sinalizando que a ação foi executada.

document.addEventListener("DOMContentLoaded", function () {
    // Recupera todos os botões com a classe '.btn-aluno-acao' e adiciona um ouvinte de evento para o clique
    document.querySelectorAll(".btn-aluno-acao").forEach(button => {
        button.addEventListener("click", function () {
            // Recupera os valores dos atributos personalizados do botão (alunoId, cursoId e tipo)
            let alunoId = this.getAttribute("data-aluno-id");
            let cursoId = this.getAttribute("data-curso-id");
            let tipo = this.getAttribute("data-tipo");

            // Define a URL para a requisição com base no tipo da ação (adicionar ou remover)
            let url = tipo === "adicionar"
                ? "/AlunoCurso/AddAlunoAoCurso" // URL para adicionar aluno ao curso
                : "/AlunoCurso/RemoverAlunoDoCurso"; // URL para remover aluno do curso

            // Realiza a requisição via fetch, enviando os dados do aluno e curso no corpo da requisição
            fetch(url, {
                method: "POST", // Método POST para envio de dados
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded", // Tipo de conteúdo da requisição
                    "X-Requested-With": "XMLHttpRequest" // Cabeçalho para indicar que é uma requisição AJAX
                },
                body: `alunoId=${alunoId}&cursoId=${cursoId}` // Dados enviados no corpo da requisição
            })
                .then(response => {
                    // Se a resposta for bem-sucedida, remove o botão da interface
                    if (response.ok) {
                        button.remove(); // Remove o botão após a ação ser bem-sucedida
                    }
                    else {
                        // Caso a resposta não seja bem-sucedida, redireciona para a mesma página
                        window.location.href = window.location.href; // Redireciona para a mesma página
                    }
                })
                .catch(error => {
                    // Em caso de erro na requisição, redireciona para a mesma página
                    window.location.href = window.location.href; // Redireciona para a mesma página
                });
        });
    });
});
