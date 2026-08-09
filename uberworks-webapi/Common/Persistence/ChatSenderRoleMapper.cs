// =====================================================================================
// FILE SUMMARY
// What it does: Converts ChatSenderRole to/from the text value stored in
//               TBL_CHATS.CL_SENDER_ROLE. No special cases — Client/Professional uppercase
//               cleanly both ways. Called explicitly by ChatRepository.cs — see
//               UserRoleMapper.cs for why this is a plain static method call instead of a
//               registered Dapper TypeHandler.
// Entities connected: Chat.cs
// Tables related: TBL_CHATS.CL_SENDER_ROLE
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Common.Persistence;

public static class ChatSenderRoleMapper
{
    public static string ToDb(ChatSenderRole value) => value.ToString().ToUpperInvariant();
    public static ChatSenderRole FromDb(string value) => Enum.Parse<ChatSenderRole>(value, ignoreCase: true);
}
