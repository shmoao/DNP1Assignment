using System;
using System.Linq;
using System.Threading.Tasks;
using Entities;
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

            await SeedAsync(users, posts, comments);

            var app = new CliApp(users, posts, comments);
            await app.RunAsync();
        }

        private static async Task SeedAsync(IUserRepository users, IPostRepository posts, ICommentRepository comments)
        {
            if (users.GetManyAsync().Any() || posts.GetManyAsync().Any() || comments.GetManyAsync().Any())
                return;

            var u1 = await users.AddAsync(new User { UserName = "Soma" });
            var u2 = await users.AddAsync(new User { UserName = "Rebeca" });
            var u3 = await users.AddAsync(new User { UserName = "Chris" });

            var p1 = await posts.AddAsync(new Post { Title = "AAA", Body = "...", UserId = u1.Id });
            var p2 = await posts.AddAsync(new Post { Title = "BBB", Body = "...", UserId = u2.Id });
            var p3 = await posts.AddAsync(new Post { Title = "CCC", Body = "...", UserId = u1.Id });

            await comments.AddAsync(new Comment { Body = "bbb",  UserId = u2.Id, PostId = p1.Id });
            await comments.AddAsync(new Comment { Body = "eeee", UserId = u1.Id, PostId = p1.Id });
            await comments.AddAsync(new Comment { Body = "eeee", UserId = u3.Id, PostId = p2.Id });
        }
    }
}