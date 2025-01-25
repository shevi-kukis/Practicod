
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    public class HtmlElement
    {
        public string Id { get; set; } 
        public string Name { get; set; } 
        public List<string> Attribute { get; set; }
        public List<string> Classes { get; set; }

        public string InnerHtml { get; set; } 

        public HtmlElement Parent { get; set; } 
        public List<HtmlElement> Children { get; set; }

        public HtmlElement()
        {
            Id = "";
            Name = "";
            Attribute = new List<string>();
            Classes = new List<string>();
            InnerHtml="";
            Parent =null;
            Children =new List<HtmlElement>();
          
        }
        public IEnumerable<HtmlElement> Descendants()
        {
            var queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;

                if (current.Children != null && current.Children.Count > 0)
                {
                    foreach (var child in current.Children)
                    {
                        queue.Enqueue(child);
                    }
                }
            }
        }


        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this;
            while (Parent!=null)
            {
                yield return Parent;
                current=current.Parent;
            }
        }

        public override string ToString()
        {
            // בניית החלק של ה-ID
            string idPart = string.IsNullOrEmpty(Id) ? "" : $" id=\"{Id}\"";

            // בניית החלק של ה-Classes
            string classPart = Classes != null && Classes.Any() ? $" class=\"{string.Join(" ", Classes)}\"" : "";

            // הרכבת התוצאה
            return $"<{Name}{idPart}{classPart}>";
        }



    }
}
