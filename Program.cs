using MasterIdiomas.Data;
using MasterIdiomas.Helper;
using MasterIdiomas.Repositorio;
using MasterIdiomas.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MasterIdiomas
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adicionar servi�os � aplica��o
            ConfigureServices(builder);

            var app = builder.Build();

            // Configurar o pipeline de requisi��o HTTP
            ConfigureMiddleware(app);

            // Migra��o autom�tica do banco de dados
            ApplyDatabaseMigrations(app);

            app.Run();
        }

        // M�todo para configurar os servi�os
        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Adicionar o IHttpContextAccessor, sess�es do usu�rio
            builder.Services.AddHttpContextAccessor();

            // Configurar se��es de configura��o (EmailSettings)
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            // Configura��o de Controllers e Views
            builder.Services.AddControllersWithViews();

            // Configurar o contexto de banco de dados
            builder.Services.AddDbContext<BancoContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DataBase")));

            // Configura��o de Reposit�rios
            RegisterRepositories(builder);

            // Configura��o de session
            builder.Services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromMinutes(50);
                o.Cookie.HttpOnly = true;
                o.Cookie.IsEssential = true;
            });

            // Adicionar compress�o de resposta
            builder.Services.AddResponseCompression();
        }

        // M�todo para registrar reposit�rios de forma mais limpa
        private static void RegisterRepositories(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAlunoRepositorio, AlunoRepositorio>();
            builder.Services.AddScoped<ISessao, Sessao>();
            builder.Services.AddScoped<IProfessorRepositorio, ProfessorRepositorio>();
            builder.Services.AddScoped<ICursoRepositorio, CursoRepositorio>();
            builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
            builder.Services.AddScoped<IEmail, Email>();
            builder.Services.AddScoped<IAlunoCursoRepositorio, AlunoCursoRepositorio>();
            builder.Services.AddScoped<IProfessorCursoRepositorio, ProfessorCursoRepositorio>();
            builder.Services.AddScoped<IdiomasSettings>();
        }

        // M�todo para configurar o middleware
        private static void ConfigureMiddleware(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            // Adicionar compress�o de resposta
            app.UseResponseCompression();

            // Roteamento da aplica��o
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Login}/{id?}");
        }

        // M�todo para aplicar migra��es autom�ticas no banco de dados
        private static void ApplyDatabaseMigrations(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BancoContext>();
                dbContext.Database.Migrate(); // Aplica a migra��o automaticamente no banco de dados
            }
        }
    }
}
