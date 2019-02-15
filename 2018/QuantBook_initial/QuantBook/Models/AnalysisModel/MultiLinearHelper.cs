using Accord.Statistics.Analysis;
using Accord.Math;
using QuantBook.Models.DataModel;
using System;
using System.Data;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Linq;

namespace QuantBook.Models.AnalysisModel
{
    public static  class MultiLinearHelper
    {
        public static MultiLinearResult GetMultiRegression(DataTable dt, string[] tickers)
        {
            string depedentName = tickers[0];
            string[] independentNames = tickers.ToList().GetRange(1, tickers.Length - 1).ToArray();
            double[][] input = dt.DefaultView.ToTable(false, independentNames).ToArray();
            double[] output = dt.DefaultView.ToTable(false, depedentName).Columns[depedentName].ToArray();

            MultipleLinearRegressionAnalysis mvr = new MultipleLinearRegressionAnalysis(input, output, independentNames, depedentName, true);
            mvr.Compute();
            double[] val = new double[tickers.Length];
          
            for (int i = 0; i < tickers.Length; i++)
                val[i] = mvr.Coefficients[i].Value;

            foreach (DataRow p in dt.Rows)
            {
                double price = val[val.Length - 1];
                for (int i = 0; i < tickers.Length - 1; i++)
                    price += val[i] * Convert.ToDouble(p[tickers[i + 1]]);

                double spread = Convert.ToDouble(p[tickers[0]]) - price;

                p["MrComponent"] = price;
                p["Spread"] = spread;
            }

            return new MultiLinearResult
            {
                RSquared = mvr.RSquared,
                RSquaredAdj = mvr.RSquareAdjusted,
                Coefficients = mvr.Coefficients,
                Anova = mvr.Table
            };
        }

      
        public static DataTable GetMultiLinearDataIndex(DateTime startDate, DateTime endDate)
        {
            string[] tickers = new string[] { "HY", "SPX", "VIX" };
            DataTable res = new DataTable();
            res.Columns.Add("Date", typeof(DateTime));
            foreach (string tk in tickers)
                res.Columns.Add(tk, typeof(double));

            Caliburn.Micro.BindableCollection<IndexData> idx = Dal.GetIndexData(startDate, endDate);
            foreach (IndexData p in idx)
                res.Rows.Add(p.Date, p.HYSpread, p.SPX, p.VIX);           

            res.Columns.Add("MrComponent", typeof(double));
            res.Columns.Add("Spread", typeof(double));
          
            return res;
        }


        public static DataTable GetMultiLienarDataStock(string[] tickers, DateTime startDate, DateTime endDate)
        {
            DataTable res = new DataTable();
            res = Dal.GetMultiStockData(tickers, startDate, endDate, "AdjClose", DataSourceEnum.Yahoo);            
            res.Columns.Add("MrComponent", typeof(double));
            res.Columns.Add("Spread", typeof(double));
            return res;
        }












        #region PCA
        private static void ProcessMultiPca(DataTable dt, string[] tickers)
        {
            string[] independentNames = tickers.ToList().GetRange(1, tickers.Length - 1).ToArray();
            double[,] data = dt.ToMatrix(independentNames);
            PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis(data, AnalysisMethod.Standardize);
            pca.Compute();            
            double[,] finalData = pca.Transform(data); 

            double avg = (double)dt.Compute("Avg(" + tickers[0] + ")", "");
            double std = (double)dt.Compute("StDev(" + tickers[0] + ")", "");
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["MrComponent"] = finalData[i, 0];
                dt.Rows[i]["Spread"] = (dt.Rows[i][tickers[0]].To<double>() - avg) / std;
            }
            dt.Columns["Spread"].ColumnName = tickers[0] + "1";
            dt.Columns["MrComponent"].ColumnName = "PcaComponent";
        }

        public static List<SimpleLinearResult> GetMultiPca(DataTable dt, string[] tickers)
        {
            List<SimpleLinearResult> resList = new List<SimpleLinearResult>();

            ProcessMultiPca(dt, tickers);

            List<double> xData = new List<double>();
            List<double> yData = new List<double>();
            foreach (DataRow row in dt.Rows)
            {
                xData.Add(row["PcaComponent"].To<double>());
                yData.Add(row[tickers[0] + "1"].To<double>());
            }
            SimpleLinearResult res = LinearAnalysisHelper.GetSimplePca(xData, yData);
            if (!dt.Columns.Contains("PricePca"))
                dt.Columns.Add("PricePca", typeof(double));
            List<double> pcaList = LinearAnalysisHelper.GenerateData(xData, res.Alpha, res.Beta);
            for (int i = 0; i < pcaList.Count; i++)
                dt.Rows[i]["PricePca"] = pcaList[i];
            resList.Add(res);

            res = LinearAnalysisHelper.GetSimpleRegression(xData, yData);
            if (!dt.Columns.Contains("PriceSlr"))
                dt.Columns.Add("PriceSlr", typeof(double));
            List<double> slrList = LinearAnalysisHelper.GenerateData(xData, res.Alpha, res.Beta);
            for (int i = 0; i < slrList.Count; i++)
                dt.Rows[i]["PriceSlr"] = slrList[i];
            resList.Add(res);

            return resList;
        }

        #endregion PCA

    }

    public class MultiLinearResult
    {
        public double RSquared { get; set; }
        public double RSquaredAdj { get; set; }
        public LinearRegressionCoefficientCollection Coefficients { get; set; }
        public Accord.Statistics.Testing.AnovaSourceCollection Anova { get; set; }
    }
}
