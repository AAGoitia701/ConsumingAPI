using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsumingAPI.Models
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("question")]
        [JsonProperty("question")]
        public string QuestionQuiz { get; set; }
        [DisplayName("answer")]
        [JsonProperty("answer")]
        public string Answer { get; set; }
    }
}
