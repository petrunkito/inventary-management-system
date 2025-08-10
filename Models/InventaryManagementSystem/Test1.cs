using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventaryManagementSystem.Models.InventaryManagementSystem;

[Keyless]
[Table("test1")]
public partial class Test1
{
    [Column("isactive")]
    public bool Isactive { get; set; }
}
