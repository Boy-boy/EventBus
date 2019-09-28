using System;
using Microsoft.EntityFrameworkCore;

namespace WebAppSubscription1
{
    public class SubscriptionDbContext: DbContext
    {
        public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options)
            : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }
    }

    public class Student
    {
        public Guid Id { get; set; }
        public int Age { get; set; }
    }
}
