using Microsoft.Extensions.DependencyInjection;
//using AutoMapper;
using Microsoft.Extensions.Hosting;
using MyLibrary;
using MyLibrary.Components.ProjectCsvReader;
using MyLibrary.Components.DataProviders;
using MyLibrary.Components.TxtToCsvConverter;
using MyLibrary.Data.Entities;
using MyLibrary.Data.Repositories;
using MyLibrary.UserCommunication;
using MyLibrary.Data;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using MyLibrary.Components.MappingProfile;

var services = new ServiceCollection();
services.AddSingleton<IApp, App>();
services.AddSingleton<IRepository<Book>, DbRepository<Book>>();
services.AddSingleton<IBooksDataProvider, BooksDataProvider>();
services.AddSingleton<IUserCommunication, UserCommunication>();
services.AddSingleton<ICsvReader, ProjectCsvReader>();
services.AddSingleton<IConvertFileToCsv, ConvertFileToCsv>();
services.AddDbContext<MyLibraryDbContext>();//(options => options
    //.UseSqlServer("Data Source=LAPTOP-R6OVM9N5\\SQLEXPRESS;Initial Catalog=MyLibraryStorage;Integrated Security=True;Trust Server Certificate=True"));
//services.AddAutoMapper(typeof(MappingProfile));

//var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
//var mapper = config.CreateMapper();
var serviceProvider = services.BuildServiceProvider();
var app = serviceProvider.GetService<IApp>()!;
app.Run();


//var convert = serviceProvider.GetService<IConvertFileToCsv>()!;
//convert.Convert();

//services.AddAutoMapper(typeof(MappingProfile));

//var host = Host.CreateDefaultBuilder(args)
//    .ConfigureServices((context, services) =>
//    {
//        services.AddSingleton<IApp, App>();
//        services.AddSingleton<IRepository<Book>, FileRepository<Book>>();
//        services.AddSingleton<IBooksDataProvider, BooksDataProvider>();
//        services.AddSingleton<IUserCommunication, UserCommunication>();
//        services.AddSingleton<ICsvReader, ProjectCsvReader>();
//        services.AddSingleton<IConvertFileToCsv, ConvertFileToCsv>();
//        services.AddDbContext<MyLibraryDbContext>(options => options
//            .UseSqlServer("Data Source=LAPTOP-R6OVM9N5\\SQLEXPRESS;Initial Catalog=MyLibraryStorage;Integrated Security=True;Trust Server Certificate=True"));

//        services.AddSingleton(provider => new MapperConfiguration(cfg =>
//        {
//            cfg.AddProfile(new MappingProfile());
//        }).CreateMapper());

//        services.AddTransient<App>();
//    })
//    .Build();

//var app2 = host.Services.GetRequiredService<App>();
//app2.Run();
