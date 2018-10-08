using DatingApp.API.Models;
using DatingApp_IdentityRoles.API.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    //represent a session with a Database
    //need a specific typing to get the custom user, userroles, roles
    /*IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IdentityUserContext<TUser, TKey, TUserClaim, TUserLogin, TUserToken>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>*/        
    public class DataContext : IdentityDbContext<User, Role, int, 
                            IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, 
                            IdentityRoleClaim<int>, IdentityUserToken<int>> 
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}
        //to tell entity framework about our entities we need to give properties
        // public DbSet<Value> Values { get; set; }
        // public DbSet<User> Users { get; set; } //will come from asp.core.entity
        public DbSet<Photo> Photos { get; set; }

        public DbSet<Like> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder){

            //since we are using Identity we need to call this to configure the schema needed for EF
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole => {
                userRole.HasKey(k => new {k.UserId, k.RoleId});
                userRole.HasOne(ur => ur.Role)
                .WithMany( r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

                userRole.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            });

            //we use short hand syntax above
            // builder.Entity<UserRole>()
            // .HasOne(ur => ur.User)
            // .WithMany(u => u.UserRoles)
            // .HasForeignKey(ur => ur.UserId)
            // .IsRequired();

            // builder.Entity<UserRole>()
            // .HasOne(ur => ur.Role)
            // .WithMany( r => r.UserRoles)
            // .HasForeignKey(ur => ur.RoleId)
            // .IsRequired();


            //form the PK: LikerId + LikeeId
            builder.Entity<Like>()
            .HasKey(k => new {k.LikerId, k.LikeeId});

            //Liker could have many likees : 
            //e.g : a user (likee) could like many users and could be liked by many other users (likers)
            builder.Entity<Like>()
            .HasOne(u => u.Liker)
            .WithMany(u => u.Likees)
            .HasForeignKey( f => f.LikerId)
            .OnDelete(DeleteBehavior.Restrict);

            //Likee could have many likers
            //e.g : a user (likee) could like many users and could be liked by many other users (likers)
            builder.Entity<Like>()
            .HasOne(u => u.Likee)
            .WithMany(u => u.Likers)
            .HasForeignKey(f => f.LikeeId)
            .OnDelete(DeleteBehavior.Restrict);


           builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m => m.MessagesSent)
            .HasForeignKey(f => f.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany(m => m.MessagesReceived)
            .HasForeignKey(f => f.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);
        }


    }
}