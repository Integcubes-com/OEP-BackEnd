using System;

namespace ActionTrakingSystem.DTOs
{
    public class WH_ContractOutageDto
    {
        public int contractOutageId { get; set; }
        public int equipmentId { get; set; }
        public string unit { get; set; }
        public int outageId { get; set; }
        public string outageTitle { get; set; }
        public DateTime nextOutageDate { get; set; }
        public decimal runningHours { get; set; }
        public decimal outageDurationInDays { get; set; }
    }

    public class WH_ContractOutageUser
    {
        public int userId { get; set; }
        public WH_ContractOutageDto contractOutage { get; set; }
    }
    public class WH_Dto
    {
        public int startingId { get; set; }
        public int regionId { get; set; }
        public string regionTitle { get; set; }
        public int siteId { get; set; }
        public string siteTitle { get; set; }
        public int equipmentId { get; set; }
        public string unit { get; set; }
        public decimal startHours { get; set; }
        public DateTime startDate { get; set; }
        public DateTime onmContractExpiry { get; set; }
    }
    public class WH_UserDto
    {
        public WH_Dto result { get; set; }
        public int userId { get; set; }
    }

    public class WH_YearlyDro
    {
        public WH_Dto result { get; set; }
        public int userId { get; set; }
        public int typeId { get; set; }
        public int yearId { get; set; }
    }
    public class WH_SaveHours
    {
        public WH_YearlyModel[] yearlyResult { get; set; }
        public WH_Dto result { get; set; }
        public int userId { get; set; }
        public int typeId { get; set; }
    }
    public class WH_TimeLineDto
    {

        public int siteId { get; set; }
    }
    public class WH_YearlyModel
    {
        public int yearId { get; set; }
        public decimal runningHours { get; set; }
        public decimal yearlyTotal { get; set; }
        public int monthId { get; set; }

    }
    public class WH_TimelapseModel
    {
        public decimal startHour { get; set; }
        public string equipmentId { get; set; }
        public WH_timelapseYear[] yearData { get; set; }
    }
    public class WH_timelapseYear
    {
        public decimal yearlyTotal { get; set; }
        public string outage { get; set; }
        public int yearId { get; set; }

    }
    public class WH_MonthlyModel
    {
        public int yearId { get; set; }
        public decimal runningHours { get; set; }
        public decimal monthlyTotal { get; set; }
        public int monthId { get; set; }
    }
}
