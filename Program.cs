using FluentValidation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using minimalApi.EndPoint;
using minimalApi.Identidades;
using minimalApi.Repositorio;
using minimalApi.Servicios;
using minimalApi.Utilidades;
using System;
using Microsoft.OpenApi.Models;
using minimalApi.Swagger;
var builder = WebApplication.CreateBuilder(args);
//var apellido = builder.Configuration.GetValue<string>("apellido");

var origenesPermitidos = builder.Configuration.GetValue<string>("dominioPermitido");

//builder.Services.AddOutputCache();
builder.Services.AddStackExchangeRedisOutputCache(opciones =>
{
    opciones.Configuration = builder.Configuration.GetConnectionString("redis");
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title="Pelicula V1",
        Description="Este es un web api para trabajar con datos de peliculas",
        Contact= new OpenApiContact
        {
            Email="edderjamancamedoza@gmail.com",
            Name="Edder Jamanca Mendoza"
        },
        License = new OpenApiLicense
        {
            Name="MIT",
            Url=new Uri("https://opensourse.org/license/mit/")
        }
    });
    c.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme
    {
        Name="Authorization",
        Type=SecuritySchemeType.ApiKey,
        Scheme="Bearer",
        BearerFormat="JWT",
        In=ParameterLocation.Header
    });
    c.OperationFilter<FiltroAutorizacion>();
    //c.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference= new OpenApiReference
    //            {
    //                Type= ReferenceType.SecurityScheme,
    //                Id="Bearer"
    //            }
    //        }, new string[]{}
    //    }
    //});
});
//area de servicios
builder.Services.AddCors(opciones => {
        opciones.AddDefaultPolicy(configuracion =>
        {
            configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            //configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
        });
        opciones.AddPolicy("libres", configuracion =>
        {
            //configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
        });
    });

builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();
//AlmacenadorArchivoAzure: IAlamacenadorArchivos
//builder.Services.AddScoped<IAlamacenadorArchivos, AlmacenadorArchivoAzure>();
builder.Services.AddScoped<IAlamacenadorArchivos, AlmacenadorArchivoLocal>();
// este es el servicio para aceder wwwroot
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IRepositorioComentario, RepositorioComentario>();
builder.Services.AddScoped<IRepositorioError, RepositorioError>();
builder.Services.AddScoped<IRepositorioUsuario,RepositorioUsuario>();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddProblemDetails();

builder.Services.AddAuthentication().AddJwtBearer(opciones =>
{
    opciones.MapInboundClaims = false;

    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //IssuerSigningKey = LLave.ObtenerLlave(builder.Configuration).First(),
        // esto es para usar todas las llaves que hay el archivo 
        IssuerSigningKeys = LLave.ObtenerTodasLasLlaves(builder.Configuration),
        ClockSkew = TimeSpan.Zero,
    };
}
);
builder.Services.AddAuthorization(optiones =>
{
    optiones.AddPolicy("esadmin", politica => politica.RequireClaim("esadmin"));
});
// los servicios para usar el identity 
builder.Services.AddTransient<IUserStore<IdentityUser>, UsuarioStore>();
builder.Services.AddIdentityCore<IdentityUser>();
builder.Services.AddTransient<SignInManager<IdentityUser>>();
//find de los servicios

var app = builder.Build();
//inicio de área de los meddlewere
app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
 {
     var exceptionHandleFeature=context.Features.Get<IExceptionHandlerFeature>();
     var exception = exceptionHandleFeature?.Error!;
     var error = new Error();
     error.fecha = DateTime.UtcNow;
     error.mensajeDeError = exception.Message;
     error.StackTrace= exception.StackTrace;

     var repositorio = context.RequestServices.GetRequiredService<IRepositorioError>();
     await repositorio.crear(error);

     await TypedResults.BadRequest(
        new { tipo = "error", mensaje = "ha ocurrido un mensaje de error inesperado", estatus = 500 })
    .ExecuteAsync(context);
}));
app.UseStatusCodePages();
app.UseCors();
app.UseOutputCache();

app.UseAuthorization();

// este es el meddlewere para acceder a  los archivos estaticos
app.UseStaticFiles();

app.MapGet("/", [EnableCors(policyName: "libre")]() => "bienvenido a la api de pelicula xd");
app.MapGet("/error", () =>
{
    throw new InvalidOperationException("error de ejemplo");
});
// obtener los datos de un lugar en especifico
// [FromQuery] -> query params, [FromBody] -> body,[FromHeader]-> de la cabecera
// [FromForm] -> datos fontdata, [FromRoute] -> de la url
//[FromServices]-> esto es datos probiene de un servicio
app.MapPost("/modelbinding",(string? nombre) => 
{  
    if(nombre is null)
    {
        nombre = "vacio";
    } 
});

app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();
app.MapGroup("/pelicula").MapPelicula();
app.MapGroup("/comentario/{idpelicula}").MapComentario();
app.MapGroup("/usuario").MapUsuario();
//var endpointGeneros = app.MapGroup("/generos");

//endpointGeneros.MapGet("/", ObtenerGeneros).CacheOutput(c=>c.Expire(TimeSpan.FromSeconds(60))).WithTags("generos-get");

//endpointGeneros.MapGet("/{id:int}", ObtenerGeneroPorId);

//endpointGeneros.MapPost("/", crearGenero);

//endpointGeneros.MapPut("/",updateGenero);

//endpointGeneros.MapDelete("/{id:int}", EliminarGenero);

// fin de área de los middlewere



app.Run();


//static async Task<Ok<List<Genero>>> ObtenerGeneros(IRepositorioGeneros repositorio)
//{
//    var generos =await repositorio.ObtenerTodos();
//    return TypedResults.Ok(generos);
//}

//static  async Task<Results<Ok<Genero>,NotFound>> ObtenerGeneroPorId(IRepositorioGeneros repositorio, int id){
//    var genero = await repositorio.ObtenerPorId(id);

//    if (genero is null)
//    {
//        return TypedResults.NotFound();
//    }
//    return TypedResults.Ok(genero);
//}

//static async Task<Created<Genero>> crearGenero(Genero genero, IRepositorioGeneros repositoriogeneros, IOutputCacheStore outputCacheStore)
//{
//    var id = await repositoriogeneros.CrearGenero(genero);
//    // esto hace que se limpie la cache
//    await outputCacheStore.EvictByTagAsync("generos-get", default);

//    return TypedResults.Created($"/generos/{id}", genero);
//}

//static async Task<Results<NotFound,NoContent>> updateGenero(int id, Genero genero, IRepositorioGeneros repositoriogeneros, IOutputCacheStore outputCacheStore)
//{
//    var existe = await repositoriogeneros.Existe(id);
//    if (!existe)
//    {
//        return TypedResults.NotFound();
//    }

//    await repositoriogeneros.Actualizar(genero);

//    await outputCacheStore.EvictByTagAsync("generos-get", default);

//    return TypedResults.NoContent();
//}

//static async Task<Results<NotFound,NoContent>> EliminarGenero(int id, IRepositorioGeneros repositoriogeneros, IOutputCacheStore outputCacheStore)
//{
//    var existe = await repositoriogeneros.Existe(id);
//    if (!existe)
//    {
//        return TypedResults.NotFound();
//    }

//    await repositoriogeneros.Borrar(id);
//    await outputCacheStore.EvictByTagAsync("generos-get", default);

//    return TypedResults.NoContent();

//}