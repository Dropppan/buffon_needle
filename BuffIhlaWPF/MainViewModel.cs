﻿using System;
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
            Random rand = new Random(4545454);
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

        int pocExp;
        int pocPret;
        double d = 10.0;
        double l = 9.0;

        private void Update()

        {
            int n = 0;

            double a;
            double y;
            double alpha;
            double PI;

            pocExp++;
            y = yGenerator.NextDouble() * d;
            alpha = alphaGenerator.NextDouble() * 180;
            a = l * Math.Sin(alpha * Math.PI / 180);
            if (a + y >= d)
            {
                pocPret++;
            }
            PI = (2 * l * pocExp) / (d * pocPret);

            var s = (LineSeries)PlotModel.Series[0];

            double x = s.Points.Count > 0 ? s.Points[s.Points.Count - 1].X + 1 : 0;
            //if (s.Points.Count >= 200)
            //    s.Points.RemoveAt(0);

            s.Points.Add(new DataPoint(x, PI));

            n += s.Points.Count;

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