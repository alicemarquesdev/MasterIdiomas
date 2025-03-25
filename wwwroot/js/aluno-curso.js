document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".btn-aluno-acao").forEach(button => {
        button.addEventListener("click", function () {
            let alunoId = this.getAttribute("data-aluno-id");
            let cursoId = this.getAttribute("data-curso-id");
            let tipo = this.getAttribute("data-tipo");

            // Define a URL com base no tipo (adicionar ou remover)
            let url = tipo === "adicionar"
                ? "/AlunoCurso/AddAlunoAoCurso"
                : "/AlunoCurso/RemoverAlunoDoCurso";

            fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: `alunoId=${alunoId}&cursoId=${cursoId}`
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
