﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WU16.BolindersBilAB.DAL.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using WU16.BolindersBilAB.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using WU16.BolindersBilAB.DAL.Repository;
using WU16.BolindersBilAB.DAL.Services;

namespace WU16.BolindersBilAB.Web
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ApplicationDbContext")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<EmailService>();
            services.AddScoped<CarSearchService>();
            services.AddScoped<ImageService>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 2;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });


            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Home/Index"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });

            services.AddMvc();

            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<CarListService>();
            services.AddScoped<CarBrandService>();
            services.AddScoped<LocationService>();
            services.AddScoped<CarService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserManager<ApplicationUser> userManager, ApplicationDbContext _ctx)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStaticFiles();
            }
            else
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    OnPrepareResponse = x =>
                    {
                        const int duration = 60 * 60 * 24 * 365;
                        x.Context.Request.Headers[HeaderNames.CacheControl]
                            = "public,max-age=" + duration;
                    }
                });
            }

            app.UseAuthentication();

            app.UseMvc(x => x.MapRoute("default", template: "{Controller=Home}/{Action=Index}/{Id?}"));



            //if (!_ctx.Locations.Any())
            //{
            //    var locations = new List<Location>
            //    {
            //        new Location{Name="Bolinders Bil Jönköping", Address="Lovsjövägen 33", City="Jönköping", Zip="55626", PhoneNumber="036-123456", Email="jonkoping@bolindersbil.se", Id="BB1"},
            //        new Location{Name="Bolinders Bil Värnamo", Address="Bultgatan 2", City="Värnamo", Zip="54452", PhoneNumber="0370-123456", Email="varnamo@bolindersbil.se", Id="BB2"},
            //        new Location{Name="Bolinders Bil Göteborg", Address="Industrivägen 1", City="Göteborg", Zip="55336", PhoneNumber="031-123456", Email="goteborg@bolindersbil.se", Id="BB3"}
            //    };

            //    _ctx.AddRange(locations);
            //    _ctx.SaveChanges();
            //}

            //if (!_ctx.CarBrands.Any())
            //{
            //    var carBrands = new List<CarBrand>
            //    {
            //        new CarBrand{BrandName="Volvo", ImageUrl="/images/carbrands/bmw-logo.png"},
            //        new CarBrand{BrandName="BMW", ImageUrl="/images/carbrands/ferrari-logo.png"},
            //        new CarBrand{BrandName="Audi", ImageUrl="/images/carbrands/koenigsegg-logo.png"},
            //        new CarBrand{BrandName="Ford", ImageUrl="/images/carbrands/saab-logo.png"},
            //        new CarBrand{BrandName="Mercedes-benz", ImageUrl="/images/carbrands/saab-logo.png"},
            //        new CarBrand{BrandName="Volkswagen", ImageUrl="/images/carbrands/volvo-logo.png"},
            //    };

            //    _ctx.AddRange(carBrands);
            //    _ctx.SaveChanges();
            //}
            //if (!_ctx.Cars.Any())
            //{
            //    var mercedes = _ctx.CarBrands.FirstOrDefault(x => x.BrandName == "Mercedes-benz");
            //    var volvo = _ctx.CarBrands.FirstOrDefault(x => x.BrandName == "Volvo");
            //    var bmw = _ctx.CarBrands.FirstOrDefault(x => x.BrandName == "BMW");
            //    var vw = _ctx.CarBrands.FirstOrDefault(x => x.BrandName == "Volkswagen");

            //    var location = _ctx.Locations.FirstOrDefault(x => x.City == "Jönköping");

            //    var cars = new List<Car>
            //    {
            //        new Car{Location = location, LicenseNumber="ABC123", CarBrand = volvo, CarType=CarType.Kombi, Color="Black", CreationDate = DateTime.Now, LastUpdated = DateTime.Now, Description="<insert description here>", HorsePower=154, Gearbox = Gearbox.Automat, FuelType= FuelType.Bensin, IsLeaseable = false, ModelYear = 2013, Used = true, Milage=13000, Model="v70", Price=55000},
            //        new Car{Location = location, LicenseNumber="CDS123", CarBrand = bmw, CarType=CarType.Coupé, Color="Black", CreationDate = DateTime.Now, LastUpdated = DateTime.Now, Description="<insert description here>", HorsePower=154, Gearbox = Gearbox.Automat, FuelType= FuelType.Bensin, IsLeaseable = false, ModelYear = 2013, Used = true, Milage=13000, Model="v70", Price=55000},
            //        new Car{Location = location, LicenseNumber ="DFG545", CarBrand = mercedes, CarType=CarType.Cab, Color="Red", CreationDate = DateTime.Now, LastUpdated = DateTime.Now, Description="<insert description here>", HorsePower=354, Gearbox = Gearbox.Automat, FuelType= FuelType.Diesel, IsLeaseable = false, ModelYear = 2013, Used = true, Milage=13000, Model="C400", Price=550000},
            //        new Car{Location = location, LicenseNumber="LUL404", CarBrand = vw, CarType=CarType.Småbil, Color="Red", CreationDate = DateTime.Now, LastUpdated = DateTime.Now, Description="<insert description here>", HorsePower=354, Gearbox = Gearbox.Automat, FuelType= FuelType.Bensin, IsLeaseable = false, ModelYear = 2016, Used = true, Milage=5000, Model="Golf GTI", Price=95000},

            //    };
            //    _ctx.AddRange(cars);
            //    _ctx.SaveChanges();
            //}

            //// similar car
            //var vw2 = _ctx.CarBrands.FirstOrDefault(x => x.BrandName == "Volkswagen");
            //var location2 = _ctx.Locations.FirstOrDefault(x => x.City == "Jönköping");
            //_ctx.Cars.Add(new Car { Location = location2, LicenseNumber = "L0L404", CarBrand = vw2, CarType = CarType.Småbil, Color = "Red", CreationDate = DateTime.Now, LastUpdated = DateTime.Now, Description = "<insert description here>", HorsePower = 354, Gearbox = Gearbox.Automat, FuelType = FuelType.Bensin, IsLeaseable = false, ModelYear = 2014, Used = true, Milage = 5000, Model = "Golf GTI", Price = 95000 });
            //_ctx.SaveChanges();


            //// New Seeder

            if (!userManager.Users.Any())
            {
                var user1 = new ApplicationUser { UserName = "jonkoping@bolindersbil.se", Email = "jonkoping@bolindersbil.se" };
                var user2 = new ApplicationUser { UserName = "varnamo@bolindersbil.se", Email = "varnamo@bolindersbil.se" };
                var user3 = new ApplicationUser { UserName = "goteborg@bolindersbil.se", Email = "goteborg@bolindersbil.se" };
                var user4 = new ApplicationUser { UserName = "admin@bolindersbil.se", Email = "admin@bolindersbil.se" };

                Task.WaitAll(userManager.CreateAsync(user1, "Admin1234"));
                Task.WaitAll(userManager.CreateAsync(user2, "Admin1234"));
                Task.WaitAll(userManager.CreateAsync(user3, "Admin1234"));
                Task.WaitAll(userManager.CreateAsync(user4, "Admin1234"));
            }

            if (!_ctx.Locations.Any())
            {
                var locations = new List<Location>
                {
                new Location{Name="Bolinders Bil Jönköping", Address="Lovsjövägen 33", City="Jönköping", Zip="55626", PhoneNumber="036-123456", Email="jonkoping@bolindersbil.se", Id="BB1"},
                new Location{Name="Bolinders Bil Värnamo", Address="Bultgatan 2", City="Värnamo", Zip="54452", PhoneNumber="0370-123456", Email="varnamo@bolindersbil.se", Id="BB2"},
                new Location{Name="Bolinders Bil Göteborg", Address="Industrivägen 1", City="Göteborg", Zip="55336", PhoneNumber="031-123456", Email="goteborg@bolindersbil.se", Id="BB3"}
                };

                var carBrands = new List<CarBrand>()
                {
                new CarBrand{BrandName="Volvo", ImageName="/images/carbrands/bmw-logo.png"},
                new CarBrand{BrandName="BMW", ImageName="/images/carbrands/ferrari-logo.png"},
                new CarBrand{BrandName="Audi", ImageName="/images/carbrands/koenigsegg-logo.png"},
                new CarBrand{BrandName="Ford", ImageName="/images/carbrands/saab-logo.png"},
                new CarBrand{BrandName="Mercedes-benz", ImageName="/images/carbrands/saab-logo.png"},
                new CarBrand{BrandName="Volkswagen", ImageName="/images/carbrands/volvo-logo.png"},
                };

                _ctx.Set<CarBrand>().AddRange(carBrands);
                _ctx.Set<Location>().AddRange(locations);
                _ctx.SaveChanges();
            }

            if (!_ctx.Cars.Any())
            {
                Seeder.Seed<Car>(_ctx, 1000);
            }
        }
    }
}
