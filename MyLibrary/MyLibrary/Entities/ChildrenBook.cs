using System.ComponentModel;

namespace MyLibrary.Entities;

public class ChildrenBook : Book
{
    public override string ToString() => base.ToString() + " (Children's Book)";
}
