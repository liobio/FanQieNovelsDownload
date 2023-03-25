using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQieNovelsDownload
{
    public class Novles
    {
        string author;
        string bookId;
        string bookName;
        string currentItemId;
        string chapterCount;
        public Novles(string author, string bookId, string bookName, string currentItemId, string chapterCount)
        {
            Author = author;
            BookId = bookId;
            BookName = bookName;
            CurrentItemId = currentItemId;
            ChapterCount = chapterCount;
        }

        public Novles()
        {

        }

        public string Author { get => author; set => author = value; }
        public string BookId { get => bookId; set => bookId = value; }
        public string BookName { get => bookName; set => bookName = value; }
        public string CurrentItemId { get => currentItemId; set => currentItemId = value; }
        public string ChapterCount { get => chapterCount; set => chapterCount = value; }
    }
}
