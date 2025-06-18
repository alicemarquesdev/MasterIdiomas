// Este script lida com a remoção de itens (alunos, professores, cursos, usuários, etc.). 
// Quando um item é selecionado para remoção, ele abre um modal com os dados daquele item,
// permitindo que o usuário confirme a ação. O tipo de item determina a ação específica do formulário.

document.addEventListener("DOMContentLoaded", function () {
    // Recupera o modal de remoção, que será utilizado para confirmar a exclusão de itens.
    const modalRemocao = document.getElementById("modalRemocao");

    // Verifica se o modal está presente na página
    if (modalRemocao) {
        // Adiciona um ouvinte de evento para quando o modal for aberto (evento "show.bs.modal")
        modalRemocao.addEventListener("show.bs.modal", function (event) {
            // Obtém o botão que acionou o modal, usando o objeto `event` que contém o "relatedTarget" (o elemento que acionou o modal)
            const button = event.relatedTarget;

            // Recupera os atributos personalizados do botão acionador do modal. Esses atributos contêm as informações do item que será removido.
            const id = button.getAttribute("data-id");
            const nome = button.getAttribute("data-nome");
            const tipo = button.getAttribute("data-tipo");
            const cursoId = button.getAttribute("data-curso-id");
            const form = modalRemocao.querySelector("form");

            // Verifica os valores recuperados dos atributos e imprime no console para debug
            console.log("Aluno ID:", id);
            console.log("Nome:", nome);
            console.log("Tipo:", tipo);
            console.log("Curso ID:", cursoId);

            // Atualiza os campos do modal com os dados recuperados
            document.getElementById("itemNome").textContent = nome; // Exibe o nome do item que será removido
            document.getElementById("itemId").value = id; // Define o valor do ID para remoção

            // Se o tipo for relacionado a curso, aluno ou professor, inclui o cursoId no formulário
            if (tipo === "removerAlunoDoCurso" || tipo === "removerProfessorDoCurso") {
                document.getElementById("cursoId").value = cursoId; // Atualiza o campo do cursoId no caso de remoção de aluno ou professor de curso
            }

            // Define a ação do formulário dependendo do tipo do item que será removido
            switch (tipo) {
                case "curso":
                    form.action = "/Curso/RemoverCurso"; // Remove curso
                    break;
                case "professor":
                    form.action = "/Professor/RemoverProfessor"; // Remove professor
                    break;
                case "aluno":
                    form.action = "/Aluno/RemoverAluno"; // Remove aluno
                    break;
                case "removerAlunoDoCurso":
                    form.action = "/AlunoCurso/RemoverAlunoDoCurso"; // Remove aluno de curso
                    break;
                case "removerProfessorDoCurso":
                    form.action = "/ProfessorCurso/RemoverProfessorDoCurso"; // Remove professor de curso
                    break;
                case "usuario":
                    form.action = "/Usuario/RemoverUsuario"; // Remove usuário
                    break;
            }
        });
    }
});
