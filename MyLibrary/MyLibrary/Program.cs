using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyLibrary;
using MyLibrary.Components.CsvHandler;
using MyLibrary.Components.DataProviders;
using MyLibrary.Components.TxtToCsvConverter;
using MyLibrary.Data.Entities;
using MyLibrary.Data.Repositories;
using MyLibrary.UserCommunication;
using MyLibrary.Data;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MyLibrary.Components.InputDataValidation;
using MyLibrary.Components.ExceptionsHandler;

var services = new ServiceCollection();
services.AddSingleton<IApp, App>();
services.AddSingleton<IRepository<Book>, DbRepository<Book>>();
services.AddSingleton<IBooksDataProvider, BooksDataProvider>();
services.AddSingleton<IUserCommunication, UserCommunication>();
services.AddSingleton<IInputDataValidation, InputDataValidation>();
services.AddSingleton<IExceptionsHandler, ExceptionsHandler>();
services.AddSingleton<ICsvReader, ProjectCsvReader>();
services.AddSingleton<IConvertFileToCsv, ConvertFileToCsv>();
services.AddDbContext<MyLibraryDbContext>();

var serviceProvider = services.BuildServiceProvider();
var app = serviceProvider.GetService<IApp>()!;
app.Run();
