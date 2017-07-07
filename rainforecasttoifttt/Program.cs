using System.Linq;
using System.Net.Http;
using System.Text;
using ForecastIO;

namespace rainforecasttoifttt
{
	class Program
	{
		static int Main(string[] args)
		{
			string forecastIOKey = args[0];
			string iftttKey = args[1];
			var forecastRequest = new ForecastIORequest(forecastIOKey, 47.754433f, -122.214010f, Unit.si);
			ForecastIOResponse forecastResponse = forecastRequest.Get();
			double totalRainfall = forecastResponse.daily.data.Where(x => x.precipType != null && x.precipType.Equals("rain")).Sum(x => x.precipIntensity);

			using (var client = new HttpClient())
			{
				string totalRainfallFormatted = totalRainfall.ToString("0.##");

				var content = new StringContent($"{{\"value1\":{totalRainfallFormatted}}}", Encoding.UTF8, "application/json");

				HttpResponseMessage result =
					client.PostAsync($"https://maker.ifttt.com/trigger/weeklyforecast_rain_inches/with/key/{iftttKey}", content).Result;

				if (result.IsSuccessStatusCode)
					return 0;

				return (int) result.StatusCode;
			}
		}
	}
}
