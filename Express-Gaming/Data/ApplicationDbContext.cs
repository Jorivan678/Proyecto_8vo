using Express_Gaming.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Express_Gaming.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario,Rol,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Usuario>(entityTypeBuilder =>
            {
                entityTypeBuilder.ToTable("Usuarios");
            });

            builder.Entity<Rol>(entityTypeBuilder =>
            {
                entityTypeBuilder.Property(u => u.Descripcion)
                    .HasMaxLength(100)
                    .IsRequired(true)
                    .IsUnicode(true);
            });
        }

        public DbSet<Noticia> Noticias { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ComentarioReview> ComentariosReview { get; set; }
        public DbSet<ComentarioNoticia> ComentariosNoticias { get; set; }
        public DbSet<Contacto> Contactos { get; set; }
    }
}
