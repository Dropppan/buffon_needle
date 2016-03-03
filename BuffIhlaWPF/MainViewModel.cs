using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuffIhlaWPF.Annotations;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using LinearAxis = OxyPlot.Axes.LinearAxis;

namespace BuffIhlaWPF
{
    using OxyPlot;
    using OxyPlot.Series;


    public class MainViewModel : INotifyPropertyChanged
    {
        private const int UpdateInterval = 1;

        private readonly Timer timer;
        private readonly Stopwatch watch = new Stopwatch();
        private int numberOfSeries;
        private Random yGenerator;
        private Random alphaGenerator;

        public MainViewModel()
        {
            this.timer = new Timer(OnTimerElapsed);
            SetupModel();
            Random rand = new Random();
            yGenerator = new Random(rand.Next());
            alphaGenerator= new Random(rand.Next());
        }

        private void SetupModel()
        {
            PlotModel = new PlotModel();
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

            PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });

            this.watch.Start();

            this.RaisePropertyChanged("PlotModel");

            this.timer.Change(1000, UpdateInterval);
        }

        public int TotalNumberOfPoints { get; set; }
        public double CalculatedPiValue { get; private set; }

        public PlotModel PlotModel { get; private set; }

        private void OnTimerElapsed(object state)
        {
            lock (this.PlotModel.SyncRoot)
            {
                this.Update();
            }

            this.PlotModel.InvalidatePlot(true);
        }

        int _expCount;
        int _intersectCount;
        double _lineDistance = 10.0;
        double _needleLenght = 9.0;

        private void Update()

        {
            //a - calculated lenght of triangle edge
            //y - generated distance between one of the lines and end of the needle
            //alpha - degree of the needle
            //PI - calculated value of PI 
            double a, y, alpha, PI;

            _expCount++;
            y = yGenerator.NextDouble() * _lineDistance;
            alpha = alphaGenerator.NextDouble() * 180;
            a = _needleLenght * Math.Sin(alpha * Math.PI / 180);
            if (a + y >= _lineDistance)
            {
                _intersectCount++;
            }
            PI = (2 * _needleLenght * _expCount) / (_lineDistance * _intersectCount);

            var s = (LineSeries)PlotModel.Series[0];

            double x = s.Points.Count > 0 ? s.Points[s.Points.Count - 1].X + 1 : 0;
            //if (s.Points.Count >= 200)
            //    s.Points.RemoveAt(0);

            s.Points.Add(new DataPoint(x, PI));

            CalculatedPiValue = PI;
            RaisePropertyChanged("CalculatedPiValue");

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }

}
