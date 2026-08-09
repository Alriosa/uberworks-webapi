// =====================================================================================
// FILE SUMMARY
// What it does: Identifies which of the two parties on a Chat row actually sent that
//               message — a conversation only ever has exactly two participants (one Client,
//               one Professional), so this is a closed 2-value enum rather than reusing the
//               much larger UserRole. Without this column, TBL_CHATS had no way to tell the
//               two sides of a conversation apart (see AddSenderRoleToChats.sql).
// Entities connected: Chat.cs (the Chat.SenderRole property is of this type)
// Tables related: TBL_CHATS.CL_SENDER_ROLE
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Maps to the values used in TBL_CHATS.CL_SENDER_ROLE.
/// </summary>
public enum ChatSenderRole
{
    Client,
    Professional
}
