﻿using System;
using System.Collections.Generic;
using System.Text;

namespace timeRecorder.Common.Model
{
    public class Timer
    {
        public int IdEmployee { get; set; }

        public DateTime Registry { get; set; }

        public int RegistryType { get; set; }

        public bool WrapRegistries { get; set; }
    }
}
