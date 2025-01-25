// See https://aka.ms/new-console-template for more information
using HtmlSerializer;
using System.Text.RegularExpressions;
 HtmlElement BuildTree(IEnumerable<string> htmlLines, List<string> htmlTags, List<string> htmlVoidTags)
{
    var root = new HtmlElement
    {
        Name = "html",
      
    };

    var elementsStack = new Stack<HtmlElement>();
    elementsStack.Push(root);
  
    foreach (var line in htmlLines)
    {
        var trimmedLine = line.Trim();

        if (trimmedLine.Equals("/html"))
        {
            break;
        }

        if (trimmedLine.StartsWith("/"))
        {
            
            var closedElement = elementsStack.Pop();


            closedElement.InnerHtml = closedElement.InnerHtml.Trim();
            continue;
        }

        var tagName = trimmedLine.Split(' ')[0];

        if (htmlTags.Contains(tagName))
        {
            if (elementsStack.Count == 0)
            {
                throw new InvalidOperationException("The stack is empty. Ensure the HTML structure is correct.");
            }
            var newElement = new HtmlElement
            {
                Name = tagName,
              

           

                Parent = elementsStack.Peek(),
            };

            var attributesRegex = new Regex("([^\\s]*?)=\"(.*?)\"");
            var matches = attributesRegex.Matches(trimmedLine);
            foreach (Match match in matches)
            {
                var attributeName = match.Groups[1].Value.Trim();
                var attributeValue = match.Groups[2].Value.Trim();

                if (string.IsNullOrWhiteSpace(attributeValue))
                {
                    continue;
                }

                if (attributeName == "class")
                {
                    var classes = attributeValue.Split(' ')
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .Distinct();

                    foreach (var className in classes)
                    {
                        if (!newElement.Classes.Contains(className))
                        {
                            newElement.Classes.Add(className);
                        }
                    }
                }
                else if (attributeName == "id")
                {
                    if (string.IsNullOrWhiteSpace(newElement.Id))
                    {
                        newElement.Id = attributeValue;
                    }
                }
                else
                {
                    var attributeEntry = $"{attributeName}={attributeValue}";
                    if (!newElement.Attribute.Contains(attributeEntry))
                    {
                        newElement.Attribute.Add(attributeEntry);
                    }
                }
            }

            elementsStack.Peek().Children.Add(newElement);

            if (!htmlVoidTags.Contains(tagName))
            {
                elementsStack.Push(newElement);
            }
            else
            {
             
                newElement.InnerHtml = string.Empty;
                newElement.Children = null;
            }
        }
        else
        {
           
            elementsStack.Peek().InnerHtml += " " + trimmedLine;
        }
    }

    return root;
}
async Task<string> Load(string url)
 {
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
 }

//main
var html = await Load("https://hebrewbooks.org/beis");


var cleanHtml = new Regex("\\s").Replace(html, " ");
var tagMatches = Regex.Matches(cleanHtml, @"<\/?([a-zA-Z][a-zA-Z0-9]*)\b[^>]*>|([^<]+)").Where(l => !String.IsNullOrWhiteSpace(l.Value));
var htmlLines = new List<string>();
foreach (Match item in tagMatches)
{
    string tag = item.Value.Trim();
    if (tag.StartsWith('<'))
        tag = tag.Trim('<', '>');
    htmlLines.Add(tag);
}
HtmlElement root = BuildTree(htmlLines, HtmlHelper.Instance.HtmlTags, HtmlHelper.Instance.HtmlVoidTags);
var result=HtmlElementExtensions.FindBySelector(root,Selector.Parse("html head"));
result.ToList().ForEach(e => Console.WriteLine(e.ToString()));
