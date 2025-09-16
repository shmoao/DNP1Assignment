using InMemoryRepositories;
using RepositoryContracts;
using CLI.UI;

namespace CLI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IUserRepository users = new UserInMemoryRepository();
            IPostRepository posts = new PostInMemoryRepository();
            ICommentRepository comments = new CommentInMemoryRepository();

            var app = new CliApp(users, posts, comments);
            await app.RunAsync();
        }
    }
}