using DevChallenge.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevChallenge.Api.Data.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasColumnName("id")
                   .HasColumnType("CHAR")
                   .IsRequired();

            builder.Property(x => x.Username)
                   .HasColumnName("username")
                   .HasColumnType("VARCHAR")
                   .IsRequired();

            builder.Property(x => x.Password)
                   .HasColumnName("password")
                   .HasColumnType("VARCHAR")
                   .IsRequired();

            builder.Property(x => x.Role)
                   .HasColumnName("role")
                   .HasColumnType("CHAR")
                   .IsRequired();

            builder.HasIndex(x => x.Id, "IX_USER_Id")
                   .IsUnique();

            builder.HasIndex(x => x.Username, "IX_USER_Username")
                   .IsUnique();
        }
    }
}
