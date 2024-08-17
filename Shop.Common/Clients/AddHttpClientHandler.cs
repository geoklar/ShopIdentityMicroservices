	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
	using Polly;
	using Polly.Timeout;
	using Shop.Common.Clients;
	using Shop.Common.Models;
	namespace Shop.Common.Extensions
	{
	    public static class HttpClient<T> {
	        public static void AddHttpClientHandler<T>(IServiceCollection services)
	        {
	            Random jitterer = new Random();
	            services.AddHttpClient<HttpShopClient<T>>()
	            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
	                5,
	                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
	                                + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
	                onRetry: (outcome, timespan, retryAttempt) =>
	                {
	                    var serviceProvider = services.BuildServiceProvider();
	                    serviceProvider.GetService<ILogger<HttpShopClient<T>>>()?
	                        .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}");
	                }
	            ))
	            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
	                3,
	                TimeSpan.FromSeconds(15),
	                onBreak: (outcome, timespan) =>
	                {
	                    var serviceProvider = services.BuildServiceProvider();
	                    serviceProvider.GetService<ILogger<HttpShopClient<T>>>()?
	                        .LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds...");
	                },
	                onReset: () =>
	                {
	                    var serviceProvider = services.BuildServiceProvider();
	                    serviceProvider.GetService<ILogger<HttpShopClient<T>>>()?
	                        .LogWarning($"Closing the circuit...");
	                }
	            ))
	            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
	        }
	    }
	}
