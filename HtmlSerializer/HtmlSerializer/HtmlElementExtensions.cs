using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    public static class HtmlElementExtensions
    {
        public static HashSet<HtmlElement> FindBySelector(this HtmlElement element, Selector selector)
        {
            var result = new HashSet<HtmlElement>();
            FindBySelectorRecursive(element, selector, result);
            return result;
           
        }

        private static void FindBySelectorRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
        {
            if (selector == null) return;

            var matchingElements = element.Descendants().Where(el =>
                (string.IsNullOrEmpty(selector.TagName) || el.Name == selector.TagName) &&
                (string.IsNullOrEmpty(selector.Id) || el.Id == selector.Id) &&
                (!selector.Classes.Any() || selector.Classes.All(cls => el.Classes.Contains(cls)))
            ).ToList();

            if (selector.Child == null)
            {
                matchingElements.ForEach(r => result.Add(r));
            }
            else
            {
                foreach (var match in matchingElements)
                {
                    FindBySelectorRecursive(match, selector.Child, result);
                }
            }

        }
    }
}
