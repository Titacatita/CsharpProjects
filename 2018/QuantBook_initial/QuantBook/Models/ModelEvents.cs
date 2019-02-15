using System;
using System.Collections.Generic;

namespace QuantBook.Models
{
    public class ModelEvents
    {
        public List<object> EventList { get; private set; }
        public ModelEvents(List<object> objects)
        {
            this.EventList = objects;
        }
    }
}
