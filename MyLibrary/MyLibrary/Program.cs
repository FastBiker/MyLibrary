using Microsoft.Extensions.DependencyInjection;
using MyLibrary;
using MyLibrary.Components.DataProviders;
using MyLibrary.Components.TxtToCsvConverter;
using MyLibrary.Data.Entities;
using MyLibrary.Data.Repositories;
using MyLibrary.UserCommunication;

var services = new ServiceCollection();
services.AddSingleton<IApp, App>();
services.AddSingleton<IRepository<Book>, ListRepository<Book>>();
services.AddSingleton<IRepository<Book>, SqlRepository<Book>>();
services.AddSingleton<IRepository<Book>, FileRepository<Book>>();
services.AddSingleton<IBooksDataProvider, BooksDataProvider>();
services.AddSingleton<IUserCommunication, UserCommunication>();
services.AddSingleton<IConvertFileTxtToCsv, ConvertFileTxtToCsv>();

var serviceProvider = services.BuildServiceProvider();
var app = serviceProvider.GetService<IApp>()!;
app.Run();

//var convert = serviceProvider.GetService<IConvertFileTxtToCsv>()!;
//convert.ConvertWithReadAllLines();
