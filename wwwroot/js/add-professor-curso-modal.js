document.addEventListener("DOMContentLoaded", function () {
    const modalAddProfessor = document.getElementById('modalAddProfessor');

    // Quando o modal for exibido, preenche o campo de ID do curso
    if (modalAddProfessor) {
        modalAddProfessor.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;
            const cursoId = button.getAttribute('data-cursoid');
            document.getElementById('cursoIdInput').value = cursoId;
        });
    }

    // Lógica para o envio do formulário tradicional
    const formElement = document.querySelector("#formAdicionarProfessor");

    if (formElement) {
        formElement.addEventListener("submit", function (event) {
            event.preventDefault(); // Previne o envio padrão do formulário

            const professorId = document.querySelector("#ProfessorId").value;
            const cursoId = document.querySelector("#cursoIdInput").value;

            // Validação: verifica se o professor foi selecionado
            if (!professorId) {
                return; // Impede o envio do formulário caso o professor não seja selecionado
            }

            const formData = new FormData(formElement);

            // Realiza o envio via fetch
            fetch(formElement.action, {
                method: formElement.method,
                body: formData
            })
                .then(response => {
                    // Se a requisição não for bem-sucedida, redireciona para a mesma página
                    if (!response.ok) {
                        throw new Error('Erro na requisição');
                    }

                    // Caso o fetch seja bem-sucedido, apenas redireciona
                    window.location.href = window.location.href;
                })
                .then(() => {
                    window.location.href = window.location.href;
                })
                .catch(error => {
                    window.location.href = window.location.href; // Redireciona para a mesma página
                });
        });
    }
});
