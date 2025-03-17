// Este script é responsável por preencher os campos do modal de edição de aluno com os dados do aluno
// ao clicar no botão de editar. Ele recupera as informações de cada aluno e as exibe no formulário
// de atualização do modal, permitindo que os dados sejam editados.

// Usado em views que exibem listas de alunos.

// Recupera o modal de edição de aluno, que será aberto para edição de informações
var modalEditarAluno = document.getElementById('modalEditarAluno');

// Adiciona um ouvinte de evento para o momento em que o modal for aberto (evento "show.bs.modal")
modalEditarAluno.addEventListener('show.bs.modal', function (event) {
    // Obtém o botão que acionou o modal, usando o objeto `event` que contém o "relatedTarget" (o elemento que acionou o modal)
    var button = event.relatedTarget;

    // Recupera os atributos personalizados do botão acionador do modal. Esses atributos contêm as informações do aluno que precisam ser preenchidas no formulário.
    var alunoId = button.getAttribute('data-aluno-id');
    var nome = button.getAttribute('data-nome');
    var dataNascimento = button.getAttribute('data-data-nascimento');
    var genero = button.getAttribute('data-genero');
    var status = button.getAttribute('data-status');

    // Preenche os campos do modal com os valores recuperados dos atributos do botão.
    // Os IDs dos campos do modal (AlunoId, Nome, DataNascimento, Genero, Status) correspondem aos elementos de entrada no modal de edição.
    document.getElementById('AlunoId').value = alunoId;
    document.getElementById('Nome').value = nome;
    document.getElementById('DataNascimento').value = dataNascimento;
    document.getElementById('Genero').value = genero;
    document.getElementById('Status').value = status;
});
