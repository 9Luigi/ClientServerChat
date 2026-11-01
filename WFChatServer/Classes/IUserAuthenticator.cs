using ChatShared.Protocol;

namespace WFChatServer
{
    /// <summary>Checks user names and passwords for <see cref="ChatServer"/>.</summary>
    public interface IUserAuthenticator
    {
        /// <summary>Returns null when the user name and password are correct, otherwise the reason of the refusal.</summary>
        /// <remarks>Throws when the check itself fails, e.g. the database is unavailable.</remarks>
        LoginError? Authenticate(string userName, string password);
    }
}
