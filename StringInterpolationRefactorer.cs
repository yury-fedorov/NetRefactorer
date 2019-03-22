using System;
using NetRefactorer.Interface;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Text.RegularExpressions;

namespace NetRefactorer
{
    [Export(typeof(IRefactorer))]
    public class StringInterpolationRefactorer : IRefactorer
    {
        private Regex rgx = new Regex("String.Format\\([\\s]*(\"[^\"]\")([^()]+)\\)"); // any characters

        public string Refactor(string code)
        {
            var newCode = code;
            try
            {
                IDictionary<string, string> mappings = new Dictionary<string, string>();
                var matches = rgx.Matches(code);
                foreach (Match match in matches)
                {
                    if (!mappings.ContainsKey(match.Value))
                    {
                        mappings.Add(match.Value, ToStringInterpolation(match.Value));
                    }
                }

                foreach (var mapping in mappings)
                {
                    newCode = newCode.Replace(mapping.Key, mapping.Value);
                }
            }
            catch (Exception)
            {
                // add exception processing to evidence problematic parts
                newCode = code;
            }
            return newCode;
        }

        public bool ToRefactor(string filename) => Path.GetExtension(filename) == ".cs";

        public string Wrap(object p) => "{" + p + "}";

        public string ToStringInterpolation(string stringFormat)
        {
            var match = rgx.Match(stringFormat);
            if (match.Success)
            {
                var formattingString = match.Groups[1].Value;
                var paramList = match.Groups[2].Value;
                var paramArray = paramList.Split(','); // split parameters
                // we treat only the simplest case when all parameters are just indexes without formatting
                if (paramArray.Length > 0)
                {                    
                    if (paramArray.Length == 1)
                    {
                        // extremely simplified version (only formatting string)
                        return formattingString; // we do not need string interpolation at all in this case
                    }

                    for (var index = 0; index < paramList.Length; index++)
                    {
                        // all occurancies are replaced
                        formattingString = formattingString.Replace(Wrap(index), Wrap(paramList[index+1]) );
                    }
                    return "$" + formattingString;
                }
            }
            return stringFormat;
        }
    }    
}
