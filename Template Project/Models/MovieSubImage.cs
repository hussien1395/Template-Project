namespace Template_Project.Models
{
    public class MovieSubImage
    {
        public string ImagePath { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
