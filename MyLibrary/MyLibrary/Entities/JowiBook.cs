using System.ComponentModel;

namespace MyLibrary.Entities;

public class JowiBook : Book
{
    public override string ToString() => base.ToString() + "(Jowi's Book)";
}
