using RepositoryContracts;
using InMemoryRepositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var storage = builder.Configuration["Storage"] ?? "InMemory";
if (storage.Equals("File", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IUserRepository, FileRepositories.UserFileRepository>();
    builder.Services.AddSingleton<IPostRepository, FileRepositories.PostFileRepository>();
    builder.Services.AddSingleton<ICommentRepository, FileRepositories.CommentFileRepository>();
}
else
{
    builder.Services.AddSingleton<IUserRepository, UserInMemoryRepository>();
    builder.Services.AddSingleton<IPostRepository, PostInMemoryRepository>();
    builder.Services.AddSingleton<ICommentRepository, CommentInMemoryRepository>();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();

app.Run();