using System.ComponentModel;

namespace MyLibrary.Entities;

public class PeterBook : Book
{
    public override string ToString() => base.ToString() + " (Peter's Book)";
}

