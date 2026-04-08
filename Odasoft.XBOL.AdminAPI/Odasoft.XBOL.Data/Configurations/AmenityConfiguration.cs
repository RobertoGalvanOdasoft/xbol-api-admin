using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Configurations
{
    public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
    {
        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
            builder.Property(a => a.IconIdentifier).HasMaxLength(500);

            builder.HasData(
                new Amenity { Id = 1, Name = "Wifi", IconIdentifier = "Wifi" },
                new Amenity { Id = 2, Name = "Sanitarios", IconIdentifier = "Wc" },
                new Amenity { Id = 3, Name = "Estacionamiento", IconIdentifier = "DirectionsCar" },
                new Amenity { Id = 4, Name = "Accesibilidad", IconIdentifier = "Accessible" },
                new Amenity { Id = 5, Name = "Extintores", IconIdentifier = "FireExtinguisher" },
                new Amenity { Id = 6, Name = "Detectores de humo", IconIdentifier = "Sensors" },
                new Amenity { Id = 7, Name = "Área para fumar", IconIdentifier = "SmokingRooms" },
                new Amenity { Id = 8, Name = "Casilleros", IconIdentifier = "DoorSliding" },
                new Amenity { Id = 9, Name = "Asistencia médica", IconIdentifier = "MedicalServices" },
                new Amenity { Id = 10, Name = "Venta de alimentos y bebidas", IconIdentifier = "Fastfood" },
                new Amenity { Id = 11, Name = "Elevadores", IconIdentifier = "Elevator" },
                new Amenity { Id = 12, Name = "Escaleras eléctricas", IconIdentifier = "Escalator" },
                new Amenity { Id = 13, Name = "Cámaras de seguridad", IconIdentifier = "Videocam" },
                new Amenity { Id = 14, Name = "Módulos de Fan ID", IconIdentifier = "Badge" },
                new Amenity { Id = 16, Name = "Salidas de emergencia", IconIdentifier = "DirectionsRun" },
                new Amenity { Id = 17, Name = "Puntos de reunión", IconIdentifier = "Groups" },
                new Amenity { Id = 18, Name = "Estaciones de carga", IconIdentifier = "BatteryChargingFull" },
                new Amenity { Id = 19, Name = "Módulos de información", IconIdentifier = "Info" }
            );
        }
    }
}
