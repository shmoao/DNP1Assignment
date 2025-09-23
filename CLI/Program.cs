using FileRepositories;
using RepositoryContracts;
using CLI.UI;

namespace CLI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IUserRepository users = new UserFileRepository();
            IPostRepository posts = new PostFileRepository();
            ICommentRepository comments = new CommentFileRepository();

            var app = new CliApp(users, posts, comments);
            await app.RunAsync();
        }
    }
}