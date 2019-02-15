using System;
using Bloomberglp.Blpapi;
using System.Data;

namespace QuantBook.Models.Bloomberg
{
    public class BloombergHelper
    {
        public static DataTable BloombergHistData(string[] tickers, string[] fieldNames, DateTime startDate, DateTime endDate)
        {
            // Setup session parameters: server, port, buffer size, etc...
            SessionOptions opt = new SessionOptions();
            opt.ServerHost = "127.0.0.1"; // ip address of localhost
            opt.ServerPort = 8194;
            // Establish/Start Session
            Session session = new Session(opt);
            if (!session.Start())
            {
                System.Console.Error.WriteLine("Failed to start session");
                return null;
            }

            if (!session.OpenService("//blp/refdata"))
            {
                System.Console.Error.WriteLine("Failed to open service");
                return null;
            }
            Service refService = session.GetService("//blp/refdata");
            Request request = refService.CreateRequest("HistoricalDataRequest");

            for (int i = 0; i < tickers.Length; i++)
            {
                request.GetElement("securities").AppendValue(tickers[i]);
            }

            for (int i = 0; i < fieldNames.Length; i++)
            {
                request.GetElement("fields").AppendValue(fieldNames[i]);
            }

            request.Set("periodicityAdjustment", "ACTUAL");
            request.Set("periodicitySelection", "DAILY");
            request.Set("startDate", startDate.ToString("yyyyMMdd"));
            request.Set("endDate", endDate.ToString("yyyyMMdd"));
            request.Set("maxDataPoints", 1000000);
            session.SendRequest(request, null);

            DataTable dt = new DataTable();
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("Name", typeof(string));
            for (int i = 0; i < fieldNames.Length; i++)
            {
                dt.Columns.Add(fieldNames[i], typeof(object));
            }

            bool isContinue = true;
            while (isContinue)
            {
                Event eventObj = session.NextEvent();
                foreach (Bloomberglp.Blpapi.Message msg in eventObj.GetMessages())
                {
                    if (msg.HasElement("securityData"))
                    {
                        Element secs = msg.GetElement("securityData");
                        Element flds = secs.GetElement("fieldData");

                        for (int j = 0; j < flds.NumValues; j++)
                        {
                            Element f = flds.GetValueAsElement(j);
                            object[] res = new object[fieldNames.Length];
                            for (int k = 0; k < res.Length; k++)
                            {
                                res[k] = null;
                            }

                            for (int k = 0; k < res.Length; k++)
                            {
                                if (f.HasElement(fieldNames[k]))
                                    res[k] = f.GetElementAsFloat64(fieldNames[k]);
                            }

                            DataRow row = dt.NewRow();
                            row["Date"] = f.GetElementAsDate("date").ToSystemDateTime();
                            row["Name"] = secs.GetElementAsString("security");
                            for (int k = 0; k < res.Length; k++)
                            {
                                row[2 + k] = res[k];
                            }

                            dt.Rows.Add(row);
                        }
                    }
                }
                if (eventObj.Type == Event.EventType.RESPONSE)
                    isContinue = false;
            }

            dt = ModelHelper.DatatableSort(dt, "Name ASC, Date ASC");
            session.Stop();
            return dt;
        }


        public static DataTable BloombergReferenceData(string[] tickers, string[] fieldNames)
        {
            SessionOptions opt = new SessionOptions();
            opt.ServerHost = "127.0.0.1";
            opt.ServerPort = 8194;
            Session session = new Session(opt);
            if (!session.Start())
            {
                System.Console.Error.WriteLine("Failed to start session");
                return null;
            }

            if (!session.OpenService("//blp/refdata"))
            {
                System.Console.Error.WriteLine("Failed to open service");
                return null;
            }
            Service refService = session.GetService("//blp/refdata");
            Request request = refService.CreateRequest("ReferenceDataRequest");

            for (int i = 0; i < tickers.Length; i++)
            {
                request.Append("securities", tickers[i]);
            }

            for (int i = 0; i < fieldNames.Length; i++)
            {
                request.Append("fields", fieldNames[i]);
            }

            session.SendRequest(request, null);

            DataTable dt = new DataTable();
            dt.Columns.Add("Security", typeof(string));
            for (int i = 0; i < fieldNames.Length; i++)
            {
                dt.Columns.Add(fieldNames[i], typeof(object));
            }


            bool isContinue = true;
            while (isContinue)
            {
                Event eventObj = session.NextEvent();
                foreach (Bloomberglp.Blpapi.Message msg in eventObj.GetMessages())
                {
                    if (msg.HasElement("securityData"))
                    {

                        int nv = msg.GetElement("securityData").NumValues;
                        for (int i = 0; i < nv; i++)
                        {
                            object[] res = new object[fieldNames.Length];
                            for (int j = 0; j < fieldNames.Length; j++)
                            {
                                res[j] = null;
                            }

                            Element sec = msg.GetElement("securityData").GetValueAsElement(i);
                            Element fld = sec.GetElement("fieldData");

                            for (int j = 0; j < fieldNames.Length; j++)
                            {
                                if (fld.HasElement(fieldNames[j]))
                                    res[j] = fld.GetElementAsFloat64(fieldNames[j]);
                            }

                            DataRow row = dt.NewRow();
                            row["Security"] = sec.GetElementAsString("security");
                            for (int j = 0; j < fieldNames.Length; j++)
                            {
                                row[j + 1] = res[j];
                            }
                            dt.Rows.Add(row);
                        }
                    }
                }
                if (eventObj.Type == Event.EventType.RESPONSE)
                    isContinue = false;
            }

            session.Stop();
            return dt;
        }

        public static DataTable BloombergTickData(string ticker, DateTime startDate, DateTime endDate)
        {
            // Setup session parameters: server, port, buffer size, etc...
            SessionOptions opt = new SessionOptions();
            opt.ServerHost = "127.0.0.1"; // ip address of localhost
            opt.ServerPort = 8194;
            // Establish/Start Session
            Session session = new Session(opt);
            if (!session.Start())
            {
                System.Console.Error.WriteLine("Failed to start session");
                return null;
            }

            if (!session.OpenService("//blp/refdata"))
            {
                System.Console.Error.WriteLine("Failed to open service");
                return null;
            }
            Service refService = session.GetService("//blp/refdata");
            Request request = refService.CreateRequest("IntradayTickRequest");

            request.Set("security", ticker);
            request.Append("eventTypes", "TRADE");
            //request.Append("eventTypes", "BID");
            request.Set("startDateTime", new Datetime(startDate.Year, startDate.Month, startDate.Day, 13, 30, 0, 0));
            request.Set("endDateTime", new Datetime(endDate.Year, endDate.Month, endDate.Day, 21, 30, 0, 0));

            request.Set("includeConditionCodes", false); 

            session.SendRequest(request, null);

            DataTable stock_table = new DataTable();
            stock_table.Columns.Add("QuoteTime", typeof(DateTime));
            stock_table.Columns.Add("Value", typeof(double));

            bool isContinue = true;
            while (isContinue)
            {
                Event eventObj = session.NextEvent();
                foreach (Bloomberglp.Blpapi.Message msg in eventObj.GetMessages())
                {
                    if (msg.HasElement("tickData"))
                    {
                        Element data = msg.GetElement("tickData").GetElement("tickData");
                        int num = data.NumValues;

                        for (int i = 0; i < num; i++)
                        {
                            Element bar = data.GetValueAsElement(i);

                            if (bar.NumElements > 1)
                            {
                                stock_table.Rows.Add(bar.GetElementAsDate("time").ToSystemDateTime().AddHours(-4),
                                                     bar.GetElementAsFloat64("value"));
                            }
                        }
                    }
                }

                if (eventObj.Type == Event.EventType.RESPONSE)
                    isContinue = false;
            }

            stock_table = ModelHelper.DatatableSort(stock_table, "QuoteTime ASC");
            session.Stop();
            return stock_table;
        }




        public static DataTable BloombergBarData(string ticker, int barInterval, DateTime startDate, DateTime endDate)
        {
            // Setup session parameters: server, port, buffer size, etc...
            SessionOptions opt = new SessionOptions();
            opt.ServerHost = "127.0.0.1"; // ip address of localhost
            opt.ServerPort = 8194;
            // Establish/Start Session
            Session session = new Session(opt);
            if (!session.Start())
            {
                System.Console.Error.WriteLine("Failed to start session");
                return null;
            }

            if (!session.OpenService("//blp/refdata"))
            {
                System.Console.Error.WriteLine("Failed to open service");
                return null;
            }
            Service refService = session.GetService("//blp/refdata");
            Request request = refService.CreateRequest("IntradayBarRequest");
            request.Set("security", ticker);
            request.Set("eventType", "TRADE");
            request.Set("interval", barInterval);
            request.Set("startDateTime", new Datetime(startDate.Year, startDate.Month, startDate.Day, 13, 30, 0, 0));
            request.Set("endDateTime", new Datetime(endDate.Year, endDate.Month, endDate.Day, 21, 30, 0, 0));

            session.SendRequest(request, null);

            DataTable stock_table = new DataTable();
            stock_table.Columns.Add("Date", typeof(DateTime));
            stock_table.Columns.Add("Open", typeof(double));
            stock_table.Columns.Add("High", typeof(double));
            stock_table.Columns.Add("Low", typeof(double));
            stock_table.Columns.Add("Close", typeof(double));
            stock_table.Columns.Add("Volume", typeof(double));

            bool isContinue = true;
            while (isContinue)
            {
                Event eventObj = session.NextEvent();
                foreach (Bloomberglp.Blpapi.Message msg in eventObj.GetMessages())
                {
                    if (msg.HasElement("barData"))
                    {
                        Element data = msg.GetElement("barData").GetElement("barTickData");
                        int num = data.NumValues;

                        for (int i = 0; i < num; i++)
                        {
                            Element bar = data.GetValueAsElement(i);

                            if (bar.NumElements > 1)
                            {
                                stock_table.Rows.Add(bar.GetElementAsDate("time").ToSystemDateTime().AddHours(-4),
                                                     bar.GetElementAsFloat64("open"), 
                                                     bar.GetElementAsFloat64("high"),
                                                     bar.GetElementAsFloat64("low"),
                                                     bar.GetElementAsFloat64("close"),
                                                     bar.GetElementAsInt64("volume"));
                            }
                        }
                    }
                }
                if (eventObj.Type == Event.EventType.RESPONSE)
                    isContinue = false;
            }

            stock_table = ModelHelper.DatatableSort(stock_table, "Date ASC");
            session.Stop();
            return stock_table;
        }
    }
}
