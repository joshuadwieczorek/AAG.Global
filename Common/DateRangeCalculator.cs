using System;
using System.Collections.Generic;
using AAG.Global.Contracts;
using AAG.Global.Enums;
using NodaTime;

namespace AAG.Global.Common
{
    public class DateRangeCalculator
    {
        private List<DateRange> dateRanges { get; init; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public DateRangeCalculator()
        {
            dateRanges = new List<DateRange>();
        }


        /// <summary>
        /// Generate date ranges by start date, end date, and schedule.
        /// </summary>
        /// <param name="dateRange"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public List<DateRange> Generate(
              DateRange dateRange
            , Schedule schedule)
        {
            // Determine period units for NodaTime.
            var periodUnits = schedule switch
            {
                Schedule.ScheduledMonthly => PeriodUnits.Months,
                Schedule.ScheduledDaily => PeriodUnits.Days,
                Schedule.ManualMonthly => PeriodUnits.Months,
                Schedule.ManualDaily => PeriodUnits.Days,
                _ => throw new ArgumentException($"Schedule '{schedule}' is not supported!")
            };

            // Generate date ranges.
            Generate(dateRange.StartDate, dateRange.EndDate, periodUnits);

            // Return date ranges.
            return dateRanges;
        }


        /// <summary>
        /// Generate date ranges.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="periodUnits"></param>
        /// <returns></returns>
        private void Generate(
              DateTime startDate
            , DateTime endDate
            , PeriodUnits periodUnits = PeriodUnits.Months)
        {
            int maxNumber;
            LocalDate ld1 = new LocalDate(startDate.Year, startDate.Month, startDate.Day);
            LocalDate ld2 = new LocalDate(endDate.Year, endDate.Month, endDate.Day);
            Period period = Period.Between(ld1, ld2, periodUnits);
            
            // Set max number to iterate over, days or months.
            maxNumber = periodUnits switch
            {
                PeriodUnits.Months => period.Months,
                PeriodUnits.Days => period.Days,
                _ => throw new ArgumentException("Invalid period unit!")
            };

            // Iterate over days and generate individual date ranges.
            for (int iterator = 0; iterator <= maxNumber; ++iterator)
            {
                if (periodUnits == PeriodUnits.Months)
                {
                    Generate(startDate.Year, startDate.Month, periodUnits: periodUnits);
                    startDate = startDate.AddMonths(1);
                }
                else
                {
                    Generate(startDate.Year, startDate.Month, startDate.Day, periodUnits);
                    startDate = startDate.AddDays(1);
                }                
            }
        }


        /// <summary>
        /// Generate time range and add to list.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="periodUnits"></param>
        private void Generate(
              int year
            , int month
            , int day = 1
            , PeriodUnits periodUnits = PeriodUnits.Months)
        {
            DateTime startDate = new DateTime(year, month, day);
            DateTime endDate;
            if (periodUnits == PeriodUnits.Months)
                endDate = startDate.AddMonths(1).AddMinutes(-1);
            else
                endDate = startDate.AddDays(1).AddMinutes(-1);
            dateRanges.Add(new DateRange(startDate, endDate));
        }
    }
}