using ConsumingAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;
using System.Linq;
using System.Text.Json.Serialization;

namespace ConsumingAPI.Controllers
{
    public class QuizController : Controller
    {
        private static int currentId = 2;
        private static int correctAnswers =0;
        Uri uri = new Uri("https://localhost:7052/");
        private readonly HttpClient _httpClient;

        public QuizController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = uri;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Quiz> quizList = new List<Quiz>();
            HttpResponseMessage response =  _httpClient.GetAsync(_httpClient.BaseAddress + "quiz/getAll").Result;
            if (response.IsSuccessStatusCode) 
            {
                string data =  response.Content.ReadAsStringAsync().Result;
                try
                {
                    quizList = JsonConvert.DeserializeObject<List<Quiz>>(data);
                }
                catch (JsonSerializationException ex)
                {
                    Console.WriteLine("Error en la deserialización: " + ex.Message);
                }
                
            }

            return View(quizList);
        }

        [HttpGet]
        public IEnumerable<Quiz> GetList()
        {
            IEnumerable<Quiz> questionsList = new List<Quiz>();
            HttpResponseMessage response = _httpClient.GetAsync(uri + "quiz/getAll").Result;

            if (response.IsSuccessStatusCode) 
            { 
                string data = response.Content.ReadAsStringAsync().Result;
                try
                {
                    questionsList = JsonConvert.DeserializeObject<List<Quiz>>(data);
                }
                catch (JsonSerializationException ex)
                {
                    Console.WriteLine("Error en la deserialización: " + ex.Message);
                }

            }

            return questionsList;
        }

        //Returns one object from que API with a specific ID, if the id is > list of objects then it return null
        public Quiz GetOneFromAPI()
        {
            var lisQuiz= GetList();
            int id = currentId;

            currentId++;

            Quiz quizobj = new Quiz();

            quizobj.Id = id;

            HttpResponseMessage response = _httpClient.GetAsync(uri + $"quiz/getOne/{id}").Result;
            if (response.IsSuccessStatusCode) {
                string data = response.Content.ReadAsStringAsync().Result;
                quizobj = JsonConvert.DeserializeObject<Quiz>(data);
            }

            if (currentId > 5) //change it to .count()
            {
                return null;
            }

            return quizobj;
        }

        public ActionResult GetOneViewModel() 
        {
            Quiz OneQuestion = GetOneFromAPI();

            ViewModel model = new ViewModel()
            {
                QuizApi = OneQuestion,
                UserAnswer = new UserAnswer()
            };

            if(OneQuestion == null)
            {
                return RedirectToAction("FinishedQuiz");
            }

            return View(model);
        }

       public ActionResult CheckAnswer(string answer, string correctAnswer, string question) 
       {

            if (correctAnswer.ToLower().Contains(answer.ToLower()))
            {
                correctAnswers++;
            }
            else
            {
                // We get the tempdata as string
                var answerListJson = TempData["answerUserList"] as string;

                var questionListJson = TempData["questionList"] as string;

                // If the list doesn't exist, we create it, otherwise we deserialize it
                List<string> answerList = string.IsNullOrEmpty(answerListJson)
                    ? new List<string>()
                    : JsonConvert.DeserializeObject<List<string>>(answerListJson);

                List<string> questionList = string.IsNullOrEmpty(questionListJson) ?
                    new List<string>() :
                    JsonConvert.DeserializeObject<List<string>>(questionListJson);

                // Add new data
                answerList.Add(answer);

                questionList.Add(question);

                // Save the serialized data again in tempData
                TempData["answerUserList"] = JsonConvert.SerializeObject(answerList);

                TempData["questionList"] = JsonConvert.SerializeObject(questionList);
            }
           

            return RedirectToAction("GetOneViewModel");
        }

        public ActionResult FinishedQuiz()
        {
            // Recuperamos la lista serializada de TempData
            var answerListJson = TempData["answerUserList"] as string;

            var questionListJson = TempData["questionList"] as string;

            // Si no existe, se crea una lista vacía
            List<string> answerList = string.IsNullOrEmpty(answerListJson)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(answerListJson);

            List<string> questionList = string.IsNullOrEmpty(questionListJson)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(questionListJson);

            CorrectIncorrectViewModel correctIncorrect = new CorrectIncorrectViewModel()
            {
                CorrectAnswers = correctAnswers,
                IncorrectAnswers = answerList,
                QuestionList = questionList
            };

            // Pasamos la lista a la vista
            return View(correctIncorrect);
             
                               

       }


    }
}
