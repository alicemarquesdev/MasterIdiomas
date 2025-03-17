// Usado para abrir um modal para adicionar professor ao curso

// Aguarda até que o conteúdo da página (DOM) seja completamente carregado
document.addEventListener("DOMContentLoaded", function () {
    // Verifica se o modal de adicionar professor está presente no HTML
    const modalAddProfessor = document.getElementById('modalAddProfessor');

    // Se o modal estiver presente, adiciona um ouvinte de evento para quando o modal for exibido
    if (modalAddProfessor) {
        modalAddProfessor.addEventListener('show.bs.modal', function (event) {
            // Quando o modal for mostrado, pega o botão que foi clicado para abrir o modal
            const button = event.relatedTarget;

            // Obtém o valor do atributo 'data-cursoid' do botão, que representa o ID do curso
            const cursoId = button.getAttribute('data-cursoid');

            // Encontra o campo de input escondido (hidden) que irá armazenar o ID do curso
            // Preenche o campo com o ID do curso que foi obtido do botão
            document.getElementById('cursoIdInput').value = cursoId;
        });
    }
});
