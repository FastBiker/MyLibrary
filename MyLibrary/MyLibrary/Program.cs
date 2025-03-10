﻿using Microsoft.Extensions.DependencyInjection;
using MyLibrary;
using MyLibrary.Components.ProjectCsvReader;
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
services.AddSingleton<ICsvReader, ProjectCsvReader>();
services.AddSingleton<IConvertFileToCsv, ConvertFileToCsv>();

var serviceProvider = services.BuildServiceProvider();
var app = serviceProvider.GetService<IApp>()!;
app.Run();

//var convert = serviceProvider.GetService<IConvertFileToCsv>()!;
//convert.Convert();

//Data Source=LAPTOP-R6OVM9N5\SQLEXPRESS;Initial Catalog=MyLibraryStorage;Integrated Security=True;Trust Server Certificate=True