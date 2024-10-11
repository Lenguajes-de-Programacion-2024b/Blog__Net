namespace Blog__Net.Models
{
    public class PostLike
    {

        public Guid Id { get; set; }

        public int PostId { get; set; }

        public Guid UserId { get; set; }

        public DateTime LikeDate { get; set; }

    }
}
