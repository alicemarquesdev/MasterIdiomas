# Master Idiomas

 [Veja a aplicação em ação](mailto:alicemarques.dev@hotmail.com)

Master Idiomas é um sistema de gerenciamento para escolas de idiomas, desenvolvido em ASP.NET Core MVC. 
A plataforma permite o gerenciamento eficiente de cursos, professores, alunos e usuários, oferecendo 
funcionalidades essenciais para a administração acadêmica e operacional.
O sistema fornece uma interface intuitiva para cadastro, edição, exclusão e visualização de dados, 
além de contar com recursos de segurança e controle de acesso.

## Principais Funcionalidades
- **Gerenciamento CRUD Completo**: Permite o gerenciamento de cursos, professores, alunos e usuários, com validações de dados e relacionamentos entre os registros.
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

## Tecnologias Usadas

### **Back-End**
C# | ASP.NET Core MVC | Entity Framework Core | SQL Server

### **Front-End**
HTML | CSS | Bootstrap | jQuery | JavaScript

## Instalação

### **Pré-Requisitos**

Antes de rodar o projeto, é necessário ter as seguintes ferramentas instaladas:

- **Visual Studio 2022 ou superior** com o suporte para **ASP.NET Core e .NET 8.0**.
- **.NET SDK 8.0** (necessário para compilar o projeto).
- **SQL Server** (necessário para o banco de dados relacional).
- Compátivel com **Windows** | **macOS** | **Linux**.



### **Passo a Passo para Executar o Projeto Localmente**

1. **Clonar o Repositório**  
   Primeiro, você precisa clonar o repositório do projeto para sua máquina local. Utilize o Git para isso:

   ```bash
   git clone https://github.com/alicemarquesdev/MasterIdiomas.git
   ```

2. **Instalar as Dependências do Projeto**
O projeto utiliza pacotes do NuGet que são gerenciados pelo Visual Studio.

MailKit (4.11.0) | Microsoft.EntityFrameworkCore (9.0.3) | Microsoft.EntityFrameworkCore.Design (9.0.3) | Microsoft.EntityFrameworkCore.SqlServer (9.0.3) | 
Microsoft.EntityFrameworkCore.Tools (9.0.3) | Newtonsoft.Json (13.0.3) | Serilog.AspNetCore (9.0.0) | Serilog.Sinks.Console (6.0.0) | Serilog.Sinks.File (6.0.0)

3. *Configuração appsettings*
Verifique se você possui um arquivo appsettings.json com as configurações corretas para o banco de dados e outras variáveis.
Certifique-se de ter o SQL Server instalado e configurado. Crie um banco de dados para o projeto e configure a string de conexão no arquivo.
Configure também as credenciais SMTP.
Exemplo de configuração:


Exemplo de string de conexão:

```bash
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.File" ],
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "Path": "logs/app.log",
          "RollingInterval": "Day",
          "RetainedFileCountLimit": 7,
          "FileSizeLimitBytes": 10485760,
          "Buffered": true
        }
      }
    ]
  },
"ConnectionStrings": {
    "DataBase": "Server=localhost;Database=NomeDoBanco;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
},
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com", // utilizando gmail
    "SmtpPort": 587,
    "SenderEmail": "seuemail@dominio.com",
    "SenderPassword": "suasenha"
  },
  "AllowedHosts": "*"
}
```

4. **Aplicar as Migrations**
Aplicar as migrations para criar o esquema do banco de dados. Execute o seguinte comando no Package Manager Console ou Terminal:

```bash
dotnet ef database update
```

5. **Executar o Projeto**
Clique em Run ou Iniciar sem Depuração (F5) para rodar o servidor localmente. O projeto será executado no navegador padrão.

6. **Verificação**
Após a execução, o projeto estará disponível em http://localhost:5000 (ou a porta configurada no launchSettings.json). 
Verifique se o sistema está funcionando conforme esperado.


## Demonstração

- Link do Site Hospedado:  [Veja a aplicação em ação](mailto:alicemarques.dev@hotmail.com)

## Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE.md](LICENSE.md) para detalhes.

MIT License

Copyright (c) 2025 Alice Oliveira Marques

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Contato

Alice Marques - [alicemarques.dev@hotmail.com](mailto:alicemarques.dev@hotmail.com)

Link Projeto: https://github.com/alicemarquesdev/MasterIdiomas.git




