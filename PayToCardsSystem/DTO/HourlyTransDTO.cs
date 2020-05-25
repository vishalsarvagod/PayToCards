using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayToCardsSystem.DTO
{
    public class HourlyTransDTO
    {

        public string type { get; set; }
        public Data data { get; set; }
        public Options options { get; set; }
        public class Options
        {
            public Title title { get; set; }
        }
        public class Title
        {
            public bool display { get; set; }
            public string text { get; set; }
        }
        public class Data
        {
            public List<string> labels { get; set; }
            public List<Dataset> datasets { get; set; }
        }

        public class Dataset
        {
            public string label { get; set; }
            public List<int> data { get; set; }
            public string borderColor { get; set; }
        }
    }
}