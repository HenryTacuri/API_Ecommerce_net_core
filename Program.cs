
//Se crea un builder para crear y configuar nuestra appp web, args son los argumentos que se van resivir desde la linea de comandos
//al iniciar nuestra aplicacion en caso de ser necesario.
using APIEcommerce.Constants;
using APIEcommerce.Data;
using APIEcommerce.Models;
using APIEcommerce.Repository;
using APIEcommerce.Repository.IRepository;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


//Registro de los servicios.
//Se agregan los servicios según sea necesario.

// Add services to the container.

//Configuración de la conexion a la base de datos
string dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");
builder.Services.AddDbContext<ApplicationDbContext>(options => 

    options.UseSqlServer(dbConnectionString).UseSeeding((context, _) => {
        ApplicationDbContext appContext = (ApplicationDbContext) context;
        DataSeeder.seedData(appContext);
    })

);


//Configuración del servicio para la Caché
builder.Services.AddResponseCaching(options => {
    options.MaximumBodySize = 1024 * 1024; //1024 Bytes * 1024 Bytes = 1MB
    options.UseCaseSensitivePaths = true;
});


//Configuramos los repositorios, creamos una instancia del repositorio respectivo para cada solicitud de http
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

//Configuramos el AutoMapper
builder.Services.AddAutoMapper(cfg => {
    cfg.AddMaps(typeof(Program).Assembly);
});


//Registro del servicio de Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


//Servicio de autenticación
string secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");

if (string.IsNullOrEmpty(secretKey)) throw new InvalidOperationException("La SecretKey no esta configurada");

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.RequireHttpsMetadata = false; //Desactivamos la necesitad de utilizar https en producccion debe de ser true
    options.SaveToken = true; //Guarda el token en el contexto de autenticacion
    options.TokenValidationParameters = new TokenValidationParameters{ //Definicion de parametros
        ValidateIssuerSigningKey = true, //Se valida el token
        ///Que este firmado por una clave valida, se establece la clave secreta para validar la firma del token.
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false, //No se valida el emisor del token
        ValidateAudience = false, //No Se valida el publico del token si no se necesita restringir a ciertos clientes.
    };
});

// Agrega el servicio de controladores a la aplicacion ya que se utilizan controladores en la app.
builder.Services.AddControllers(options => {

    //Configuración perfiles de cache
    options.CacheProfiles.Add(CacheProfiles.Default10, CacheProfiles.Profile10);

    options.CacheProfiles.Add("Default20", new CacheProfile() {
        Duration = 20
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(
    options =>
    {
        //Añadimos un esquema de seguridad el cual se llama Bearer
        /*options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
            Description = "Nuestra API utiliza la Autenticación JWT usando el esquema Bearer. \n\r\n\r" +
                        "Ingresa la palabra a continuación el token generado en login.\n\r\n\r" +
                        "Ejemplo: \"12345abcdef\"",
            Name = "Authorization", //El token se envia a traves del encabezado Authorization
            In = ParameterLocation.Header, //Decimos que el token va a estar en la cabecera
            Type = SecuritySchemeType.Http, //Esquema de seguridad http
            Scheme = "Bearer" //Se hace uso del esquema bearer
        });*/

        options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Nuestra API utiliza la Autenticación JWT usando el esquema Bearer. \n\r\n\r" +
                        "Ingresa la palabra a continuación el token generado en login.\n\r\n\r" +
                        "Ejemplo: \"12345abcdef\""
        });

        //En los requerimientos le decimos a swagger como tiene actuar al tratar de acceder a los endpoints, en este caso se
        //dice que se necesita usar este esquema de seguridad.
        options.AddSecurityRequirement(document =>  new OpenApiSecurityRequirement
        {

            /*new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference
              {
                //Se hace referencia al esquema que hemos creado 
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2", //Esto es un requisito de swagger pero no lo hacemos uso.
              Name = "Bearer",
              In = ParameterLocation.Header
            },*/

            [new OpenApiSecuritySchemeReference("bearer", document)] = []
            //new List<string>() //Se agrega una lista vacia de scoops ya que no se estan utilizando scoops especificos para esta api.

        });


        //Documentación para las versiones de la API.
        options.SwaggerDoc("v1", new OpenApiInfo {
            Version = "v1",
            Title = "API Ecommerce V1",
            Description = "API para gestionar productos y usuarios",
            TermsOfService = new Uri("https://github.com/"),
            Contact = new OpenApiContact {
                Name = "Henry Tacuri",
                Url = new Uri("https://github.com/")
            },
            License = new OpenApiLicense {
                Name = "Licencia de uso",
                Url = new Uri("https://github.com/")
            }
        });


        options.SwaggerDoc("v2", new OpenApiInfo {
            Version = "v2",
            Title = "API Ecommerce V2",
            Description = "API para gestionar productos y usuarios",
            TermsOfService = new Uri("https://github.com/"),
            Contact = new OpenApiContact {
                Name = "Henry Tacuri",
                Url = new Uri("https://github.com/")
            },
            License = new OpenApiLicense {
                Name = "Licencia de uso",
                Url = new Uri("https://github.com/")
            }
        });
    }
);


//Configuración para el versionamiento de la API.
IApiVersioningBuilder apiVersioningBuilder = builder.Services.AddApiVersioning(options => {
    options.AssumeDefaultVersionWhenUnspecified = true; //Version por defecto
    options.DefaultApiVersion = new ApiVersion(1, 0); //Version por defecto se manda el Grupo y Version minima (opcional)
    options.ReportApiVersions = true; //Ayuda para que los clientes sepan que versiones estan disponibles.
    //options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version")); //Con esto se manda la version de la API a traves de parametro de la URL.
});

//Configuración del API EXPLORER para que swagger muestre las versiones.
apiVersioningBuilder.AddApiExplorer(options => {
    options.GroupNameFormat = "'v'VVV"; //Se de cual es el formato que se va a utilizar en la API, ejemplo V1, V2, V3 .......
    options.SubstituteApiVersionInUrl = true; //Permite sustituir partes de la url por las versiones de la API.
});


//Configuracion de CORS
builder.Services.AddCors(options => {
    options.AddPolicy(
        PolicyNames.AllowSpecificOrigin,
        builder => {
            //builder.WithOrigins("http://localhost:3000")
            builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
        }
    );
});

//builder.Services.AddOpenApi();


//Se inicia la contrucción de la app, se devuelve una instancia de un web app, gracias a a esta instancia se pueden configurar los 
//MiddelWares
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();

    app.UseSwagger();

    app.UseSwaggerUI(options => {
        //Argumentos: URL y nombre
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "API v2");
    });
}

//Este middleware permite trabajar con archivos estaticos como imagenes, pdf y todos los archivos diferentes a los habituales.
app.UseStaticFiles();

//Configuracion para redirigir automaticamente a HTTP a HTTPS para mejorar la seguridad
app.UseHttpsRedirection();

//Agregamos el middelware de los CORS
app.UseCors(PolicyNames.AllowSpecificOrigin);

//Agregamos el middelware para la cache
app.UseResponseCaching();

//Agregamos el middelware para al autenticacion
app.UseAuthentication();

//MiddelWare es el de autorizacion y sirve para proteger los endpoints.
app.UseAuthorization();

//Permite que .NET busque clases con atributos como API Controllers y Root para poder enrutar las solicitudes.
app.MapControllers();


//Inciar la aplicación, este metodo es bloqueante y mantiene la aplicacion en ejecucion. 
app.Run();



/*
Este archivo se encarga de configurar los servicios y tambien de construir y ejecutar el servidor web. 



dotnet tool install --global dotnet-ef      con este comando se instalan de forma global las herramientas de Entity Framework Core.


//Comando para iniciar una migracion
dotnet ef migrations add InitialMigration


//Comando para realizar la migracion a la bd
dotnet ef database update


//Comando para remover la ultima migracion migracion
dotnet ef migrations remove


//Comando para borrar la base de datos
dotnet ef database drop

Para ver el schema de retorno en el swagger o scalar es con este cambio en el dataannotation del metodo

[ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
*/