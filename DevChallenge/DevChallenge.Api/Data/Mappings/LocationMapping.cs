using DevChallenge.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevChallenge.Api.Data.Mappings
{
    public class LocationMapping : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("ibge");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasColumnName("id")
                   .HasColumnType("CHAR")
                   .HasMaxLength(7)
                   .IsRequired();

            builder.Property(x => x.State)
                   .HasColumnName("state")
                   .HasColumnType("CHAR")
                   .HasMaxLength(2)
                   .IsRequired();

            builder.Property(x => x.City)
                   .HasColumnName("city")
                   .HasColumnType("VARCHAR")
                   .HasMaxLength(80)
                   .IsRequired();

            builder.HasIndex(x => x.Id, "IX_IBGE_Id");

            builder.HasIndex(x => x.State, "IX_IBGE_State");

            builder.HasIndex(x => x.City, "IX_IBGE_City");
        }
    }
}
