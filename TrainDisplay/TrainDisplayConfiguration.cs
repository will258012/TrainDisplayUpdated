using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrainDisplay
{
    [ConfigurationPath("AsmapeTrainDisplay.xml")]
    public class TrainDisplayConfiguration
    {
        public int DisplayWidth { get; set; } = 512;
    }
}
