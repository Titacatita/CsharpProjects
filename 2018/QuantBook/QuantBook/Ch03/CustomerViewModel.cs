using System.Linq;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using QuantBook.Models;
using System.Collections.Generic;
using QuantBook.Models.DataModel;

namespace QuantBook.Ch03
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CustomerViewModel : Screen
    {
        private readonly IEventAggregator _events; 
        [ImportingConstructor]
        public CustomerViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "01. Customers";
            MyCustomers = new BindableCollection<Customer>();
        }

        public BindableCollection<Customer> MyCustomers { get; set; }

        public void GetData()
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                IQueryable<Customer> query = from d in db.Customers select d;
                //var query = db.Customers.SqlQuery("SELECT * FROM Customers");
                MyCustomers.Clear();
                MyCustomers.AddRange(query);
            }
            _events.PublishOnUIThread(new ModelEvents(new List<object>(new object[] { "From Customers: Count = " + MyCustomers.Count.ToString() })));
        }

    }
}
