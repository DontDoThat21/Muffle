namespace Muffle.Data.Models
{
    public class Friend
    {
        public Friend()
        {

        }
        public Friend(int id, string name, string description, string image, string memo, DateTime friendshipDate, DateTime creationDate)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Image = image;
            this.Memo = memo;
            this.FriendshipDate = friendshipDate;
            this.CreationDate = creationDate;
        }
        public int UserId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Memo { get; set; }
        public string Image { get; set; }        
        public DateTime FriendshipDate { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
