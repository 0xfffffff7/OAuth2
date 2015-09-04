using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace MyFeedApp
{
    public partial class feedview : System.Web.UI.Page
    {
        private static string format_json(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // アクセストークン取得.
            string resData = string.Empty;
            string accessToken = Session["access_token"].ToString();
            string expires = Session["expires"].ToString();

            if (string.IsNullOrEmpty(accessToken))
            {
                LABEL1.Text = "accessToken is null.";
                return;
            }

            // これ以後、APIへアクセスする際にベアラー(署名なし)アクセストークンをBASIC認証で使用するだけでよい。
            // [Authorization: Bearer accessToken]
            // Facebookはクエリパラメータでアクセストークンを送信する.

            string html = string.Empty;

            try
            {
                WebClient wc = new WebClient();
                // 自分のフィードを5つ取得する。
                using (Stream st = wc.OpenRead("https://graph.facebook.com/me/feed?limit=5&" + "access_token=" + accessToken))
                {
                    Encoding enc = Encoding.GetEncoding("utf-8");
                    using (StreamReader sr = new StreamReader(st, enc))
                    {
                        string output = sr.ReadToEnd();
                        html = format_json(output);
                    }
                }
            }catch (Exception ex){
                LABEL1.Text = ex.Message;
                return;
            }

            html += "<BR><BR><BR><BR><BR>";

            try
            {
                WebClient wc = new WebClient();
                // 取得可能なノードとフィールドの一覧を取得する。
                using (Stream st = wc.OpenRead("https://graph.facebook.com/me?metadata=1&" + "access_token=" + accessToken))
                {
                    Encoding enc = Encoding.GetEncoding("utf-8");
                    using (StreamReader sr = new StreamReader(st, enc))
                    {
                        html += format_json(sr.ReadToEnd());
                    }
                }
            }
            catch (Exception ex)
            {
                LABEL1.Text = ex.Message;
                return;
            }


            LABEL1.Text = html.Replace("\n", "<br>").Replace(" ", "&ensp;");


        }
    }
}