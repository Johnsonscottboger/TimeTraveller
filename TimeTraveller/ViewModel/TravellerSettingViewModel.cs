using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using TimeTraveller.Command;
using TimeTraveller.Model;
using TimeTraveller.Win32Api;

namespace TimeTraveller.ViewModel
{
    public class TravellerSettingViewModel : INotifyPropertyChanged
    {
        private TravellerSetting _travellerSetting;
        private string _configFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", $"{nameof(TravellerSetting)}.json");
        private static Timer _timer;
        private static bool _isRuning;

        //记录修改之后的当前时间
        private DateTime _currentDateTime = DateTime.Now;

        public event PropertyChangedEventHandler PropertyChanged;

        public TravellerSetting TravellerSetting
        {
            get { return this._travellerSetting; }
            set
            {
                this._travellerSetting = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TravellerSetting)));
            }
        }


        public TravellerSettingViewModel()
        {
            this._travellerSetting = new TravellerSetting()
            {
                TravellMillseconds = 1,
                TravellSecond = 1
            };

            var getTask = GetTravellerSettingAsync();
            getTask.ContinueWith(p =>
            {
                if (p.Result != null)
                {
                    this.TravellerSetting = p.Result;
                }
            });
        }

        #region 读取设置配置
        private Task<TravellerSetting> GetTravellerSettingAsync()
        {
            return Task.Factory.StartNew<TravellerSetting>(() =>
            {
                if (!File.Exists(_configFileName))
                    return null;
                else
                {
                    var content = string.Empty;
                    using (var fs = new FileStream(this._configFileName, FileMode.OpenOrCreate, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(fs))
                        {
                            content = reader.ReadToEnd();
                        }
                    }
                    return JsonConvert.DeserializeObject<TravellerSetting>(content);
                }
            });
        }

        #endregion

        #region 更新设置命令
        /// <summary>
        /// 更新设置执行方法
        /// </summary>
        /// <returns></returns>
        private void UpdateTravellerSetting()
        {
            Task.Factory.StartNew(() =>
            {
                this.TravellerSetting = this.TravellerSetting;
                var content = JsonConvert.SerializeObject(this.TravellerSetting);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var fileInfo = new FileInfo(_configFileName);
                    if (!fileInfo.Directory.Exists)
                    {
                        Directory.CreateDirectory(fileInfo.DirectoryName);
                    }
                    using (var fs = new FileStream(this._configFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 2048, FileOptions.Asynchronous))
                    {
                        using (var writer = new StreamWriter(fs))
                        {
                            writer.Write(content);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 是否可执行更新设置执行方法
        /// </summary>
        /// <returns></returns>
        private bool CanUpdateTravellerSetting()
        {
            if (!File.Exists(_configFileName))
                return true;
            //检测文件是否正在被另一进程占用
            try
            {
                File.Move(_configFileName, _configFileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 更新设置命令
        /// </summary>
        public ICommand UpdateTravellerSettingCommand
        {
            get { return new RelayCommand(UpdateTravellerSetting, CanUpdateTravellerSetting); }
        }
        #endregion

        #region 修改系统时间命令
        /// <summary>
        /// 更新系统时间执行方法
        /// </summary>
        private void UpdateSystemDateTime()
        {
            var callback = new TimerCallback(obj =>
            {
                _currentDateTime = _currentDateTime.AddMilliseconds(this.TravellerSetting.TravellMillseconds);
                var time = new SYSTEMTIME(_currentDateTime);
                Debug.WriteLine($"{_currentDateTime.ToString("yyyy-MM-dd HH:mm:ss tttt")}");
                var success = LocalTime.SetLocalTime(ref time);
            });

            _isRuning = true;
            _timer = new Timer(callback, null, 0, 1000);
        }

        /// <summary>
        /// 是否可执行更新系统时间方法
        /// </summary>
        /// <returns></returns>
        private bool CanUpdateSystemDateTime()
        {
            return !_isRuning;
        }

        /// <summary>
        /// 更新系统时间命令
        /// </summary>
        public ICommand UpdateSystemDateTimeCommand
        {
            get { return new RelayCommand(UpdateSystemDateTime, CanUpdateSystemDateTime); }
        }
        #endregion

        #region 还原系统时间命令
        /// <summary>
        /// 还原系统时间执行方法
        /// </summary>
        private void RestoreSystemDateTime()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            var now = GetInternetDateTime();
            var systime = new SYSTEMTIME(now);
            LocalTime.SetLocalTime(ref systime);
        }

        /// <summary>
        /// 是否可执行还原系统时间方法
        /// </summary>
        /// <returns></returns>
        private bool CanRestoreSystemDateTime()
        {
            return _isRuning;
        }

        /// <summary>
        /// 获取Interner时间
        /// </summary>
        /// <returns></returns>
        private DateTime GetInternetDateTime()
        {
            var url = @"http://www.beijing-time.org/time15.asp";
            var yearPattern = new Regex(@"(?<=nyear=)\d+(?=;)", RegexOptions.Compiled);
            var monthPattern = new Regex(@"(?<=nmonth=)\d+(?=;)", RegexOptions.Compiled);
            var dayPattern = new Regex(@"(?<=nday=)\d+(?=;)", RegexOptions.Compiled);
            var hourPattern = new Regex(@"(?<=nhrs=)\d+(?=;)", RegexOptions.Compiled);
            var minutePattern = new Regex(@"(?<=nmin=)\d+(?=;)", RegexOptions.Compiled);
            var secondPattern = new Regex(@"(?<=nsec=)\d+(?=;)", RegexOptions.Compiled);

            var webClient = new WebClient();
            var html = webClient.DownloadString(url);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var now = DateTime.Now;
            var yearMatch = yearPattern.Match(html);
            var monthMatch = monthPattern.Match(html);
            var dayMatch = dayPattern.Match(html);
            var hourMatch = hourPattern.Match(html);
            var minuteMatch = minutePattern.Match(html);
            var secondMatch = secondPattern.Match(html);

            var year = 0;
            var month = 0;
            var day = 0;
            var hour = 0;
            var minute = 0;
            var second = 0;
            if (!int.TryParse(yearMatch.Value, out year))
            {
                year = now.Year;
            }
            if (!int.TryParse(monthMatch.Value, out month))
            {
                month = now.Month;
            }
            if (!int.TryParse(dayMatch.Value, out day))
            {
                day = now.Day;
            }
            if (!int.TryParse(hourMatch.Value, out hour))
            {
                hour = now.Hour;
            }
            if(!int.TryParse(minuteMatch.Value,out minute))
            {
                minute = now.Minute;
            }
            if(!int.TryParse(secondMatch.Value,out second))
            {
                second = now.Second;
            }
            var newNow = new DateTime(year, month, day, hour, minute, second);
            newNow.Add(stopWatch.Elapsed);
            stopWatch.Stop();
            return newNow;
        }

        /// <summary>
        /// 还原系统时间命令
        /// </summary>
        public ICommand RestoreSystemDateTimeCommand
        {
            get { return new RelayCommand(RestoreSystemDateTime, CanRestoreSystemDateTime); }
        }
        #endregion
    }
}
