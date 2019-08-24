using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WPFUserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExecuteSync_Click(object sender, RoutedEventArgs e)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();

            DemoMethods.RunDownloadSync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execution time: { elapsedMs }";
        }

        private async void ExecuteAsync_Click(object sender, RoutedEventArgs e)
        {
            if (cancellationTokenSource.IsCancellationRequested)
                cancellationTokenSource = new CancellationTokenSource();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;

            try
            {
                await DemoMethods.RunDownloadAsync(progress, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                resultsWindow.Text += $"The async download was cancelled. {Environment.NewLine }";
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execution time: { elapsedMs }";
        }

        private void ReportProgress(object sender, ProgressReportModel e)
        {
            dashboardProgress.Value = e.PercentageComplete;
            PrintResults(e.SitesDownloaded);
        }

        private async void ExecuteParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            if (cancellationTokenSource.IsCancellationRequested)
                cancellationTokenSource = new CancellationTokenSource();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            await DemoMethods.RunDownloadParallelAsync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"PARALLEL Total execution time: { elapsedMs }";
        }

        private void CancelOperation_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }

        private void PrintResults(List<WebsiteDataModel> results)
        {
            resultsWindow.Text = "";
            foreach (var item in results)
            {
                resultsWindow.Text += $"{ item.WebsiteUrl } downloaded: { item.WebsiteData.Length } characters long.{ Environment.NewLine }";
            }
        }


    }
}
