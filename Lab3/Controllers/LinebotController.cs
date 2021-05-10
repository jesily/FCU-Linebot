
using Line.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            return "Linebot";
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
                    postData = reader.ReadToEndAsync().Result;
                }

                //剖析JSON，轉換為物件
                var receivedMessage = isRock.LineBot.Utility.Parsing(postData);

                //判斷訊息內是否有包含 LINE 的訊息事件
                if (receivedMessage.events.Count > 0)
                {
                    var userSays = receivedMessage.events[0].message.text; //使用者說的况
                    var replyToken = receivedMessage.events[0].replyToken; //回覆時使用的 Token
                    var userId = receivedMessage.events[0].source.userId;  //使用者ID

                    //回覆訊息
                    string Message = "hello, 你說了:" + userSays;
                    //回覆用戶
                    bot.ReplyMessage(replyToken, Message);
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
