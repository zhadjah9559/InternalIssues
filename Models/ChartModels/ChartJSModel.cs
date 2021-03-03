using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Models.ChartModels
{
    public class ChartJSModel
    {
        //overloaded ctor
        public ChartJSModel()
        {
            Labels = new List<string>();
            Data = new List<int>();
            BackgroundColors = new List<string>();
        }
                
        public List<string> Labels { get; set; }
        public List<string> BackgroundColors { get; set; }

        public List<int> Data { get; set; }


    }
}
