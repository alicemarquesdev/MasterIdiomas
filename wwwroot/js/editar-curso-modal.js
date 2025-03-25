// Este script preenche os campos do modal de edição de curso com as informações do curso
// ao clicar no botão de editar. Ele recupera os dados de cada curso e os exibe no formulário
// de atualização do modal, permitindo que os dados sejam editados.

// Usado em views que exibem listas de cursos.

var editarCursoModal = document.getElementById('modalEditarCurso');

// Adiciona um ouvinte de evento para o momento em que o modal for aberto (evento "show.bs.modal")
editarCursoModal.addEventListener('show.bs.modal', function (event) {
    // Obtém o botão que acionou o modal, usando o objeto `event` que contém o "relatedTarget" (o elemento que acionou o modal)
    var button = event.relatedTarget;

    // Recupera os atributos personalizados do botão acionador do modal. Esses atributos contêm as informações do curso que precisam ser preenchidas no formulário.
    var cursoId = button.getAttribute('data-curso-id');
    var idioma = button.getAttribute('data-idioma');
    var turno = button.getAttribute('data-turno');
    var nivel = button.getAttribute('data-nivel');
    var dataInicio = button.getAttribute('data-data-inicio');
    var cargaHoraria = button.getAttribute('data-carga-horaria');
    var maxAlunos = button.getAttribute('data-max-alunos');
    var status = button.getAttribute('data-status');

    // Preenche os campos do modal com os valores recuperados dos atributos do botão.
    // Os IDs dos campos do modal (CursoId, Idioma, Turno, Nivel, DataInicio, CargaHoraria, MaxAlunos, Status) 
    // correspondem aos elementos de entrada no modal de edição.
    document.getElementById('CursoId').value = cursoId;
    document.getElementById('Idioma').value = idioma;
    document.getElementById('Turno').value = turno;
    document.getElementById('Nivel').value = nivel;
    document.getElementById('DataInicio').value = dataInicio;
    document.getElementById('CargaHoraria').value = cargaHoraria;
    document.getElementById('MaxAlunos').value = maxAlunos;
    document.getElementById('Status').value = status;

    // Validações para os campos
    validarCampos();
});
