using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IAsyncEnumerable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();

            DebugText = string.Empty;

        }
        List<string> fpList = new List<string>() { "a", "b", "c", "d" };
        public string DebugText { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var dateStart = DateTime.Now;  //记录用时的起始时间
            foreach (var item in MockIO(fpList))
            {
                var dateEnd = DateTime.Now;
                var timeSpan = dateEnd - dateStart;//记录开票用时
                DebugText += item + " " + timeSpan.TotalSeconds + "\r\n";

            }

        }

        /// <summary>
        /// 批量开票方法
        /// </summary>
        /// <param name="ls"></param>
        /// <returns></returns>
        public static IEnumerable<string> MockIO(List<string> ls)
        {
            foreach (var item in ls)
            {
                Task.Delay(1000).Wait();
                yield return item;
                Debug.WriteLine(Thread.GetCurrentProcessorId());
            }
        }
        public static async Task<IEnumerable<string>> MockIOAsync(List<string> ls)
        {
            List<string> lsTemp = new List<string>();
            foreach (var item in ls)
            {
                await Task.Delay(1000);
                lsTemp.Add(item);
            }
            return lsTemp;
        }

        public static async IAsyncEnumerable<string> MockIOYieldAsync(List<string> ls)
        {
            foreach (var item in ls)
            {
                Task<Task<string>> task = Task<Task<string>>.Factory.StartNew(async () =>
               {
                   await Task.Delay(1000);
                   return item;

               });

                yield return await task.Result;
            }
        }

        public static async Task<IEnumerable<string>> MockIOPerformanceAsync(List<string> ls)
        {
            List<string> lss = new List<string>();
            List<Task> tasks = new List<Task>();
            foreach (var item in ls)
            {

                Task task = new Task(() =>
              {
                  Task.Delay(1000).Wait();
                  Debug.WriteLine(Thread.GetCurrentProcessorId());
                  lss.Add(item);
              });
                tasks.Add(task);
                task.Start();

            }
            foreach (var item in tasks)
            {
                await item;
            }
            return lss;
        }

        private async void btn_MockIOAsync_Click(object sender, RoutedEventArgs e)
        {
            var dateStart = DateTime.Now;  //记录用时的起始时间

            foreach (var item in await MockIOAsync(fpList))
            {
                var dateEnd = DateTime.Now;
                var timeSpan = dateEnd - dateStart;//记录开票用时
                DebugText += item + " " + timeSpan.TotalSeconds + "\r\n";

            }
            ;
        }  
        private async void btn_MockIOPerformanceAsync_Click(object sender, RoutedEventArgs e)
        {
            var dateStart = DateTime.Now;  //记录用时的起始时间

            foreach (var item in await MockIOPerformanceAsync(fpList))
            {
                var dateEnd = DateTime.Now;
                var timeSpan = dateEnd - dateStart;//记录开票用时
                DebugText += item + " " + timeSpan.TotalSeconds + "\r\n";

            }
            ;
        }
        private async void btn_MockIOYieldAsync_Click(object sender, RoutedEventArgs e)
        {
            var dateStart = DateTime.Now;  //记录用时的起始时间

            await foreach (var item in MockIOYieldAsync(fpList))
            {
                var dateEnd = DateTime.Now;
                var timeSpan = dateEnd - dateStart;//记录开票用时
                DebugText += item + " " + timeSpan.TotalSeconds + "\r\n";

            }
            ;
        }
    }
}
