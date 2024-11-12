using System.ComponentModel.DataAnnotations;

namespace ConsumingAPI.Models
{
    public class UserAnswer
    {
        [Required]
        public string AnswerUser { get; set; }
    }
}
