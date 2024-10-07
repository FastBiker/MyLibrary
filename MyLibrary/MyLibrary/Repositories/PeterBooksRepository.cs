﻿using MyLibrary.Entities;

namespace MyLibrary.Repositories;

public class PeterBooksRepositories
{
    private readonly List<PeterBook> _peterBooks = new ();

    public void Add(PeterBook peterBook) 
    {
        peterBook.Id = _peterBooks.Count +1;
        _peterBooks.Add(peterBook);
    }

    public void Save() 
    {
        foreach(var peterBook in _peterBooks) 
        {
            Console.WriteLine(peterBook);
        }
    }

    public PeterBook GetById(int id) 
    {
        return _peterBooks.Single(item => item.Id == id);
    }
}
