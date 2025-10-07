using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementRedis.Core.Models;
public class Author
{
    public int AuthorId { get; set; }
    public string Name { get; set; }
    public string Nationality { get; set; }

    public List<Book> Books { get; set; } = new List<Book>();
}
