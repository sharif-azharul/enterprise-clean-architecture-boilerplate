using Enterprise.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(
            EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.LastName)
                .HasMaxLength(100)
                .IsRequired();
            builder.HasIndex(x => x.Email)
                    .IsUnique();
        }
    }
}
