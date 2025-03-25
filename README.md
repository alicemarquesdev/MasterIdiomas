# Master Idiomas

Master Idiomas é um sistema de gerenciamento para escolas de idiomas, desenvolvido em ASP.NET Core MVC. 
A plataforma permite o gerenciamento eficiente de cursos, professores, alunos e usuários, oferecendo 
funcionalidades essenciais para a administração acadêmica e operacional.
O sistema fornece uma interface intuitiva para cadastro, edição, exclusão e visualização de dados, 
além de contar com recursos de segurança e controle de acesso.

---

## Principais Funcionalidades
- Gerenciamento CRUD completo de cursos, professores, alunos e usuários, com validações de dados e relacionamentos entre registros.
- **Sistema de Login**: Sistema de login seguro, com autenticação baseada em cookies e verificação de identidade, garantindo acesso autorizado ao sistema.
- **Redefinição de Senha via Email (SMTP)**: Envio de email com nova senha do usuário. 
- **Alteração de Senha**: Permite a alteração de senha pelo usuário, com verificação de senha atual e novas senhas que atendem a critérios de segurança.
- **Filtros de Autorização**: Controle de acesso utilizando filtros de autorização, garantindo que apenas usuários com permissões adequadas possam acessar determinadas páginas.
- **Segurança**: Segurança robusta contra ataques comuns como SQL Injection, XSS e CSRF, utilizando Razor, tokens anti-forgery,
 e LINQ com Entity Framework para consultas seguras. Validação de entradas nos formulários e restrições para garantir a segurança dos dados
- **Entity Framework ORM**: Utilização do Entity Framework Core como ORM para interagir com o banco de dados SQL Server, aproveitando migrations automáticas para facilitar a evolução do esquema de dados.
- **Proteção de Senha**: Criptografia de senhas, utilizando o algoritmo SHA-1.
- **Sessão do Usuário**: Gerenciamento da sessão do usuário com armazenamento seguro em cookies, garantindo que o usuário permaneça autenticado durante a navegação.
- **Logs de Atividade do Usuário**: Logs de atividades do usuário registrados com Serilog, para monitoramento e diagnóstico

---


## Tecnologias Usadas

- C# / ASP.NET Core
- Banco de Dados SQL Server
- HTML, CSS, JavaScript, Bootstrap
- Outras tecnologias, bibliotecas ou frameworks

## Como Rodar Localmente

1. Clone este repositório:
    ```bash
    git clone https://github.com/<username>/<repo-name>.git
    ```

2. Navegue até a pasta do projeto:
    ```bash
    cd <repo-name>
    ```

3. Configure as variáveis de ambiente ou crie um arquivo `appsettings.json` (se necessário).
   
4. Instale as dependências:
    ```bash
    dotnet restore
    ```

5. Rode o projeto:
    ```bash
    dotnet run
    ```

## Demonstração

Se você hospedou o site, forneça o link de acesso aqui.

## Licença

Se você quiser, adicione uma licença para o seu projeto. Caso não saiba qual escolher, a **MIT** é uma boa opção para projetos públicos.

