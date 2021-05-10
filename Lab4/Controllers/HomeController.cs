using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Lab4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Lab4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string userSay)
        {
            //由 google api console 建立服務帳號, 並匯出存取token
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"D:\temp\worldmap-1357-c80b706e2fa6.json");

            var input = new TextInput
            {
                Text = userSay, LanguageCode = "zh-TW"
            };

            // Create client
            SessionsClient sessionsClient = SessionsClient.Create();

            // Initialize request argument(s)
            DetectIntentRequest request = new DetectIntentRequest
            {
                SessionAsSessionName = SessionName.FromProjectSession("worldmap-1357", "[SESSION]"),
                QueryParams = new QueryParameters(),
                QueryInput = new QueryInput() { Text = input },
                OutputAudioConfig = new OutputAudioConfig(),
                InputAudio = ByteString.Empty,
                OutputAudioConfigMask = new FieldMask(),
            };

            // Make the request
            DetectIntentResponse response = sessionsClient.DetectIntent(request);
            ViewData["response"] = response.QueryResult.FulfillmentText;
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
