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
using System.Windows.Input;
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
        private Thread thread;
        public MainViewModel()
        {
            SetupModel();
            Random rand = new Random();
            yGenerator = new Random(rand.Next());
            alphaGenerator = new Random(rand.Next());
            thread = new Thread(Update);
        }

        private void SetupModel()
        {
            PlotModel = new PlotModel();

            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

            PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });

            this.RaisePropertyChanged("PlotModel");

        }

        public int TotalNumberOfPoints { get; set; }

        public double LineDistance { get; set; } = 10.0;

        public double NeedleLenght { get; set; } = 9.0;

        public int NumberOfReplications { get; set; } = 5000000;

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
        // distatce between lines


        private ICommand _startSimCommand;

        public ICommand StartSimCommand
        {
            get
            {
                if (_startSimCommand == null)
                {
                    _startSimCommand = new RelayCommand(
                        param => this.StartSimulation(),
                        param => this.CanStartSimulation()
                        );
                }
                return _startSimCommand;
            }
        }

        private void StartSimulation()
        {
            if (!thread.IsAlive)
                thread.Start();
        }

        private bool CanStartSimulation()
        {
            return true;
        }
        private void Update()
        {
            //a - calculated lenght of triangle edge
            //y - generated distance between one of the lines and end of the needle
            //alpha - degree of the needle
            //PI - calculated value of PI 

            double a, y, alpha, PI;

            var s = (LineSeries)PlotModel.Series[0];

            for (int i = 0; i < NumberOfReplications; i++)
            {
                _expCount++;

                y = yGenerator.NextDouble() * LineDistance;

                alpha = alphaGenerator.NextDouble() * 180;

                a = NeedleLenght * Math.Sin(alpha * Math.PI / 180);
                //if the needle intersect with a line
                if (a + y >= LineDistance)
                {
                    _intersectCount++;
                }
                PI = (2 * NeedleLenght * _expCount) / (LineDistance * _intersectCount);

                if (i % 1000 != 0) continue;

                CalculatedPiValue = PI;

                lock (this.PlotModel.SyncRoot)
                {
                    double x = s.Points.Count > 0 ? s.Points[s.Points.Count - 1].X + 1000 : 0;
                    s.Points.Add(new DataPoint(x, PI));

                }
                Thread.Sleep(40);
                RaisePropertyChanged("CalculatedPiValue");

                this.PlotModel.InvalidatePlot(true);
            }

            Console.Write("thread has stoped");
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
