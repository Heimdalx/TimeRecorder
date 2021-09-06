using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace timeRecorder.Function.Entities
{
    public class TimeRecorderEntity : TableEntity
    {
        public int IdEmployee { get; set; }

        public DateTime Registry { get; set; }

        public int RegistryType { get; set; }

        public bool WrapRegistries { get; set; }
    }
}
