// Barra de Pesquisa exibida na NavBar, responsável por pesquisar cursos, exibir um dropdown com a lista de cursos encontrados
// Redireciona para uma view que mostra os detalhes do curso em que o usuário selecionou

// Aguarda até que o conteúdo da página (DOM) seja completamente carregado
document.addEventListener("DOMContentLoaded", function () {
    // Recupera o campo de input onde o usuário vai digitar o termo de pesquisa
    const searchInput = document.getElementById("searchInput");

    // Recupera o dropdown onde as sugestões de busca serão mostradas
    const searchDropdown = document.getElementById("searchDropdown");

    // Adiciona um ouvinte de evento para a digitação no campo de pesquisa
    searchInput.addEventListener("keyup", function () {
        // Obtém o valor do campo de pesquisa e remove espaços extras nas extremidades
        let searchTerm = searchInput.value.trim();

        // Verifica se o termo de pesquisa tem pelo menos 1 caractere (ajustado para 3 no comentário original)
        if (searchTerm.length > 0) {
            // Faz uma requisição ao servidor para buscar sugestões com base no termo digitado
            fetch(`/Home/BarraDePesquisa?termo=${searchTerm}`)
                .then(response => response.json()) // Converte a resposta para JSON
                .then(data => {
                    // Limpa qualquer conteúdo existente no dropdown
                    searchDropdown.innerHTML = "";

                    // Verifica se existem resultados de pesquisa
                    if (data.length > 0) {
                        // Para cada item nos dados retornados, cria um novo link para cada sugestão
                        data.forEach(item => {
                            let option = document.createElement("a");
                            option.href = `/Curso/DetalhesCurso/${item.id}`; // Define o link para a página de detalhes do curso
                            option.classList.add("dropdown-item"); // Adiciona a classe de item do dropdown para estilização
                            option.textContent = item.nome; // Define o texto da sugestão com o nome do curso
                            searchDropdown.appendChild(option); // Adiciona o novo item no dropdown
                        });
                    } else {
                        // Se não houver resultados, exibe a mensagem de "Nenhum resultado encontrado"
                        let noResultsOption = document.createElement("a");
                        noResultsOption.classList.add("dropdown-item", "text-muted");
                        noResultsOption.textContent = "Nenhum resultado encontrado";
                        searchDropdown.appendChild(noResultsOption);
                    }

                    // Exibe o dropdown
                    searchDropdown.style.display = "block";
                })
                .catch(error => console.error("Erro ao buscar sugestões:", error)); // Caso ocorra um erro na requisição, exibe no console
        } else {
            // Se o termo de pesquisa for vazio ou muito curto, oculta o dropdown
            searchDropdown.style.display = "none";
        }
    });

    // Adiciona um ouvinte para o evento de clique no documento
    // Esse código serve para ocultar o dropdown caso o usuário clique fora do campo de pesquisa ou do próprio dropdown
    document.addEventListener("click", function (event) {
        if (!searchInput.contains(event.target) && !searchDropdown.contains(event.target)) {
            searchDropdown.style.display = "none"; // Esconde o dropdown
        }
    });
});
