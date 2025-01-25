using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlSerializer;

public class Selector
{
    public string TagName { get; set; }
    public string Id { get; set; }
    public List<string> Classes { get; set; } = new List<string>();
    public Selector Parent { get; set; }
    public Selector Child { get; set; }

    public Selector()
    {
        Parent = null;
        Child = null;
        Id = "";
        Classes = new List<string>();
        TagName = string.Empty;

    }

    public Selector(string tagName, string id, List<string> classes)
    {
        TagName = tagName;
        Id = id;
        Classes = classes ?? new List<string>();
    }

    public static Selector Parse(string query)
    {
        // if (string.IsNullOrWhiteSpace(query))
        //     throw new ArgumentException("Query string cannot be null or empty.");

        string[] levels = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        Selector root = null;
        Selector current = null;

        foreach (var level in levels)
        {
            // Parse the current selector level.
            string tagName = null;
            string id = null;
            List<string> classes = new List<string>();

            string[] parts = Regex.Split(level, @"(?=[#\\.])");


            foreach (var part in parts)
            {
                if (part.StartsWith("#"))
                {
                    id = part.Substring(1);
                }
                else if (part.StartsWith("."))
                {
                    classes.Add(part.Substring(1));
                }
                else
                {
                    if (IsValidHtmlTag(part))
                    {
                        tagName = part;
                    }
                }
            }

         
            Selector newSelector = new Selector(tagName, id, classes);

            if (root == null)
            {
                root = newSelector;
            }
            else
            {
                current.Child = newSelector;
                newSelector.Parent = current;
            }

            current = newSelector;
        }

        return root;
    }

    private static bool IsValidHtmlTag(string tag)
    {
      
        var validTags = HtmlHelper.Instance.HtmlTags;
       
        return validTags.Contains(tag);
    }

}
