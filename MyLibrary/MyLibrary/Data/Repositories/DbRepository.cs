﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Data.Entities;
using System.Linq;

namespace MyLibrary.Data.Repositories;

public class DbRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    private readonly MyLibraryDbContext _myLibraryDbContext;
    private readonly DbSet<T> _dbSet;
    private readonly Action<T>? _itemAddedCallback;
    private readonly Action<T>? _itemRemovedCallback;

    public DbRepository(MyLibraryDbContext myLibraryDbContext, Action<T>? itemAddedCallback = null, Action<T>? itemRemovedCallback = null)
    {
        _myLibraryDbContext = myLibraryDbContext;
        _dbSet = _myLibraryDbContext.Set<T>();
        _itemAddedCallback = itemAddedCallback;
        _itemRemovedCallback = itemRemovedCallback;
    }

    public event EventHandler<T> ItemAdded;
    public event EventHandler<T> ItemRemoved;

    public void Add(T item) 
    {
        item.Id = 0;
        _dbSet.Add(item);
        _itemAddedCallback?.Invoke(item);
        ItemAdded?.Invoke(this, item);
    }

    public IEnumerable<T> GetAll()
    {
        var dataFromDb = (IEnumerable<T>)_dbSet.ToList();
        if (dataFromDb != null)
        {
            return dataFromDb;
        }
        else if (dataFromDb == null)
        {
            return new List<T>(); //Enumerable.Empty<T>();
        }
        else
        {
            throw new Exception("Wystąpił problem z pobraniem danych z bazy!");
        }
        
    }

    public T? GetById(int id)
    {
        return _dbSet.Find(id);
        //return _dbSet.FirstOrDefault(x => x.Id == id);
        //return GetAll().SingleOrDefault(x => x.Id == id);

    }

    public void Remove(T item)
    {
        _dbSet.Remove(item);
        _itemRemovedCallback?.Invoke(item);
        ItemRemoved?.Invoke(this, item);
    }

    public void Save()
    {
        _myLibraryDbContext.SaveChanges();
    }
}

//``csharp
//public class EmployeeRepository
//{
//    private string connectionString;

//    public EmployeeRepository(string connString)
//    {
//        connectionString = connString;
//    }

//    public List<Employee> GetAllEmployees()
//    {
//        using (SqlConnection connection = new SqlConnection(connectionString))
//        {
//            connection.Open();
//            // Tutaj możesz wykonywać zapytania do bazy danych
//        }
//    }
//}
