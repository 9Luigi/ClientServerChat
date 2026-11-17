namespace WFChatServer
{
    /// <summary>How important a console line is. The console shows the chosen level and everything more important above it.</summary>
    public enum LogLevel
    {
        Critical, //the server can't work properly: port taken, database down, a bug
        Error,    //one client misbehaved, lost its connection or was refused at login
        Info,     //normal events: server started, a user logged in or out
        Message   //chat messages
    }
}
