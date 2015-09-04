using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Net.Http;

namespace MyFeedApp
{
    public partial class oauth : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //-----------------------------------------------------------------------
            // エラー検査.
            //-----------------------------------------------------------------------

            // エラーコード.
            string error = Request["error"];

            // エラー詳細.
            string error_description = Request["error_description"];

            // エラー詳細情報を記載したURL.
            string error_uri = Request["error_uri"];

            string resHtml = string.Empty;
            if(string.IsNullOrEmpty(error) == false){
                resHtml += "error=";
                resHtml += error;
                resHtml += "<BR>";

                if (string.IsNullOrEmpty(error_description) == false)
                {
                    resHtml += "error_description=";
                    resHtml += error_description;
                    resHtml += "<BR>";
                }

                if (string.IsNullOrEmpty(error_uri) == false)
                {
                    resHtml += "error_uri=";
                    resHtml += error_uri;
                    resHtml += "<BR>";
                }

                LABEL1.Text = HttpUtility.HtmlEncode(resHtml);
                return;

            }
            else
            {
                //-----------------------------------------------------------------------
                // 認可成功.
                //-----------------------------------------------------------------------

                string clientID = ConfigurationManager.AppSettings["CLIENT_ID"];
                string clientSecret = ConfigurationManager.AppSettings["CLIENT_SECRET"];
                string accessTokenURL = ConfigurationManager.AppSettings["ACCESSTOKEN_URL"];
                string redirect_uri = ConfigurationManager.AppSettings["CALLBACK_URL"];

                // 認可コードを取得する.
                string code = Request["code"];
                if (string.IsNullOrEmpty(code))
                {
                    resHtml += "code is none.";
                    resHtml += "<BR>";
                    LABEL1.Text = HttpUtility.HtmlEncode(resHtml);
                    return;
                }

                // nonceをチェックする.
                string state = Request["state"];
                if (string.IsNullOrEmpty(state) && Session["state"].ToString() != state)
                {
                    resHtml += "state is Invalid.";
                    resHtml += "<BR>";
                    LABEL1.Text = HttpUtility.HtmlEncode(resHtml);
                    return;
                }

                //-----------------------------------------------------------------------
                // アクセストークンを要求するためのURLを作成.
                // 次の変数をPOSTする.
                //   認可コード.
                //   コールバックURL.認可時に使用したものと同じ.
                //   グラントタイプ
                //   client_id
                //   client_secret
                //-----------------------------------------------------------------------

                string url = accessTokenURL;

                System.Net.WebClient wc = new System.Net.WebClient();

                // POSTデータの作成.
                System.Collections.Specialized.NameValueCollection ps =
                    new System.Collections.Specialized.NameValueCollection();

                //アプリケーションに渡された認可コード。
                ps.Add("code", code);

                // コールバックURL.
                ps.Add("redirect_uri", redirect_uri);

                // グラントタイプ.
                // 認可コードをアクセストークンに交換する場合は「authorization_code」を指定する。
                ps.Add("grant_type", "authorization_code");

                // BASIC認証でclient_secretを渡すか、
                // POSTでclient_idとclient_secret各種の値を渡す.
                ps.Add("client_id", clientID);
                ps.Add("client_secret", clientSecret);

                //データを送受信する
                byte[] resData = wc.UploadValues(url, ps);
                wc.Dispose();

                //受信したデータを表示する
                string resText = System.Text.Encoding.UTF8.GetString(resData);

                // レスポンスはサービスによって変わる。
                // Googleの場合はJSON
                // Facebookの場合はフォームエンコードされた&区切りのKey=Valueが返る。
                // 返ってくる可能性があるパラメータは次の通り
                //
                // access_token  APIリクエストを認可するときに使用するトークン。
                // token_type  発行されたアクセストークンの種類。多くの場合は「bearer」だが、拡張としていくつかの値を取り得る。
                // アクセストークンには期限が付与されていることがあります。その場合、さらに次のような情報が追加されています。
                // expires_in   アクセストークンの有効期限の残り（秒数）。
                // refresh_token  リフレッシュトークン。現在のアクセストークンの期限が切れた後、新しいアクセストークンを取得するために使う.
                // リフレッシュトークンを入手した場合はユーザーがキーボードの前にいなくてもデータにアクセス可能となる。
                // リフレッシュトークンはセキュアなストアに保存する。

                Dictionary<string, string> dict = new Dictionary<string,string>();

                string[] stArrayData = resText.Split('&');
                foreach (string stData in stArrayData)
                {
                    string[] keyValueData = stData.Split('=');
                    Session[keyValueData[0]] = keyValueData[1];
                }

                Response.Redirect("feedview.aspx");
            }
        }
    }
}