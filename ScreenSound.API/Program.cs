using Microsoft.AspNetCore.Mvc;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using System.Security.Cryptography.Xml;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ScreenSoundContext>();
builder.Services.AddTransient<DAL<Artista>>();


builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/artistas", ([FromServices] DAL<Artista> dal) =>
{
    return Results.Ok(dal.Listar());

});

app.MapGet("/artistas/{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
{
    var artista = dal.RecuperarListaPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));
    if (artista is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(artista);
});

app.MapPost("/artistas", ([FromServices] DAL<Artista> dal, [FromBody]Artista artista) => {
    dal.Adicionar(artista);
    return Results.Ok();
});

app.MapDelete("/artistas/{id}", ([FromServices] DAL<Artista> dal, int id) => {
    var artista = dal.RecuperarPor(a => a.Id == id);
    if (artista is null)
    {
        return Results.NoContent();
    }
    dal.Deletar(artista);
    return Results.Ok();
});

app.MapPut("/artistas", ([FromServices] DAL<Artista> dal, [FromBody] Artista artista) => {
    var artistaAtualizar = dal.RecuperarPor(a => a.Id == artista.Id);

    if (artistaAtualizar is null)
    {
        return Results.NotFound();
    }
    artistaAtualizar.Nome = artista.Nome;
    artistaAtualizar.Bio = artista.Bio;

    artistaAtualizar.FotoPerfil = artista.FotoPerfil;
    dal.Atualizar(artistaAtualizar);
    return Results.Ok();
});



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
