using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTraveller.Model
{
    public class TravellerSetting
    {
        #region 私有成员
        private Int64 _travellMillseconds;
        #endregion

        public Int64 TravellMillseconds
        {
            get
            {
                return this._travellMillseconds;
            }
            set
            {
                this._travellMillseconds = value;
            }
        }

        public Int64 TravellSecond
        {
            get
            {
                return this.TravellMillseconds / 1000;
            }
            set
            {
                this.TravellMillseconds = value * 1000;
            }
        }

        public Int64 TravellMinute
        {
            get
            {
                return this.TravellSecond / 60;
            }
            set
            {
                this.TravellSecond = value * 60;
            }
        }

        public Int64 TravellHour
        {
            get
            {
                return this.TravellMinute / 60;
            }
            set
            {
                this.TravellMinute = value * 60;
            }
        }

        public Int64 TravellDay
        {
            get
            {
                return this.TravellHour / 24;
            }
            set
            {
                this.TravellHour = value * 24;
            }
        }
    }
}
