using System;
using System.Text.Json.Serialization;

namespace ChatShared.Protocol
{
    /// <summary>
    /// Base type of every message exchanged between client and server.
    /// On the wire each message is one JSON object on its own line; the "type" field tells which class it is.
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(LoginRequest), "login")]
    [JsonDerivedType(typeof(LoginResult), "loginResult")]
    [JsonDerivedType(typeof(SendText), "text")]
    [JsonDerivedType(typeof(ChatText), "chatText")]
    [JsonDerivedType(typeof(UserJoined), "userJoined")]
    [JsonDerivedType(typeof(UserLeft), "userLeft")]
    public abstract record ChatMessage;

    /// <summary>Client → server: the first message after connecting.</summary>
    public sealed record LoginRequest(string UserName, string Password) : ChatMessage;

    /// <summary>Server → client: answer to <see cref="LoginRequest"/>. <see cref="Error"/> is set only when <see cref="Success"/> is false.</summary>
    public sealed record LoginResult(bool Success, LoginError? Error = null) : ChatMessage;

    /// <summary>Client → server: a chat message typed by the user.</summary>
    public sealed record SendText(string Text) : ChatMessage;

    /// <summary>Server → clients: a chat message from <see cref="From"/>; <see cref="Time"/> is when the server received it.</summary>
    public sealed record ChatText(string From, string Text, DateTimeOffset Time) : ChatMessage;

    /// <summary>Server → clients: a user has logged in.</summary>
    public sealed record UserJoined(string UserName, DateTimeOffset Time) : ChatMessage;

    /// <summary>Server → clients: a user has disconnected.</summary>
    public sealed record UserLeft(string UserName, DateTimeOffset Time) : ChatMessage;

    [JsonConverter(typeof(JsonStringEnumConverter<LoginError>))] //written as "IncorrectPassword", not as a number
    public enum LoginError
    {
        UserNotFound,
        IncorrectPassword,
        ServerError
    }
}
