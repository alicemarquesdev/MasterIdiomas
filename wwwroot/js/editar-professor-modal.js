// Este script preenche os campos do modal de edição de professor com as informações do professor
// ao clicar no botão de editar. Ele recupera os dados de cada professor e os exibe no formulário
// de atualização do modal, permitindo que os dados sejam editados.

// Usado em views que exibem listas de professores.

var atualizarProfessorModal = document.getElementById('modalAtualizarProfessor');

// Adiciona um ouvinte de evento para o momento em que o modal for aberto (evento "show.bs.modal")
atualizarProfessorModal.addEventListener('show.bs.modal', function (event) {
    // Obtém o botão que acionou o modal, usando o objeto `event` que contém o "relatedTarget" (o elemento que acionou o modal)
    var button = event.relatedTarget;

    // Recupera os atributos personalizados do botão acionador do modal. Esses atributos contêm as informações do professor que precisam ser preenchidas no formulário.
    var professorId = button.getAttribute('data-professor-id');
    var nome = button.getAttribute('data-nome');
    var email = button.getAttribute('data-email');
    var genero = button.getAttribute('data-genero');
    var dataNascimento = button.getAttribute('data-data-nascimento');
    var status = button.getAttribute('data-status');

    // Preenche os campos do modal com os valores recuperados dos atributos do botão.
    // Os IDs dos campos do modal (ProfessorId, Nome, Email, Genero, DataNascimento, Status) 
    // correspondem aos elementos de entrada no modal de edição.
    document.getElementById('ProfessorId').value = professorId;
    document.getElementById('Nome').value = nome;
    document.getElementById('Email').value = email;
    document.getElementById('Genero').value = genero;
    document.getElementById('DataNascimento').value = dataNascimento;
    document.getElementById('Status').value = status;
});
