using Microsoft.Extensions.DependencyInjection;
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

var services = new ServiceCollection();
services.AddSingleton<IApp, App>();
services.AddSingleton<IRepository<Book>, ListRepository<Book>>();
services.AddSingleton<IRepository<Book>, SqlRepository<Book>>();
services.AddSingleton<IRepository<Book>, FileRepository<Book>>();
services.AddSingleton<IBooksDataProvider, BooksDataProvider>();
services.AddSingleton<IUserCommunication, UserCommunication>();
services.AddSingleton<ICsvReader, ProjectCsvReader>();
services.AddSingleton<IConvertFileToCsv, ConvertFileToCsv>();
services.AddDbContext<MyLibraryDbContext>(options => options
    .UseSqlServer("Data Source=LAPTOP-R6OVM9N5\\SQLEXPRESS;Initial Catalog=MyLibraryStorage;Integrated Security=True;Trust Server Certificate=True"));

var serviceProvider = services.BuildServiceProvider();
var app = serviceProvider.GetService<IApp>()!;
app.Run();

//var convert = serviceProvider.GetService<IConvertFileToCsv>()!;
//convert.Convert();
