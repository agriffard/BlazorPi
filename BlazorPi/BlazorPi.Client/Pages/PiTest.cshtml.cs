using BlazorPi.Client.Services;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;

namespace BlazorPi.Client.Pages
{
    public class PiTestModel : BlazorComponent
    {
        [Inject]
        protected PiState State { get; set; }

        public string PiDecimals = "14159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535940812848111745028410270193852110555964462294895493038196442881097566593344612847564823378678316527120190914564856692346034861045432664821339360726024914127372458700660631558817488152092096282925409171536436789259036001133053054882046652138414695194151160943305727036575959195309218611738193261179310511854807446237996274956735188575272489122793818301194912";

        public bool ShowPi { get; set; } = false;
        public bool ShowGenerate { get; set; } = true;
        public bool ShowText { get; set; } = false;
        public string Lcp { get; set; } = "";
        public int LineLength { get; set; } = 10;
        public int ChunkLength { get; set; } = 2;
        public string SplitChar { get; set; } = " ";
        public int NbErrors = 0;

        private string _test = "";
        public string Test
        {
            get => _test;
            set
            {
                _test = value;
                Lcp = GetCommonPrefix(PiDecimals, CleanTest);

                if (!string.IsNullOrEmpty(_test) && !ResultIsLongest)
                {
                    NbErrors += 1;
                }
            }
        }

        public string CleanTest
        {
            get => Regex.Replace(Test, @"\n|\r|\s|\t|" + SplitChar, "");
        }

        private int _testLength = 200;
        public string TestLength
        {
            get => _testLength.ToString();
            set
            {
                Int32.TryParse(value, out _testLength);
                if (_testLength > PiDecimals.Length)
                {
                    string pi = State.CalculatePi(_testLength);
                    PiDecimals = pi.Substring(1);
                }
                Test = PiDecimals.Substring(0, _testLength);
                if (LineLength > 0)
                {
                    Test = SplitText(Test, LineLength, ChunkLength, SplitChar);
                }
            }
        }

        private string _text = "";
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                Test = GetWordLengthFromText(Text);
            }
        }
        public bool PiContainsResult
        {
            get
            {
                return PiDecimals.StartsWith(CleanTest);
            }
        }
        public bool ResultIsLongest
        {
            get
            {
                return CleanTest.Length == Lcp.Length;
            }
        }
        public string RemainingNumbers
        {
            get
            {
                return string.IsNullOrEmpty(Lcp) ? "" : CleanTest.Replace(Lcp, "");
            }
        }

        public static string GetCommonPrefix(string a, string b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            if (b == null)
                throw new ArgumentNullException(nameof(b));

            var min = Math.Min(a.Length, b.Length);
            var sb = new StringBuilder(min);
            for (int i = 0; i < min && a[i] == b[i]; i++)
                sb.Append(a[i]);

            return sb.ToString();
        }

        public static string GetWordLengthFromText(string textValue)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(textValue))
            {
                textValue = Regex.Replace(textValue, @"\t|\n|\r", " ");

                string[] words = textValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                result = string.Join("", words.Select(w => w.Length % 10).ToArray());
            }

            return result;
        }

        public static string SplitText(string text, int lineLength, int chunkLength, string splitChar)
        {
            string result = text;

            if (lineLength > 0)
            {
                result = Regex.Replace(text, "(.{" + lineLength + "})", "$1" + Environment.NewLine);
            }

            if (chunkLength > 0)
            {
                result = Regex.Replace(result, ".{" + chunkLength + "}", "$0" + splitChar).Trim(splitChar.ToCharArray());
            }

            return result;
        }
    }
}