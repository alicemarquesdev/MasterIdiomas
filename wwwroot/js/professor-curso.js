document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".btn-professor-acao").forEach(button => {
        button.addEventListener("click", function () {
            let professorId = this.getAttribute("data-professor-id");
            let cursoId = this.getAttribute("data-curso-id");
            let tipo = this.getAttribute("data-tipo");

            // Define a URL com base no tipo (adicionar ou remover)
            let url = tipo === "adicionar"
                ? "/ProfessorCurso/AddProfessorAoCurso"
                : "/ProfessorCurso/RemoverProfessorDoCurso";

            fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: `professorId=${professorId}&cursoId=${cursoId}`
            })
                .then(response => {
                    if (response.ok) {
                        button.remove(); // Remove o botão após a ação ser bem-sucedida
                    }
                    else {
                        window.location.href = window.location.href; // Redireciona para a mesma página
                    }
                })
               
                .catch(error => {
                    window.location.href = window.location.href; // Redireciona para a mesma página
                });
        });
    });
});
