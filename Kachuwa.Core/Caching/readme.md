   @ConfigureServices

   //Cache config start
    services.AddCaching();
	//
  services.AddSingleton<ICache, DefaultCache>(); or use redis

   services.AddTransient<KachuwaCacheAttribute>();



   @Configure
     app.UseMiddleware<CacheMiddleware>();
  [KachuwaCache(Duration = 30)]//in second
        public IActionResult Index()