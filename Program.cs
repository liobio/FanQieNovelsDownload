namespace FanQieNovelsDownload
{
    using LitJson;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;


    public class Program
    {
        public static List<Novles> novlesList = new();
        public static async Task Main(string[] args)
        {
            //https://fanqienovel.com/api/reader/full?itemId=6982735801973113351章节
            //https://fanqienovel.com/api/reader/directory/detail?bookId=6982529841564224526章节json
            //https://fanqienovel.com/page/6982529841564224526详细内容
            GetNovelsList();

        }

        private static void GetNovelsList()
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
                        HttpResponseMessage detailedContentResponse;
                        while (true)
                        {
                            detailedContentResponse = requestHtml.Get(detailedContentUrl + book_id);
                            if (detailedContentResponse.Content.Headers.ContentLength == 0)
                            {
                                continue;
                            }
                            string str = detailedContentResponse.Text();
                            string book_name = MidStrEx_New(str, "<title>", "完整版");
                            string author = MidStrEx_New(str, "全文阅读,", "小说");
                            HttpResponseMessage response = requestJson.Get(chapterListUrl + book_id);
                            JsonData chapterListData = JsonMapper.ToObject(response.Text())["data"];
                            JsonData allItemIdsData = chapterListData["allItemIds"];
                            string currentItemId = allItemIdsData[0].ToString();
                            novlesList.Add(new Novles(author, book_id, book_name, currentItemId));
                            Console.WriteLine($"已找到：{book_name},{book_id}");
                            break;
                        }


                    }
                }
            }
            try
            {
                StreamWriter sw = new StreamWriter(path);

                sw.Write(Regex.Unescape(JsonMapper.ToJson(novlesList)));
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("novlesList Executing finally block.");
            }
        }

        public static string MidStrEx_New(string sourse, string startstr, string endstr)
        {
            Regex rg = new("(?<=(" + startstr + "))[.\\s\\S]*?(?=(" + endstr + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(sourse).Value;
        }
        public class Novles
        {
            string? author;
            string? bookId;
            string? bookName;
            string? currentItemId;

            public Novles(string? author, string? bookId, string? bookName, string? currentItemId)
            {
                Author = author;
                BookId = bookId;
                BookName = bookName;
                CurrentItemId = currentItemId;
            }

            public string? Author { get => author; set => author = value; }
            public string? BookId { get => bookId; set => bookId = value; }
            public string? BookName { get => bookName; set => bookName = value; }
            public string? CurrentItemId { get => currentItemId; set => currentItemId = value; }
        }

    }

}



