using Prometheus;

namespace outRp
{
    class Prometheus
    {
        // private static readonly Counter TickTock = Metrics.CreateCounter("sampleapp_ticks_total", "滴答滴答");
        private static readonly Gauge TotalPlayer = Metrics.CreateGauge("total_player", "服务器总人数");
        
        // Clear Events
        public static void EVENT_Clear()
        {
            RepairStationUsage(0, true);
            ATM_WithdrawEvent(0, true);
            ATM_DepositEvent(0, true);
            Boombox_Usage(0, true);
            Dead_Event(0, true);
        }
        public static void PlayerCounter(int total)
        {
            TotalPlayer.Set((double)total);
        }

        // Araç tamir istasyonları
        private static readonly Gauge TotalRepairStationUsage = Metrics.CreateGauge("total_repair_station_usage", "总计修车点使用量.");
        public static void RepairStationUsage(int value, bool isSet = false)
        {
            if (isSet)
                TotalRepairStationUsage.Set((double)value);
            else
                TotalRepairStationUsage.Inc((double)value);
        }

        // ATM Eventları
        private static readonly Gauge TotalBankWithdraw = Metrics.CreateGauge("total_atm_withdraw", "总计ATM取款次数.");
        private static readonly Gauge TotalBankDeposit = Metrics.CreateGauge("total_atm_deposit", "总计ATM存款次数.");

        public static void ATM_WithdrawEvent(int value, bool isSet = false)
        {
            if (isSet)
                TotalBankWithdraw.Set((double)value);
            else
                TotalBankWithdraw.Inc((double)value);
        }

        public static void ATM_DepositEvent(int value, bool isSet = false)
        {
            if (isSet)
                TotalBankDeposit.Set((double)value);
            else
                TotalBankDeposit.Inc((double)value);
        }

        // Boombox 
        private static readonly Gauge BoomboxUsage = Metrics.CreateGauge("total_boombox_usage", "总计音响使用次数");
        public static void Boombox_Usage(int value, bool isSet = false)
        {
            if (isSet)
                BoomboxUsage.Set((double)value);
            else
                BoomboxUsage.Inc((double)value);
        }

        // Dead
        private static readonly Gauge TotalDead = Metrics.CreateGauge("total_dead", "总计死亡次数.");
        public static void Dead_Event(int value, bool isSet = false)
        {
            if (isSet)
                TotalDead.Set((double)value);
            else
                TotalDead.Inc((double)value);
        }

        
        
    }
}
