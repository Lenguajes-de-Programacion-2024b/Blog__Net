namespace Blog__Net.Models.ViewModels
{
    public class PostDetailsViewModels
    {
        public Posts? Post {  get; set; }
        public List<Comments>? MainComments { get; set; }
        public List<Comments>? SonComments { get; set; }
        public List<Comments>? GrandSonComments { get; set; }
        public List<Posts>? RecentPosts { get; set; }
        public int LikesCount { get; set; }
        public bool UserHasLiked { get; set; }
    }
}
