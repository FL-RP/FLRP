using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Data;
using outRp.Utils;

namespace outRp.Models
{
    public class PlantModel
    {
        public virtual int Id { get; set; }
        public virtual Position Pos { get; set; }
        public virtual PlantUtil.TypeList Type { get; set; }
        public virtual int Timer { get; set; }
        public virtual bool LessWaterStatus { get; set; }
        public virtual DateTime startTime { get; set; }
        public PlantModel(int id, Position pos, PlantUtil.TypeList type, int timer, bool LessWaterStatus, DateTime startTime)
        {
            Id = id;
            Pos = pos;
            Type = type;
            Timer = timer;
            this.LessWaterStatus = LessWaterStatus;
            this.startTime = startTime;
        }
    }
}
