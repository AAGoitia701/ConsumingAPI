namespace ConsumingAPI.Models
{
    public class CorrectIncorrectViewModel
    {
        public int CorrectAnswers { get; set; } = 0;
        public List<string> IncorrectAnswers { get; set; } = new List<string>();
    }
}
