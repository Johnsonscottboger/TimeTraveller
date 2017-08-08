﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
                //var success = LocalTime.SetLocalTime(ref time);
            });

            _isRuning = true;
            _timer = new Timer(callback, null, 0, 1);
        }

        /// <summary>
        /// 是否可执行更新系统时间方法
        /// </summary>
        /// <returns></returns>
        private bool CanUpdateSystemDateTime()
        {
            return !_isRuning;
        }

        public ICommand UpdateSystemDateTimeCommand
        {
            get { return new RelayCommand(UpdateSystemDateTime, CanUpdateSystemDateTime); }
        }
        #endregion


    }
}