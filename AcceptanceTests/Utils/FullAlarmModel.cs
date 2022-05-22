using System;
using System.Collections.Generic;
using System.Text;
using TimberyardClient.Client;

namespace AcceptanceTests.Utils
{
    public class FullAlarmModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Objective { get; set; }
        public Field Field { get; set; }
        public int Threshold { get; set; }
        public bool Active { get; set; }
        public List<string> Receivers { get; set; }
    }
}
