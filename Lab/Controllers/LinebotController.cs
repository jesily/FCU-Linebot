using Google.Apis.Auth.OAuth2;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Line.Messaging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace linebot.Controllers
{
    [Route("api/linebot")]
    [ApiController]
    public class LinebotController : ControllerBase
    {
        /// <summary>
        /// Linebot的存取權杖，由 Linebot Developer 管理介面中取得
        /// </summary>
        private string _AccessToken = "Linebot的存取權杖";

        public LinebotController()
        {
        }

        public string Get()
        {
            #region 測試　Dialogflow 時需要使用, GOOGLE_APPLICATION_CREDENTIALS 需由 Google API Console 介面中取得
            /*
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"認證檔.json");

            var txt = new TextInput
            {
                Text = "iphone有什麼優費?",
                LanguageCode = "en-us"
            };

            // Create client
            SessionsClient sessionsClient = SessionsClient.Create();
            // Initialize request argument(s)
            DetectIntentRequest request = new DetectIntentRequest
            {
                SessionAsSessionName = SessionName.FromProjectSession("APP Name", "[SESSION]"),
                QueryParams = new QueryParameters(),
                QueryInput = new QueryInput() {  Text = txt },
                OutputAudioConfig = new OutputAudioConfig(),
                InputAudio = ByteString.Empty,
                OutputAudioConfigMask = new FieldMask(),
            };

            // Make the request
            DetectIntentResponse response = sessionsClient.DetectIntent(request);
            */
            #endregion

            return "Linebot";
        }

        /// <summary>
        /// 呼叫 Dialogflow 識別出意圖
        /// </summary>
        /// <param name="userSay"></param>
        /// <returns></returns>
        public DetectIntentResponse CallDialogFlow(string userSay)
        {
            //由 google api console 建立服務帳號, 並匯出存取token
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"認證檔.json");

            var input = new TextInput
            {
                Text = userSay,
                LanguageCode = "zh-TW"
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

            return response;
        }


        /// <summary>
        /// 接收來自 LINE WebHook 的訊息內容
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Post()
        {           
            isRock.LineBot.Bot bot;

            bot = new isRock.LineBot.Bot(_AccessToken);

            try
            {
                string postData = string.Empty;
                using (var reader = new StreamReader(Request.Body))
                {
                    postData =  reader.ReadToEndAsync().Result;
                }

                //剖析JSON，轉換為物件
                var receivedMessage = isRock.LineBot.Utility.Parsing(postData);

                //判斷訊息內是否有包含 LINE 的訊息事件
                if (receivedMessage.events.Count > 0)
                {
                    var userSays = receivedMessage.events[0].message.text; //使用者說的况
                    var replyToken = receivedMessage.events[0].replyToken; //回覆時使用的 Token
                    var userId = receivedMessage.events[0].source.userId;  //使用者ID

                    //建立一個 dialogflow 回應物件 — DetectIntentResponse
                    DetectIntentResponse result = new DetectIntentResponse()
                    {
                        QueryResult = new QueryResult()
                        {
                            Intent = new Intent()
                            {
                                IsFallback = false
                            } 
                         } 
                    };

                    //呼叫 DialogFlow 找看看是否能對映到設計好的意圖
                    //result =  CallDialogFlow(userSays);

                    if (result.QueryResult.Intent.IsFallback)
                    {
                        //Dialogflow無法分析意圖
                        //依照用戶說的特定關鍵字來回應
                        switch (userSays.ToLower())
                        {
                            case "/myid":
                                //回覆使用者的ID                      
                                bot.ReplyMessage(replyToken, userId);
                                break;

                            case "/satellite":
                                //回覆圖片
                                bot.ReplyMessage(replyToken, new Uri("https://www.cwb.gov.tw/Data/satellite/LCC_IR1_CR_2750/LCC_IR1_CR_2750-2021-05-04-17-30.jpg"));
                                break;

                            default:
                                //回覆訊息
                                string Message = "哈囉, 你說了:" + userSays ;
                                //回覆用戶
                                bot.ReplyMessage(replyToken, Message);
                                break;
                        }

                    } else
                    {
                        //Dialogflow找到意圖，回覆內容
                        bot.ReplyMessage(replyToken, result.QueryResult.FulfillmentText);
                    }
                }
                //回覆API OK
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok();
            }
        }
    }
}
