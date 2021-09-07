using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace timeRecorder.Function.Entities
{
    public class WrapeEntity : TableEntity
    {
        public int IdEmployee { get; set; }

        public DateTime Date { get; set; }

        public int MinsDone { get; set; }
    }
}
