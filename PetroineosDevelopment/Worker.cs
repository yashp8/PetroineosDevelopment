using System.Collections;
using System.Globalization;
using CsvHelper;
using Newtonsoft.Json;
using Services;

namespace PetroineosDevelopment;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _config;
    PowerService PowerService = new PowerService();

    public Worker(IConfiguration config, ILogger<Worker> logger)
    {
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            string s = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone).ToString("HHmm");
            string y = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone).ToString("yyyyMMdd");
            string timer = _config.GetValue<string>("MySettings:timer");
            var csvPath = _config.GetValue<string>("MySettings:csvPath");
            var filename = "PowerPosition_" + y + "_" + s + ".csv";
            csvPath = csvPath + filename;

            //////JSON//////
            var jPath = _config.GetValue<string>("MySettings:csvPath");
            var jsonfilename = "JSON_PowerPosition_" + y + "_" + s + ".csv";
            var jsonfilePath = jPath + jsonfilename;
            //////JSON//////

            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime FirstPeriod = DateTime.Today.AddHours(23);
                FirstPeriod = FirstPeriod.AddDays(-1);
                var TradeData = await PowerService.GetTradesAsync(FirstPeriod);

                //////JSON//////
                var json = JsonConvert.SerializeObject(TradeData);
                File.WriteAllText(jsonfilePath, json);
                //////JSON//////

                ArrayList al = new ArrayList();
                foreach (dynamic element in TradeData)
                {
                    foreach (var p in element.Periods)
                    {
                        al.Add(p);
                    }
                }
                var list = new List<PowerTrade>();

                foreach (dynamic als in al)
                {
                    if (als.Period == 1)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "23:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 2)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "00:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 3)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "01:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 4)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "02:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 5)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "03:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 6)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "04:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 7)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "05:05", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 8)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "06:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 9)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "07:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 10)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "08:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 11)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "09:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 12)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "10:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 13)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "11:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 14)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "12:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 15)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "13:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 16)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "14:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 17)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "15:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 18)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "16:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 19)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "17:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 20)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "18:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 21)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "19:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 22)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "20:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 23)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "21:00", Volume = als.Volume }
                            );
                    }
                    else if (als.Period == 24)
                    {
                        list.Add(
                            new PowerTrade { LocalTime = "22:00", Volume = als.Volume }
                            );
                    }
                }

                Dictionary<string, double> totals = new Dictionary<string, double>();
                foreach (var e in list)
                {
                    if (!totals.Keys.Contains(e.LocalTime))
                    {
                        totals[e.LocalTime] = 0;
                    }
                    totals[e.LocalTime] += Convert.ToInt32(e.Volume);
                }

                using (var writer = new StreamWriter(csvPath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteHeader<PowerTrade>();
                    csv.NextRecord();
                    csv.WriteRecords(totals);
                }

                _logger.LogInformation("File read successfully at: {time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromMinutes(Int32.Parse(timer)), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Something went wrong at: {time} Error: " + ex, DateTimeOffset.Now);
            throw;
        }

    }
}


//public class PowerTrade
//{
//    public string LocalTime { get; set; }
//    public double Volume { get; set; }
//}