namespace Muffle.Data.Models
{
    public class ChannelPermission
    {
        public int ChannelId { get; set; }
        public int RoleId { get; set; }
        public bool AllowRead { get; set; }
        public bool AllowSend { get; set; }
        public bool AllowManage { get; set; }
    }
}
