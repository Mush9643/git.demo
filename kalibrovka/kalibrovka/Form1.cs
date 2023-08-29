using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using Application = Microsoft.Office.Interop.Excel.Application;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Runtime.InteropServices.ComTypes;


namespace kalibrovka
{
    public partial class Form1 : Form
    {
        private PointF[] dataPoints = { new PointF(588, 13), new PointF(767, 152), new PointF(884, 11) };
        string strFon, strAm, strCs, strC, strRad, strSr;
        private float a, b; // Коэффициенты уравнения линейной регрессии
        public Form1()
        {

            ShowIcon = false;

            InitializeComponent();

            List<string> fileNames = new List<string>(); // Создаем список для хранения имен файлов

            DirectoryInfo d = new DirectoryInfo(@"C:\Users\misha\Desktop\Работа\064\");
            FileInfo[] Files = d.GetFiles("*.xls");

            bool hasFonFile = false;
            bool hasAm241File = false;
            bool hasRadFile = false;
            bool hasSrFile = false;
            bool hasCsFile = false;
            bool hasCFile = false;

            

            foreach (FileInfo file in Files)
            {
                fileNames.Add(file.Name); // Добавляем имя файла в список

                if (file.Name == "Fon_DAI-064_14.07.2023.xls")
                {
                    hasFonFile = true; 
                    strFon = file.Name + "  - [Файл подключен]"; 
                }
                if (!hasFonFile) strFon = file.Name + "  - [Файл не найден ]";

                if (file.Name == "Am-241_DAI-064_14.07.2023.xls")
                {
                    hasAm241File = true; 
                    strAm = file.Name + "  - [Файл подключен]";
                }
                if (!hasAm241File) strAm = file.Name + "  - [Файл не найден ]";

                if (file.Name == "Спектр с прокачки ДАИ 0064 одинарный.xls")
                {
                    hasRadFile = true; 
                    strRad = file.Name + "  - [Файл подключен]";
                }
                if (!hasRadFile) strRad = file.Name + "  - [Файл не найден ]";

                if (file.Name == "Sr-90_DAI-064_14.07.2023.xls")
                {
                    hasSrFile = true; 
                    strSr = file.Name + "  - [Файл подключен]";
                }
                if (!hasSrFile) strSr = file.Name + "  - [Файл не найден ]";

                if (file.Name == "Cs-137_DAI-064_14.07.2023.xls")
                {
                    hasCsFile = true; 
                    strCs = file.Name + "  - [Файл подключен]";
                }
                if (!hasCsFile) strCs = file.Name + "  - [Файл не найден ]";

                if (file.Name == "C-14_DAI-064_14.07.2023.xls")
                {
                    hasCFile = true; 
                    strC = file.Name + "  - [Файл подключен]";
                }
                if (!hasCFile) strC = file.Name + "  - [Файл не найден ]";

            }



            ToolStripMenuItem fileItem = new ToolStripMenuItem("import Spectrum Files");
            

            ToolStripMenuItem fon = new ToolStripMenuItem(strFon) { Checked = false, CheckOnClick = true };
            fon.Click += fon_Click;
            fileItem.DropDownItems.Add(fon);

            ToolStripMenuItem americium = new ToolStripMenuItem(strAm) { Checked = false, CheckOnClick = true };
            americium.Click += americium_Click;
            fileItem.DropDownItems.Add(americium);

            ToolStripMenuItem radon = new ToolStripMenuItem(strRad) { Checked = false, CheckOnClick = true };
            radon.Click += radon_Click;
            fileItem.DropDownItems.Add(radon);

            ToolStripMenuItem strontium = new ToolStripMenuItem(strSr) { Checked = false, CheckOnClick = true };
            strontium.Click += strontium_Click;
            fileItem.DropDownItems.Add(strontium);

            ToolStripMenuItem cesium = new ToolStripMenuItem(strCs) { Checked = false, CheckOnClick = true };
            cesium.Click += cesium_Click;
            fileItem.DropDownItems.Add(cesium);

            ToolStripMenuItem carbon = new ToolStripMenuItem(strC) { Checked = false, CheckOnClick = true };
            carbon.Click += carbon_Click;
            fileItem.DropDownItems.Add(carbon);

            menuStrip1.Items.Add(fileItem);

            if (!hasFonFile) fon.BackColor = Color.Red;
            else fon.BackColor = Color.LightGreen;
   
            if (!hasAm241File) americium.BackColor = Color.Red;
            else americium.BackColor = Color.LightGreen;

            if (!hasSrFile) strontium.BackColor = Color.Red;
            else strontium.BackColor = Color.LightGreen;

            if (!hasCsFile) cesium.BackColor = Color.Red;
            else cesium.BackColor = Color.LightGreen;

            if (!hasCFile) carbon.BackColor = Color.Red;
            else carbon.BackColor = Color.LightGreen;

            if (!hasRadFile) radon.BackColor = Color.Red;
            else radon.BackColor = Color.LightGreen;
        }

        void fon_Click(object sender, EventArgs e)
        {
            chart();
        }

        void americium_Click(object sender, EventArgs e)
        {
            chart();
            Am();
        }

        void radon_Click(object sender, EventArgs e)
        {
            chart();
            Rad();
        }

        void strontium_Click(object sender, EventArgs e)
        {
            chart();
        }

        void cesium_Click(object sender, EventArgs e)
        {
            chart();
        }

        void carbon_Click(object sender, EventArgs e)
        {
            chart();
        }

        

        public void Form1_Load(object sender, EventArgs e)
        {
            autochart(@"C:\Users\misha\Desktop\Работа\064\Am-241_DAI-064_14.07.2023.xls");
            Am();
            autochart(@"C:\Users\misha\Desktop\Работа\064\Спектр с прокачки ДАИ 0064 одинарный.xls");
            Rad();
            CalculateRegressionCoefficients();
            PlotRegressionLine();
            
            
        }

        private void CalculateRegressionCoefficients()
        {
            float sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            foreach (var point in dataPoints)
            {
                sumX += point.X;
                sumY += point.Y;
                sumXY += point.X * point.Y;
                sumX2 += point.X * point.X;
            }

            float n = dataPoints.Length;
            a = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            b = (sumY - a * sumX) / n;
        }

        private void PlotRegressionLine()
        {
            System.Windows.Forms.DataVisualization.Charting.Series regressionSeries = chart2.Series.Add("Regression Line");
            regressionSeries.ChartType = SeriesChartType.Line;

            float xStart = dataPoints[0].X;
            float xEnd = dataPoints[dataPoints.Length - 1].X;
            float yStart = a * xStart + b;
            float yEnd = a * xEnd + b;

            regressionSeries.Points.AddXY(xStart, yStart);
            regressionSeries.Points.AddXY(xEnd, yEnd);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var point in dataPoints)
            {
                chart2.Series[0].Points.AddXY(point.X, point.Y);
            }
        }
        void chart()
        {
            Application excelApp = new Application();

      
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files (*.xlsx; *.xls)|*.xlsx;*.xls";
            openFileDialog.InitialDirectory = @"C:\Users\misha\Desktop\Работа\064";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Workbook workbook = excelApp.Workbooks.Open(openFileDialog.FileName);

     
                Worksheet worksheet = (Worksheet)workbook.Sheets[1];

         
                Range range = worksheet.UsedRange;

                // Получаем количество строк и столбцов
                int rowCount = range.Rows.Count;
                int columnCount = range.Columns.Count;

    
                var newSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
                newSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

                // Добавляем точки из текущей таблицы на новую серию
                for (int rowIndex = 3; rowIndex <= rowCount; rowIndex++)
                {
                    double xValue = rowIndex - 2;
                    double yValue = Convert.ToDouble(((Range)range.Cells[rowIndex, 2]).Text);
                    newSeries.Points.AddXY(xValue, yValue);
                }

                // Добавляем новую серию на график
                chart1.Series.Add(newSeries);

                // Закрываем приложение Excel
                workbook.Close();
                excelApp.Quit();

                
                chart1.Titles.Clear();
                chart1.Titles.Add("График Спектров");
            }
        }
        
        void autochart(string filePath)
        {
            Application excelApp = new Application();

      
            Workbook workbook = excelApp.Workbooks.Open(filePath);

        
            Worksheet worksheet = (Worksheet)workbook.Sheets[1];

         
            Range range = worksheet.UsedRange;

     
            int rowCount = range.Rows.Count;
            int columnCount = range.Columns.Count;

            // Создаем новую серию для данных из текущей таблицы
            var newSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
            newSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            // Добавляем точки из текущей таблицы на новую серию
            for (int rowIndex = 203; rowIndex <= rowCount; rowIndex++)
            {
                double xValue = rowIndex - 2;
                double yValue = Convert.ToDouble(((Range)range.Cells[rowIndex, 2]).Text);
                newSeries.Points.AddXY(xValue, yValue);
            }

            // Добавляем новую серию на график
            chart1.Series.Add(newSeries);

            workbook.Close();
            excelApp.Quit();

            chart1.Titles.Clear();
            chart1.Titles.Add("График Спектров");
        }
        void Am()
        {
            double AmaxX = 0;
            double AmaxY = 0;
            
            // Перебираем все серии на графике
            foreach (var series in chart1.Series)
            {
                // Перебираем все точки в серии
                foreach (var point in series.Points)
                {
                    // Получаем координаты текущей точки
                    double x = point.XValue;
                    double y = point.YValues[0];

                    // Если найдена точка с большим значением Y, обновляем максимальные координаты
                    if (y > AmaxY)
                    {
                        AmaxX = x;
                        AmaxY = y;
                    }
                }
            }

            var newSeriesAm = new System.Windows.Forms.DataVisualization.Charting.Series();
            newSeriesAm.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            for (int i = 0; i <= AmaxY; i++)
            {
 
                newSeriesAm.Points.AddXY(AmaxX, i);

            }

            // Добавляем новую серию на график
            chart1.Series.Add(newSeriesAm);

            chart1.Series[1].ToolTip = "X = #VALX, Y = #VALY";
            chart1.Series[1].BorderWidth = 2;
            
            // Выводим результаты поиска в MessageBox
            //MessageBox.Show($"Максимальная точка: X = {AmaxX}, Y = {AmaxY}", "Результат поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void Rad()
        {
            // Объявляем переменные для хранения максимальных точек
            double firstMaxX = 0;
            double firstMaxY = 0;
            double secondMaxX = 0;
            double secondMaxY = 0;
            double thirdMaxX = 0;
            double thirdMaxY = 0;

            // Перебираем все серии на графике
            foreach (var series in chart1.Series)
            {
                // Перебираем все точки в серии
                foreach (var point in series.Points)
                {
                    // Получаем координаты текущей точки
                    double x = point.XValue;
                    double y = point.YValues[0];

                    if (y > firstMaxY && x >= 400 && x <= 600)
                    {
                        firstMaxX = x;
                        firstMaxY = y;
                    }
                   
                    else if (y > secondMaxY && x >= 600 && x <= 800)
                    {
                        secondMaxX = x;
                        secondMaxY = y;
                    }
               
                    else if (y > thirdMaxY && x >= 800 && x <= 1000)
                    {
                        thirdMaxX = x;
                        thirdMaxY = y;
                    }
                }
            }

            // Создаем новые серии для каждой максимальной точки
            var firstMaxSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
            firstMaxSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;


            firstMaxSeries.Points.AddXY(firstMaxX, firstMaxY);
            firstMaxSeries.Points.AddXY(firstMaxX, 0);
            
            chart1.Series.Add(firstMaxSeries);

            var secondMaxSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
            secondMaxSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            secondMaxSeries.Points.AddXY(secondMaxX, secondMaxY);
            secondMaxSeries.Points.AddXY(secondMaxX, 0);

            chart1.Series.Add(secondMaxSeries);

            var thirdMaxSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
            thirdMaxSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            thirdMaxSeries.Points.AddXY(thirdMaxX, thirdMaxY);
            thirdMaxSeries.Points.AddXY(thirdMaxX, 0);

            chart1.Series.Add(thirdMaxSeries);

            

            System.Windows.Forms.DataVisualization.Charting.Series pointsSeries = chart1.Series.Add("Points");
            pointsSeries.ChartType = SeriesChartType.Point;

            pointsSeries.Points.AddXY(firstMaxX, firstMaxY);

            pointsSeries.Points.AddXY(secondMaxX, secondMaxY);

            pointsSeries.Points.AddXY(thirdMaxX, thirdMaxY);

          
            pointsSeries.Color = System.Drawing.Color.Black;
            pointsSeries.MarkerSize = 8;

            pointsSeries.ToolTip = "X = #VALX, Y = #VALY";
           
            
        }

        private void chart2_Click_1(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Zoom in
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(20, 40);
            chart1.ChartAreas[0].AxisY.ScaleView.Zoom(20, 40);
            chart1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Zoom out
            chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            chart1.Refresh();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
           
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void chart2_Click(object sender, EventArgs e)
        {
           
        }
    }
}
