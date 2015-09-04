using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace MyFeedApp
{
    public partial class defalut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string oauth = Request["oauth"];
            if (string.IsNullOrEmpty(oauth) == false)
            {

                string clientID = ConfigurationManager.AppSettings["CLIENT_ID"];
                string authorizeURL = ConfigurationManager.AppSettings["AUTHORIZE_URL"];

                string facebookOAuthURL = authorizeURL;

                // ユーザーキー.
                string client_id = clientID;

                // 認可後のコールバックURL.
                string redirect_uri = ConfigurationManager.AppSettings["CALLBACK_URL"];

                // APIの要求スコープ.
                string scope = "user_about_me,user_photos,user_activities,read_stream";

                // リクエストタイプ.ウェブアプリケーションフローなので認可コードを意味する「code」を指定する。
                string response_type = "code";

                // XSRF対策でnonceを生成.
                Guid guidValue = Guid.NewGuid();
                string state = guidValue.ToString();
                Session["state"] = state;

                // URL作成.
                facebookOAuthURL += "client_id=" + clientID;
                facebookOAuthURL += "&redirect_uri=" + HttpUtility.UrlEncode(redirect_uri);
                facebookOAuthURL += "&response_type=" + response_type;
                facebookOAuthURL += "&state=" + state;
                facebookOAuthURL += "&scope=" + scope;
                

                Response.Redirect(facebookOAuthURL);
            }
        }
    }
}