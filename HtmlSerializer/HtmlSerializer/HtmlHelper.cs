using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    public class HtmlHelper
    {
        
      private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public List<string> HtmlTags { get; set; }
        public List<string> HtmlVoidTags { get; set; }
        private HtmlHelper() {
              try
             {
                HtmlTags = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("Json/HtmlTags.json")) ?? new List<string>();
                HtmlVoidTags = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("Json/HtmlVoidTags.json")) ?? new List<string>();
             }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"שגיאה באתחול HtmlHelper: {ex.Message}");
                HtmlTags = new List<string>();
                HtmlVoidTags = new List<string>();
            }


        }
    }
}
