// Este script lida com a abre um modal para exibir um select com a lista de professores para adicionar ao curso
// É usado em algumas views. O codigo lida também com o envio para o formulário.

document.addEventListener("DOMContentLoaded", function () {
    // Recupera o modal de adição de professor, utilizado para associar um professor a um curso
    const modalAddProfessor = document.getElementById('modalAddProfessor');

    // Verifica se o modal de adição de professor foi encontrado na página
    if (modalAddProfessor) {
        // Adiciona um ouvinte de evento para quando o modal for exibido (evento "show.bs.modal")
        modalAddProfessor.addEventListener('show.bs.modal', function (event) {
            // Obtém o botão que acionou o modal a partir do objeto 'event' e o 'relatedTarget'
            const button = event.relatedTarget;

            // Recupera o ID do curso do atributo personalizado do botão acionador do modal
            const cursoId = button.getAttribute('data-cursoid');

            // Preenche o campo de ID do curso no modal com o ID recuperado
            document.getElementById('cursoIdInput').value = cursoId;
        });
    }

    // Lógica para o envio do formulário de adição do professor ao curso
    const formElement = document.querySelector("#formAdicionarProfessor");

    // Verifica se o formulário de adição de professor foi encontrado na página
    if (formElement) {
        // Adiciona um ouvinte de evento para quando o formulário for enviado
        formElement.addEventListener("submit", function (event) {
            // Previne o envio padrão do formulário (para enviar via fetch)
            event.preventDefault();

            // Recupera os valores dos campos ProfessorId e cursoIdInput
            const professorId = document.querySelector("#ProfessorId").value;
            const cursoId = document.querySelector("#cursoIdInput").value;

            // Validação: se o professor não for selecionado, impede o envio do formulário
            if (!professorId) {
                return; // Não envia o formulário caso o campo ProfessorId esteja vazio
            }

            // Cria um objeto FormData com os dados do formulário
            const formData = new FormData(formElement);

            // Realiza o envio do formulário usando o método fetch
            fetch(formElement.action, {
                method: formElement.method,
                body: formData
            })
                .then(response => {
                    // Se a resposta da requisição não for bem-sucedida, lança um erro
                    if (!response.ok) {
                        throw new Error('Erro na requisição');
                    }

                    // Caso a requisição seja bem-sucedida, recarrega a página atual
                    window.location.href = window.location.href;
                })
                .catch(error => {
                    // Caso ocorra um erro, recarrega a página atual
                    window.location.href = window.location.href; // Redireciona para a mesma página em caso de erro
                });
        });
    }
});
