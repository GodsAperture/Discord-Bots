using RexLapis.Database;

public partial class UserJoinedHandler
{
    private readonly DiscordSocketClient _client;

    public UserJoinedHandler(DiscordSocketClient client)
    {
        _client = client;
    }

    public Task Initialize()
    {
        _client.UserJoined += userJoinedHandler;
        _client.UserLeft += userLeftHandler;  
        return Task.CompletedTask;
    }

    private async Task userJoinedHandler(SocketGuildUser user)
    {
        
        return;
    }

    private async Task userLeftHandler(SocketGuild guild, SocketUser user){

    }
}