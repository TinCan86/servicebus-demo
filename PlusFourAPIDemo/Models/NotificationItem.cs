using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NotificationApi.Models
{
    public class NotificationContext : DbContext
    {
        public NotificationContext(DbContextOptions<NotificationContext> options)
            : base(options)
        {
        }

        public DbSet<NotificationItem> NotificationItems { get; set; }
    }
}

public class NotificationItem
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; }
    public string Message { get; set; }
}