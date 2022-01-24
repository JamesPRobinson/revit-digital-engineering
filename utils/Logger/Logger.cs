using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace Utils.Logger
{
    public class Logger
    {
        public Logger() { }
        string IncomingWebhookUrl = "ENTERTEAMSURLHERE";
        string ImageStorage = "ENTERBLOBSTROAGEURLHERE";
        public async void PostCaptionTeams(string email, string user, string message, string path, string webhook)
        {
            webhook = string.IsNullOrWhiteSpace(webhook) ? IncomingWebhookUrl : webhook;
            string blob_url = string.Empty;
            try
            {
                string imageBase64String = Converters.ImagetoBase64(path);
                using (var client = new WebClient())
                {
                    var data = new NameValueCollection
                        {
                            { "image", imageBase64String }
                        };
                    var response  = client.UploadValues(ImageStorage, data);
                    blob_url = System.Text.Encoding.Default.GetString(response).Replace("\"", string.Empty);
                }
                
            if (!string.IsNullOrEmpty(blob_url))
            {
                string m = JsonConvert.SerializeObject(new Message()
                {
                    type = "message",
                    attachments = new List<Attachment>()
                {
                    new Attachment()
                    {
                        contentType = "application/vnd.microsoft.card.adaptive",
                        content = new Content()
                        {
                            Schema = "http://adaptivecards.io/schemas/adaptive-card.json",
                            type = "AdaptiveCard",
                            version = "1.0",
                            body = new List<Body>()
                            {
                                new Body()
                                {
                                    type = "Container",
                                    items = new List<Item>()
                                    {
                                        new Item()
                                        {
                                            type = "TextBlock",
                                            text = "User: " + user ,
                                            weight = "bolder",
                                            size = "medium"
                                        },
                                        new Item()
                                        {
                                            type = "TextBlock",
                                            text = "Email: " + email,
                                            weight = "bolder",
                                            size = "medium"
                                        },
                                        new Item()
                                        {
                                            type = "ColumnSet",
                                            columns = new List<Column>()
                                            {
                                                new Column()
                                                {
                                                    type = "Column",
                                                    width = "auto",
                                                    items = new List<Item>()
                                                    {
                                                        new Item()
                                                        {
                                                            type = "Image",
                                                            url = blob_url,
                                                            width = "750px",
                                                            height = "500px"
                                                        }
                                                    }
                                                },
                                            }
                                        },
                                        new Item()
                                        {
                                            type = "TextBlock",
                                            text = "Message",
                                            weight = "bolder",
                                            wrap = true,
                                        },
                                        new Item()
                                        {
                                            type = "TextBlock",
                                            text = message,
                                            isSubtle = true,
                                            wrap = true,
                                        },
                                        new Item()
                                        {
                                            type = "TextBlock",
                                            spacing = "none",
                                            text = "CREATED: " + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"),
                                            isSubtle = true,
                                            wrap = true
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                });
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        using (var teams_client = new HttpClient())
                        {
                            var content = new StringContent(m, Encoding.UTF8, "application/json");
                            HttpResponseMessage resp = await teams_client.PostAsync(webhook, content);
                            Task<string> c = resp.Content.ReadAsStringAsync();
                            var x = c.Result;
                            if (resp.IsSuccessStatusCode)
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        PostErrorTeams(user, ex.ToString(), string.Empty);
                    }

                }

            }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        public async void PostErrorTeams(string user, string message, string webhook)
        {
            webhook = string.IsNullOrWhiteSpace(webhook) ? IncomingWebhookUrl : webhook;
            string m = JsonConvert.SerializeObject(new TextMessage() { title = user, text = message });
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var content = new StringContent(m, Encoding.UTF8, "application/json");
                        HttpResponseMessage resp = await client.PostAsync(IncomingWebhookUrl, content);
                        if (resp.IsSuccessStatusCode)
                        {
                            break;
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }
 
    public class TextMessage
    {
        public string title;
        public string text;
    }
}