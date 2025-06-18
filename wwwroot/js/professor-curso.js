// O script é responsável pelo metodo de Adicionar e Remover o Professor do Curso sem redirecionar toda a página
// Quando efetuado com sucesso o botão some, sinalizando que a ação foi executada.

document.addEventListener("DOMContentLoaded", function () {
    // Recupera todos os botões com a classe '.btn-professor-acao' e adiciona um ouvinte de evento para o clique
    document.querySelectorAll(".btn-professor-acao").forEach(button => {
        button.addEventListener("click", function () {
            // Recupera os valores dos atributos personalizados do botão (professorId, cursoId e tipo)
            let professorId = this.getAttribute("data-professor-id");
            let cursoId = this.getAttribute("data-curso-id");
            let tipo = this.getAttribute("data-tipo");

            // Define a URL para a requisição com base no tipo da ação (adicionar ou remover)
            let url = tipo === "adicionar"
                ? "/ProfessorCurso/AddProfessorAoCurso" // URL para adicionar professor ao curso
                : "/ProfessorCurso/RemoverProfessorDoCurso"; // URL para remover professor do curso

            // Realiza a requisição via fetch, enviando os dados do professor e curso no corpo da requisição
            fetch(url, {
                method: "POST", // Método POST para envio de dados
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded", // Tipo de conteúdo da requisição
                    "X-Requested-With": "XMLHttpRequest" // Cabeçalho para indicar que é uma requisição AJAX
                },
                body: `professorId=${professorId}&cursoId=${cursoId}` // Dados enviados no corpo da requisição
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
