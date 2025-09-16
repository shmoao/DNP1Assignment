using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private IUserRepository _users;
    private IPostRepository _posts;
    private ICommentRepository _comments;

    public CliApp(IUserRepository users, IPostRepository posts, ICommentRepository comments)
    {
        _users = users;
        _posts = posts;
        _comments = comments;
    }

    public async Task RunAsync()
    {
        WriteHeader();
        PrintHelp();

        while (true)
        {
            Console.Write("\n> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;

            var args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cmd = args[0].ToLowerInvariant();

            try
            {
                switch (cmd)
                {
                    case "help":
                    case "h": PrintHelp(); break;
                    case "exit":
                    case "quit":
                    case "q": Console.WriteLine("Bye!"); return;

                    case "users:add": await UsersAddAsync(); break;
                    case "users:list": UsersList(); break;
                    case "users:view": UsersView(args); break;
                    case "users:update": await UsersUpdateAsync(args); break;
                    case "users:delete": await UsersDeleteAsync(args); break;
                    case "users:find": UsersFind(args); break;

                    case "posts:add": await PostsAddAsync(); break;
                    case "posts:list": PostsList(args); break;
                    case "posts:view": PostsView(args); break;
                    case "posts:update": await PostsUpdateAsync(args); break;
                    case "posts:delete": await PostsDeleteAsync(args); break;

                    case "comments:add": await CommentsAddAsync(); break;
                    case "comments:list": CommentsList(args); break;
                    case "comments:update": await CommentsUpdateAsync(args); break;
                    case "comments:delete": await CommentsDeleteAsync(args); break;

                    default: Console.WriteLine("Unknown command. Type `help`."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static void WriteHeader()
    {
        Console.WriteLine("====================================");
        Console.WriteLine("   Blog CLI — Assignment 2");
        Console.WriteLine("====================================");
    }

    private static void PrintHelp()
    {
        Console.WriteLine("""
Available commands:

  help | h                 Show help
  exit | quit | q          Exit

  USERS
  users:add                Create user (UserName)
  users:list               List users
  users:view <id>          Show user details
  users:update <id>        Update user
  users:delete <id>        Delete user
  users:find <substring>   Find users by name

  POSTS
  posts:add                Create post (Title, Body, UserId)
  posts:list [--user <id>] List posts (filter by user)
  posts:view <id>          Show post details with comments
  posts:update <id>        Update post
  posts:delete <id>        Delete post

  COMMENTS
  comments:add             Create comment (PostId, UserId, Body)
  comments:list            List comments
  comments:update <id>     Update comment
  comments:delete <id>     Delete comment
""");
    }

    private async Task UsersAddAsync()
    {
        Console.Write("User name: ");
        var name = ReadNonEmpty();
        if (_users.GetManyAsync().Any(u => string.Equals(u.UserName, name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("This name is already taken");
        var created = await _users.AddAsync(new User { UserName = name });
        Console.WriteLine($"User created with Id={created.Id}, UserName={created.UserName}");
    }

    private void UsersList()
    {
        var all = _users.GetManyAsync().OrderBy(u => u.Id).ToList();
        if (all.Count == 0) { Console.WriteLine("(nobody)"); return; }
        Console.WriteLine("\nID    | UserName");
        Console.WriteLine("*****************************");
        foreach (var u in all) Console.WriteLine($"{u.Id,-6}| {u.UserName}");
    }

    private void UsersView(string[] args)
    {
        if (!TryParseIdArg(args, 1, out var id, "Usage: users:view <id>")) return;
        var user = _users.GetSingleAsync(id).Result;
        if (user is null) { Console.WriteLine("C."); return; }

        Console.WriteLine($"\nUser {user.Id}: {user.UserName}");

        var posts = _posts.GetManyAsync().Where(p => p.UserId == user.Id).OrderBy(p => p.Id).ToList();
        Console.WriteLine("\nPosts:");
        if (posts.Count == 0) Console.WriteLine("  (none)");
        else foreach (var p in posts) Console.WriteLine($"  - [{p.Id}] {p.Title}");

        var comments = _comments.GetManyAsync().Where(c => c.UserId == user.Id).OrderBy(c => c.Id).ToList();
        Console.WriteLine("\nComments:");
        if (comments.Count == 0) Console.WriteLine("  (none)");
        else foreach (var c in comments) Console.WriteLine($"  - [{c.Id}] (Post {c.PostId}) {c.Body}");
    }

    private async Task UsersUpdateAsync(string[] args)
    {
        if (!TryParseIdArg(args, 1, out var id, "Usage: users:update <id>")) return;
        var user = _users.GetSingleAsync(id).Result;
        if (user is null) { Console.WriteLine("User was not find."); return; }

        Console.Write($"New name (current: {user.UserName}): ");
        var name = ReadNonEmpty();
        if (_users.GetManyAsync().Any(u => u.Id != id && string.Equals(u.UserName, name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("User name is already taken.");

        user.UserName = name;
        await _users.UpdateAsync(user);
        Console.WriteLine("User updated.");
    }

    private async Task UsersDeleteAsync(string[] args)
    {
        if (!TryParseIdArg(args, 1, out var id, "Usage: users:delete <id>")) return;
        await _users.DeleteAsync(id);
        Console.WriteLine("User deleted.");
    }

    private void UsersFind(string[] args)
    {
        if (args.Length < 2) { Console.WriteLine("Usage: users:find <substring>"); return; }
        var term = string.Join(' ', args.Skip(1)).Trim();
        var found = _users.GetManyAsync()
                          .Where(u => u.UserName.Contains(term, StringComparison.OrdinalIgnoreCase))
                          .OrderBy(u => u.Id).ToList();
        if (!found.Any()) { Console.WriteLine("(no matches)"); return; }
        foreach (var u in found) Console.WriteLine($"[{u.Id}] {u.UserName}");
    }

    private async Task PostsAddAsync()
    {
        Console.Write("User Id: "); var userId = ReadInt(); EnsureUserExists(userId);
        Console.Write("Title: "); var title = ReadNonEmpty();
        Console.Write("Body: ");  var body  = ReadNonEmpty();

        var created = await _posts.AddAsync(new Post { UserId = userId, Title = title, Body = body });
        Console.WriteLine($"Post created Id={created.Id}, Title=\"{created.Title}\"");
    }

    private void PostsList(string[] args)
    {
        int? byUser = null;
        for (int i = 1; i < args.Length; i++)
        {
            if (args[i] == "--user" && i + 1 < args.Length && int.TryParse(args[i + 1], out var uid))
            { byUser = uid; i++; }
        }

        var q = _posts.GetManyAsync();
        if (byUser is int uidFilter) q = q.Where(p => p.UserId == uidFilter);

        var all = q.OrderBy(p => p.Id).ToList();
        if (all.Count == 0) { Console.WriteLine("(no posts yet)"); return; }

        Console.WriteLine("\nID    | Title                      | UserId");
        Console.WriteLine("------+-----------------------------+-------");
        foreach (var p in all) Console.WriteLine($"{p.Id,-6}| {p.Title,-27}| {p.UserId}");
    }

    private void PostsView(string[] args)
    {
        if (!TryParseIdArg(args, 1, out var id, "Usage: posts:view <id>")) return;

        var post = _posts.GetSingleAsync(id).Result;
        if (post is null) { Console.WriteLine("Post was not find."); return; }

        Console.WriteLine($"""
------------------------------
Post {post.Id}: {post.Title}
Author (UserId): {post.UserId}

{post.Body}
------------------------------
Comments:
""");

        var comments = _comments.GetManyAsync().Where(c => c.PostId == post.Id).OrderBy(c => c.Id).ToList();
        if (comments.Count == 0) Console.WriteLine("(no comments)");
        else foreach (var c in comments) Console.WriteLine($" - [{c.Id}] (User {c.UserId}) {c.Body}");
    }

    private async Task PostsUpdateAsync(string[] args)
    {
        if (!TryParseIdArg(args, 1, out var id, "Usage: posts:update <id>")) return;

        var post = _posts.GetSingleAsync(id).Result;
        if (post is null) { Console.WriteLine("Post not found."); return; }

        Console.Write($"New Title (current: {post.Title}): "); var title = ReadNonEmpty();
        Console.Write($"New Body (current: {TrimTo(post.Body, 25)}): "); var body = ReadNonEmpty();
        Console.Write($"New UserId (current: {post.UserId}): "); var userId = ReadInt(); EnsureUserExists(userId);

        post.Title = title; post.Body = body; post.UserId = userId;

        await _posts.UpdateAsync(post);
        Console.WriteLine("Post updated.");
    }

    private async Task PostsDeleteAsync(string[] args)
    {
        if (!TryParseIdArg(args, 1, out var id, "Usage: posts:delete <id>")) return;
        await _posts.DeleteAsync(id);
        Console.WriteLine("Post deleted.");
    }

    private async Task CommentsAddAsync()
    {
        Console.Write("Post Id: "); var postId = ReadInt(); EnsurePostExists(postId);
        Console.Write("User Id: "); var userId = ReadInt(); EnsureUserExists(userId);
        Console.Write("Body: ");    var body = ReadNonEmpty();

        var created = await _comments.AddAsync(new Comment { PostId = postId, UserId = userId, Body = body });
        Console.WriteLine($"Comment created Id={created.Id} for Post {postId}");
    }

    private void CommentsList(string[] args)
    {
        int? byUser = null;
        int? byPost = null;

        for (int i = 1; i < args.Length; i++)
        {
            if (args[i] == "--user" && i + 1 < args.Length && int.TryParse(args[i + 1], out var uid))
            { byUser = uid; i++; }
            else if (args[i] == "--post" && i + 1 < args.Length && int.TryParse(args[i + 1], out var pid))
            { byPost = pid; i++; }
        }

        var q = _comments.GetManyAsync();
        if (byUser is int uf) q = q.Where(c => c.UserId == uf);
        if (byPost is int pf) q = q.Where(c => c.PostId == pf);

        var all = q.OrderBy(c => c.Id).ToList();
        if (all.Count == 0) { Console.WriteLine("(no comments)"); return; }

        Console.WriteLine("\nID    | Post | User | Body");
        Console.WriteLine("------+------|------|------------------------------");
        foreach (var c in all) Console.WriteLine($"{c.Id,-6}| {c.PostId,-4} | {c.UserId,-4} | {c.Body}");
    }

    private async Task CommentsUpdateAsync(string[] args)
    {
        if (!TryParseIdArg(args, 1, out var id, "Usage: comments:update <id>")) return;

        var comment = _comments.GetSingleAsync(id).Result;
        if (comment is null) { Console.WriteLine("Comment not found."); return; }

        Console.Write($"New Body (current: {TrimTo(comment.Body, 25)}): "); var body = ReadNonEmpty();
        Console.Write($"New UserId (current: {comment.UserId}): "); var userId = ReadInt(); EnsureUserExists(userId);
        Console.Write($"New PostId (current: {comment.PostId}): "); var postId = ReadInt(); EnsurePostExists(postId);

        comment.Body = body; comment.UserId = userId; comment.PostId = postId;

        await _comments.UpdateAsync(comment);
        Console.WriteLine("Comment updated.");
    }

    private async Task CommentsDeleteAsync(string[] args)
    {
        if (!TryParseIdArg(args, 1, out var id, "Usage: comments:delete <id>")) return;

        await _comments.DeleteAsync(id);
        Console.WriteLine("Comment deleted.");
    }

    private static string ReadNonEmpty()
    {
        while (true)
        {
            var s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
            Console.Write("Enter value: ");
        }
    }

    private static int ReadInt()
    {
        while (true)
        {
            var s = Console.ReadLine();
            if (int.TryParse(s, out var n)) return n;
            Console.Write("Enter a number: ");
        }
    }

    private static bool TryParseIdArg(string[] args, int index, out int id, string usageIfFail)
    {
        id = default;
        if (args.Length <= index) { Console.WriteLine(usageIfFail); return false; }
        if (!int.TryParse(args[index], out id)) { Console.WriteLine("Id must be a number."); return false; }
        return true;
    }

    private void EnsureUserExists(int id)
    {
        if (_users.GetManyAsync().All(u => u.Id != id))
            throw new InvalidOperationException("User with given Id does not exist.");
    }

    private void EnsurePostExists(int id)
    {
        if (_posts.GetManyAsync().All(p => p.Id != id))
            throw new InvalidOperationException("Post with given Id does not exist.");
    }

    private static string TrimTo(string s, int max)
        => s.Length <= max ? s : s[..max] + "…";
}
