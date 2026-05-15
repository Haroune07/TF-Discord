using CommunityToolkit.Mvvm.ComponentModel;
using Frontend.Global;
using Shared.DTOs;
using Shared.Enums;

namespace Frontend.ViewModels
{
    public partial class ServerMemberItemViewModel : ObservableObject
    {
        public ServerMemberDTO Member { get; }

        public string Username => Member.User.Username;
        public MemberRole Role => Member.Role;
        public bool IsOnline => Member.User.IsOnline;
        public string OnlineStatusText => Member.User.IsOnline ? "En ligne" : "Hors ligne";

        public bool CanKick =>
            _canKickMembers &&
            Member.User.Id != Session.Current.User?.Id &&
            Member.Role != MemberRole.Owner;

        private readonly bool _canKickMembers;

        public ServerMemberItemViewModel(ServerMemberDTO member, bool canKickMembers)
        {
            Member = member;
            _canKickMembers = canKickMembers;
        }
    }
}
