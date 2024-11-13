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

            if (currentId > 3)
            {
                //Console.WriteLine($"You had {correctAnswers} correct answers");
                //RedirectToAction("FinishedQuiz");
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

       public ActionResult CheckAnswer(string answer, string correctAnswer) 
       {
            if (ModelState.IsValid) 
            {
                CorrectIncorrectViewModel correctIncorrectViewModel = new CorrectIncorrectViewModel();


                if (correctAnswer.ToLower().Contains(answer.ToLower()))
                {
                    correctAnswers++;
                    correctIncorrectViewModel.CorrectAnswers++;
                }
                else
                {
                    correctIncorrectViewModel.IncorrectAnswers.Add(answer);
                }

   
            }          
            

            return RedirectToAction("GetOneViewModel");
       }

        public ActionResult FinishedQuiz()
        {
       

            return View();

           

        }
    }
}
