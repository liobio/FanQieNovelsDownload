namespace FanQieNovelsDownload
{
    using LitJson;
    using System;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using static System.Net.WebRequestMethods;


    public class Program
    {


        public static List<Novles> novlesList = new();
        public static void Main(string[] args)
        {
            //https://fanqienovel.com/api/reader/full?itemId=6982735801973113351章节
            //https://fanqienovel.com/api/reader/directory/detail?bookId=6982529841564224526章节json
            //https://fanqienovel.com/page/6982529841564224526详细内容



            string json = String.Empty;
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetName().Name.ToString() + ".NovelsList.json";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    json = sr.ReadToEnd();
                }
            }
            novlesList = JsonMapper.ToObject<List<Novles>>(json);
            GetNovelsList();//更新list且保存json
            var requestContent = new Requests();
            for (int i = 0; i < novlesList.Count; i++)
            {
                DownloadOneNovel(novlesList[i], requestContent);
            }


        }
        private static void DownloadOneNovel(Novles novle, Requests requestContent)
        {
            Console.WriteLine();
            for (int i = 0; i < int.Parse(novle.ChapterCount); i++)
            {
                string conentUrl = " https://fanqienovel.com/api/reader/full?itemId=";
                string path = "D:\\_WinForm\\FanQieNovelsDownload\\";
                HttpResponseMessage response = requestContent.Get(conentUrl + novle.CurrentItemId);
                JsonData chapterData = JsonMapper.ToObject(response.Text())["data"]["chapterData"];
                string title = chapterData["title"].ToString();
                string content = title + "\n\n";

                StreamWriter sw = new StreamWriter(path + novle.BookName + ".txt", true);
                try
                {
                    foreach (string item in MidStrEx2List(chapterData["content"].ToString(), "<p>", "</p>"))
                    {
                        content += item.ToString() + "\n\n";
                    }
                    sw.Write(content);
                    novlesList[0].CurrentItemId = chapterData["nextItemId"].ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                finally
                {
                    sw.Close();
                    Console.Write(novle.BookName + ":" + i + "/" + novle.ChapterCount);
                }
            }
        }


        /// <summary>
        /// 已弃用 返回json的T对象枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static IEnumerable<T> JsonString2IEnumerable<T>(string jsonStr) where T : new()
        {
            IEnumerable<T> items = new T[] { };
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            foreach (JsonData jd in jsonData)  //对象Array里每一个元素，都转换为json再转换为对象
            {
                string s = Regex.Unescape(JsonMapper.ToJson(jd));
                items.Add(JsonMapper.ToObject<T>(s));
            }
            return items;
        }

        public static void GetNovelsList()
        {
            var requestJson = new Requests();
            var requestHtml = new Requests();
            int sort = 3;
            int page_index = 6;

            string originUrl_1 = "https://fanqienovel.com/api/author/library/book_list/v0/?page_count=18&page_index=";
            string originUrl_2 = "&gender=-1&category_id=-1&creation_status=-1&word_count=-1&sort=";
            string detailedContentUrl = "https://fanqienovel.com/page/";
            string chapterListUrl = "https://fanqienovel.com/api/reader/directory/detail?bookId=";
            string path = "D:\\_WinForm\\FanQieNovelsDownload\\NovelsList.json";
            for (int i = 0; i < sort; i++)
            {
                for (int j = 0; j < page_index; j++)
                {
                    string url = string.Format("{0}{1}{2}{3}", originUrl_1, j.ToString(), originUrl_2, i.ToString());
                    HttpResponseMessage novelsResponse = requestJson.Get(url);
                    JsonData jsonData = JsonMapper.ToObject(novelsResponse.Text());
                    JsonData data = jsonData["data"];
                    JsonData book_list = data["book_list"];
                    foreach (JsonData item in book_list)
                    {

                        JsonData book_idValue = item["book_id"];
                        string book_id = book_idValue.ToString();
                        if (novlesList.Find(n => n.BookId == book_id) != null)
                        {
                            continue;
                        }
                        HttpResponseMessage detailedContentResponse;
                        while (true)
                        {
                            detailedContentResponse = requestHtml.Get(detailedContentUrl + book_id);
                            if (detailedContentResponse.Content.Headers.ContentLength == 0)
                            {
                                continue;
                            }
                            string str = detailedContentResponse.Text();
                            string book_name = MidStrEx2FirstValue(str, "<title>", "完整版");
                            if (book_name == "")
                            {
                                break;
                            }
                            string author = MidStrEx2FirstValue(str, "全文阅读,", "小说");
                            HttpResponseMessage response = requestJson.Get(chapterListUrl + book_id);
                            JsonData chapterListData = JsonMapper.ToObject(response.Text())["data"];
                            JsonData allItemIdsData = chapterListData["allItemIds"];
                            string chapterCount = chapterListData.Count.ToString();
                            string currentItemId = allItemIdsData[0].ToString();
                            novlesList.Add(new Novles(author, book_id, book_name, currentItemId, chapterCount));
                            Console.WriteLine($"已找到：{book_name},{book_id}");
                            break;
                        }


                    }
                }
            }
            StreamWriter sw = new StreamWriter(path);
            try
            {
                sw.Write(Regex.Unescape(JsonMapper.ToJson(novlesList)));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                sw.Close();
                Console.WriteLine("novlesList Executing finally block.");
            }
        }

        public static string MidStrEx2FirstValue(string sourse, string startstr, string endstr)
        {
            Regex rg = new("(?<=(" + startstr + "))[.\\s\\S]*?(?=(" + endstr + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(sourse).Value;
        }
        public static List<string> MidStrEx2List(string sourse, string startstr, string endstr)
        {
            List<string> list = new List<string>();
            Regex rg = new("(?<=(" + startstr + "))[.\\s\\S]*?(?=(" + endstr + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            foreach (Match item in rg.Matches(sourse))
            {
                list.Add(item.Value);
            }
            return list;
        }


    }

}



